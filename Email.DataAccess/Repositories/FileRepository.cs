using System;
using System.IO;
using Email.DataAccess.IRepositories;

namespace Email.DataAccess.Repositories
{
    public class FileRepository:IFileRepository, IDisposable
    {
        public FileStream GetFileByPath(string path)
        {
            return File.Exists(path) ? File.OpenRead(path) : null;
        }

        public void Dispose()
        {
            
        }
    }
}