using Microsoft.AspNetCore.Http;
using System;
namespace UyCommerce.Application.Contracts.Persistance.Services
{
	public interface IFileService
	{

		Task SaveFile(string name, Stream stream, string folder);
        void DeleteFile(List<string> fileNames, string folder);
		bool FilesExists(List<string> fileNames, string folder);
	}
}

