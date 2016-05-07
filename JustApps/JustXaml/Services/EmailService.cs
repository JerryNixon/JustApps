using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.Storage;

namespace JustXaml.Services
{
    public class EmailService
    {
        public async Task SendAsync(string to, string subject, string body, params EmailAttachment[] attachments)
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
            };
            message.To.Add(new EmailRecipient(to));
            foreach (var item in attachments)
            {
                message.Attachments.Add(item);
            }
            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        public async Task<EmailAttachment> CreateAttachment(string body, string name = "Attachment.txt")
        {
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, body);
            var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file);
            return new EmailAttachment(file.Name, stream);
        }
    }
}
