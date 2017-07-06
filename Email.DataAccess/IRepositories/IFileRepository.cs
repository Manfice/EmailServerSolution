using System.IO;

namespace Email.DataAccess.IRepositories
{
    public interface IFileRepository
    {
        FileStream GetFileByPath(string path);
    }
}