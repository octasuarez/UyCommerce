using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;
using UyCommerce.Application.Contracts.Persistance.Services;

namespace UyCommerce.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment) => _environment = environment;

        public async Task<bool> SaveFile(string name, Stream stream, string folder)
        {
            var basePath = Path.Combine(_environment.WebRootPath, folder);

            if (stream.Length < 0)
            {
                return false;
            }

            var path = basePath + "/" + name;
            using var destinationStream = new FileStream(path, FileMode.Create);
            await stream.CopyToAsync(destinationStream);
            return Directory.Exists(path);
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

