using System.IO;

namespace Mod4Task.Mocking
{
    public class PathProvider : IPathProvider
    {
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
