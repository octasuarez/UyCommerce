using Microsoft.AspNetCore.Hosting;
using UyCommerce.Application.Contracts.Persistance.Services;
using UyCommerce.Domain.Exceptions;

namespace UyCommerce.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment) => _environment = environment;

        public async Task SaveFile(string name, Stream stream, string folder)
        {
            var basePath = Path.Combine(_environment.WebRootPath, folder);
            var path = basePath + "/" + name;

            if (stream.Length < 0) throw new Exception("Wrong file length");

            if (this.FilesExists([name],folder))
            {
                throw new FileExistsException(name);
            }

            using var destinationStream = new FileStream(path, FileMode.Create);
            await stream.CopyToAsync(destinationStream);
        }



        public void DeleteFile(List<string> fileNames, string folder)
        {

            var basePath = Path.Combine(_environment.WebRootPath, folder);

            foreach (var fn in fileNames)
            {

                File.Delete(Path.Combine(basePath, fn));
            }
        }

        public bool FilesExists(List<string> fileNames, string folder)
        {
            foreach(var fn in fileNames)
            {
                if (Directory.Exists(Path.Combine(_environment.WebRootPath, folder) + "/" + fn))
                    return true;
            }
            return false;
        }
    }
}

