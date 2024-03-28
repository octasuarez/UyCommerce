using System;
namespace UYCommerce.Services
{
	public interface IFileService
	{

		Task SaveFile(List<IFormFile> files, string folder);
	}
}

