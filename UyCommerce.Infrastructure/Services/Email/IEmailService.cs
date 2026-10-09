using System;


namespace UyCommerce.Infrastructure.Services.Email
{
	public interface IEmailService
	{

		void SendEmail(Message message);
	}
}

