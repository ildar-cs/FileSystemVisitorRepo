using Mod4Task.Mocking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mod4Task
{
    public class FileSystemVisitor
    {
        private readonly string _startPath;
        private IDirectoryProvider _directoryProvider;
        private IFileProvider _fileProvider;
        private IPathProvider _pathProvider;
        private readonly Predicate<string> _filterName;
        private readonly List<string> _entityNames;
        private bool _canceled;
        public event EventHandler Started;
        public event EventHandler Finished;
        public event EventHandler<FindedEventArgs> FileFinded;
        public event EventHandler<FindedEventArgs> DirectoryFinded;
        public event EventHandler<FilterEventArgs> FilteredFileFinded;
        public event EventHandler<FilterEventArgs> FilteredDirectoryFinded;

        public FileSystemVisitor(
            string startPath,
            IDirectoryProvider directoryProvider,
            IFileProvider fileProvider,
            IPathProvider pathProvider)
        {
            this._startPath = startPath;
            this._directoryProvider = directoryProvider;
            this._fileProvider = fileProvider;
            this._pathProvider = pathProvider;
            this._entityNames = new List<string>();
            _pathProvider = pathProvider;
        }

        public FileSystemVisitor(string startPath,
            IDirectoryProvider directoryProvider,
            IFileProvider fileProvider,
            IPathProvider pathProvider,
            Predicate<string> filterName) : this(startPath, directoryProvider, fileProvider, pathProvider)
        {
            this._filterName = filterName;
        }

        public void GetAllFilesAndDirectories()
        {
            Started?.Invoke(this, new EventArgs());

            GetDirectories(this._startPath);

            Finished?.Invoke(this, new EventArgs());

            if(this._canceled)
                this._canceled = false;
        }

        public void GetDirectories(string directoryName)
        {
            if (_canceled)
                return;

            FindDirectory(directoryName);

            foreach (var entity in this._fileProvider.GetFiles(directoryName))
            {
                FindFile(entity);

                if (this._filterName != null)
                {
                    if (!this._filterName(_pathProvider.GetFileName(entity))) continue;

                    FilterEventArgs filterFilesEventArgs = new FilterEventArgs();

                    filterFilesEventArgs.Name = _pathProvider.GetFileName(entity);

                    FilteredFileFinded?.Invoke(this, filterFilesEventArgs);

                    if (filterFilesEventArgs.Cancel)
                        _canceled = true;

                    if (!filterFilesEventArgs.ExcludeNames.Any(f => f.Equals(_pathProvider.GetFileName(entity))))
                        _entityNames.Add(this._pathProvider.GetFileName(entity));
                }
            }

            foreach (var entity in this._directoryProvider.GetDirectories(directoryName))
            {
                GetDirectories(entity);

                if (this._filterName != null)
                {
                    var name = this._pathProvider.GetFileName(entity);

                    if (!this._filterName(name)) continue;

                    FilterEventArgs filterDirectoriesEventArgs = new FilterEventArgs();
                    filterDirectoriesEventArgs.Name = name;

                    FilteredDirectoryFinded?.Invoke(this, filterDirectoriesEventArgs);

                    if (filterDirectoriesEventArgs.Cancel)
                        return;

                    if (!filterDirectoriesEventArgs.ExcludeNames.Any(f => f.Equals(name)))
                        _entityNames.Add(name);
                }

            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (this._entityNames.Count == 0)
                GetDirectories(this._startPath);

            foreach (var entityName in this._entityNames)
            {
                yield return entityName;
            }
        }

        private void FindDirectory(string entity)
        {
            var args = new FindedEventArgs()
            {
                Name = this._pathProvider.GetFileName(entity),
                FullName = entity,
                EntityType = typeof(Directory)
            };

            DirectoryFinded?.Invoke(this, args);

            AddToList(entity);
        }

        private void FindFile(string entity)
        {
            var args = new FindedEventArgs()
            {
                Name = this._pathProvider.GetFileName(entity),
                FullName = entity,
                EntityType = typeof(File)
            };

            FileFinded?.Invoke(this, args);

            AddToList(entity);
        }

        private void AddToList(string entity)
        {
            if (this._filterName == null)
                _entityNames.Add(this._pathProvider.GetFileName(entity));
        }
    }
}
