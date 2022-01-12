using System.IO;

namespace Mod4Task.Mocking
{
    public class FileProvider : IFileProvider
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}
