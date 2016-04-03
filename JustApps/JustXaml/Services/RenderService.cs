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

namespace JustXaml.Services
{
    public class RenderService : BindableBase
    {
        string _XamlString = default(string);
        public string XamlString { get { return _XamlString; } set { Set(ref _XamlString, value); XamlStringChanged(value); } }
        private void XamlStringChanged(string value)
        {
            Render(value);
        }

        UIElement _XamlResult = default(UIElement);
        public UIElement XamlResult { get { return _XamlResult; } set { Set(ref _XamlResult, value); } }

        string _ErrorShortMessage = default(string);
        public string ErrorShortMessage { get { return _ErrorShortMessage; } set { Set(ref _ErrorShortMessage, value); } }

        string _ErrorLongMessage = default(string);
        public string ErrorLongMessage { get { return _ErrorLongMessage; } set { Set(ref _ErrorLongMessage, value); } }

        bool _Enabled = true;
        public bool Enabled { get { return _Enabled; } set { Set(ref _Enabled, value); } }

        string _TemplatePath = default(string);
        public string TemplatePath { get { return _TemplatePath; } set { Set(ref _TemplatePath, value); TemplatePathChanged(value); } }
        async void TemplatePathChanged(string value)
        {
            await ReadFileAsync(value);
        }

        public async void Clear()
        {
            await ReadFileAsync("ms-appx:///Templates/Default.txt");
        }

        internal async Task ReadFileAsync(Models.File value)
        {
            await ReadFileAsync(value.FileInformation.Path);
        }

        private async Task ReadFileAsync(string value)
        {
            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                throw new InvalidCastException(value);
            }
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            XamlString = await FileIO.ReadTextAsync(file);
        }

        void Render(string xaml)
        {
            if (!Enabled)
                return;
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
