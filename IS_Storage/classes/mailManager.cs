using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IS_Storage.classes
{
    static public class mailManager
    {
        static public async void Sending(MailMessage text, string file = null)
        {
            if (file != null)
            {
                text.Attachments.Add(new Attachment(file));
            }
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            SmtpClient smtp = new SmtpClient()
            {
                Port = 587,
                Host = "smtp.rambler.ru",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("is_storage@rambler.ru", "115544Gg"),
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            await smtp.SendMailAsync(text);

        }
    }
}
