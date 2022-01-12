using System.IO;

namespace Mod4Task.Mocking
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
