
namespace UyCommerce.Application.Contracts.Persistance.Services
{
	public interface IEmailService
	{

		Task SendEmail(string toAddress, string subject, string content);
	}
}

