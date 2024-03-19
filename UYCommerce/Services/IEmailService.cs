using System;
using UYCommerce.Models;


namespace UYCommerce.Services
{
	public interface IEmailService
	{

		void SendEmail(Message message);
	}
}

