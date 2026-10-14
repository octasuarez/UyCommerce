
namespace UyCommerce.Application.Contracts.Persistance.Services
{
	public interface IEmailService
	{

		void SendEmail(string toAddress, string subject, string content);
	}
}

