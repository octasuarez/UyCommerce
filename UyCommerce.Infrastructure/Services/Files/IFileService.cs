using Microsoft.AspNetCore.Http;
using System;
namespace UyCommerce.Infrastructure.Services.Files
{
	public interface IFileService
	{

		Task SaveFile(List<IFormFile> files, string folder);
		void DeleteFile(List<string> fileNames, string folder);
	}
}

