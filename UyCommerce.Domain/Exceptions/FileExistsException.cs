
namespace UyCommerce.Domain.Exceptions
{
    public class FileExistsException : Exception
    {
        public FileExistsException(string fileName) : base($"{fileName} already exists.")
        {}
    }
}
