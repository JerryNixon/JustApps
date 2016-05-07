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
        Services.FileService _FileService;
        public async Task OnNavigatedToAsync(object parameter, IDictionary<string, object> state)
        {
            _FileService = Services.FileService.Instance;
            await Task.CompletedTask;
        }

        UIElement _XamlResult = default(UIElement);
        public UIElement XamlResult { get { return _XamlResult; } private set { Set(ref _XamlResult, value); } }
        public void FillXamlResult(string xaml)
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
        public async Task FillXamlResultAsync(Models.File file)
        {
            if (Enabled)
            {
                XamlString = await _FileService.ReadTextAsync(file);
            }
        }

        string _ErrorShortMessage = default(string);
        public string ErrorShortMessage { get { return _ErrorShortMessage; } set { Set(ref _ErrorShortMessage, value); } }

        string _ErrorLongMessage = default(string);
        public string ErrorLongMessage { get { return _ErrorLongMessage; } set { Set(ref _ErrorLongMessage, value); } }

        bool _Enabled = true;
        public bool Enabled { get { return _Enabled; } set { Set(ref _Enabled, value); } }

        string _XamlString = default(string);
        public string XamlString { get { return _XamlString; } set { Set(ref _XamlString, value); XamlStringPropertyChanged(value); } }
        private void XamlStringPropertyChanged(string value)
        {
            if (Enabled)
            {
                FillXamlResult(value);
            }
        }

        #region User Actions

        public async void Clear()
        {
            var path = new Uri("ms-appx:///Templates/Default.xaml");
            XamlString = await _FileService.ReadTextAsync(path);
        }

        #endregion
    }
}
