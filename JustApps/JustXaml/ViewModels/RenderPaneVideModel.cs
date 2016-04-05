using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JustXaml.Models;
using Template10.Mvvm;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace JustXaml.ViewModels
{
    public class RenderPaneViewModel : BindableBase
    {
        public async Task OnNavigatedToAsync(object parameter, IDictionary<string, object> state)
        {
            await Task.CompletedTask;
        }

        UIElement _XamlResult = default(UIElement);
        public UIElement XamlResult { get { return _XamlResult; } set { Set(ref _XamlResult, value); } }

        string _ErrorShortMessage = default(string);
        public string ErrorShortMessage { get { return _ErrorShortMessage; } set { Set(ref _ErrorShortMessage, value); } }

        string _ErrorLongMessage = default(string);
        public string ErrorLongMessage { get { return _ErrorLongMessage; } set { Set(ref _ErrorLongMessage, value); } }

        bool _Enabled = true;
        public bool Enabled { get { return _Enabled; } set { Set(ref _Enabled, value); } }

        string _XamlString = default(string);
        public string XamlString { get { return _XamlString; } set { Set(ref _XamlString, value); XamlStringChanged(value); } }
        private void XamlStringChanged(string value)
        {
            if (!Enabled)
                return;
            Render(value);
        }

        #region User Actions

        public async void Clear()
        {
            await ClearAsync();
        }

        #endregion

        public async void Email()
        {
            var message = new Windows.ApplicationModel.Email.EmailMessage();
            message.Body = $"Hi,{Environment.NewLine}Question...{Environment.NewLine}{Environment.NewLine}{XamlString}";
            message.Subject = "Just Code XAML Question";

            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Syntax.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, XamlString);
            var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(file);
            var attachment = new Windows.ApplicationModel.Email.EmailAttachment(file.Name, stream);
            message.Attachments.Add(attachment);


            var recipient = new Windows.ApplicationModel.Email.EmailRecipient("jnixon@microsoft.com");
            message.To.Add(recipient);

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(message);
        }

        public async Task ClearAsync()
        {
            var uri = new Uri("ms-appx:///Templates/Default.txt");
            var file = await Models.File.GetAsync(uri);
            XamlString = await file.ReadTextAsync();
        }

        public async Task RenderAsync(Models.File file)
        {
            if (!Enabled)
                return;
            XamlString = await file.ReadTextAsync();
        }

        void Render(string xaml)
        {
            ErrorLongMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(xaml))
            {
                ErrorShortMessage = "XAML is empty";
            }
            else
            {
                ErrorShortMessage = string.Empty;
                try
                {
                    XamlResult = XamlReader.Load(XamlString) as UIElement;
                }
                catch (Exception ex)
                {
                    ErrorLongMessage = ErrorLongMessage;
                    var a = Regex.Split(ex.Message, Environment.NewLine);
                    ErrorShortMessage = $"{a.Last()}";
                }
            }
        }
    }
}
