using System;
using MimeKit;

namespace UyCommerce.Infrastructure.Services.Email
{
    public class Message
    {
        public MailboxAddress? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }

        public Message(string to, string subject, string content)
        {
            To = new MailboxAddress(to, to);
            Subject = subject;
            Content = content;
        }

        public Message() { }
    }

}

