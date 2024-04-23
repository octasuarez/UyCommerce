using System;
using UYCommerce.Data;

namespace UYCommerce.Services
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

