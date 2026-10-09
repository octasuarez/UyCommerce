using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using UyCommerce.Infrastructure.Services;

namespace UyCommerce.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;


        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task SaveFile(List<IFormFile> files, string folder)
        {

            var basePath = Path.Combine(_environment.WebRootPath, folder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var path = basePath + "/" + file.FileName;
                    using Stream stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
            }

        }

        public void DeleteFile(List<string> fileNames, string folder) {

            var basePath = Path.Combine(_environment.WebRootPath, folder);

            foreach(var fn in fileNames) {

                File.Delete(Path.Combine(basePath,fn));
            }
        }

    }
}

