using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.Storage.AccessCache;
using JustXaml.Models;

namespace JustXaml.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Services.FileService _FileService;
        Services.MruService _MruService;
        Services.FutureService _FutureService;
        Services.SettingsService _SettingsService;
        Services.EmailService _EmailService;
        Services.JumpListService _JumpListService;

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                foreach (var item in Enumerable.Range(1, 3))
                {
                    FilePane.RecentFiles.Add(new Models.File { Title = $"File {item}" });
                    FilePane.FolderFiles.Add(new Models.File { Title = $"File {item}" });
                    SamplePane.RecentSamples.Add(new Models.File { Title = $"File {item}" });
                    SamplePane.FilteredSamples.Add(new Models.File { Title = $"File {item}" });
                    SamplePane.UnfilteredSamples.Add(new Models.Folder { Title = $"Folder {item}" });
                }
            }
            else
            {
                _FutureService = new Services.FutureService();
                _FileService = Services.FileService.Instance;
                _MruService = new Services.MruService();
                _SettingsService = Services.SettingsService.Instance;
                _EmailService = new Services.EmailService();
                _JumpListService = new Services.JumpListService();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await RenderPane.OnNavigatedToAsync(parameter, state);
            await FilePane.OnNavigatedToAsync(parameter, state);
            await LearningPane.OnNavigatedToAsync(parameter, state);
            await SamplePane.OnNavigatedToAsync(parameter, state);

            _FutureService.Clear();
            _MruService.Clear(null);
            await _JumpListService.ClearAsync();

            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Subscribe(LoadXamlFile);

            var folders = await _MruService.GetFoldersAsync(null);
            if (folders.Any())
            {
                FilePane.CurrentFolder = folders.LastOrDefault();
            }
            var files = await _MruService.GetFilesAsync();
            if (files.Any())
            {
                LoadXamlFile(files.LastOrDefault());
            }
            else
            {
                RenderPane.Clear();
            }
        }

        async void LoadXamlFile(Models.File file)
        {
            Title = file.Title;
            await RenderPane.FillXamlResultAsync(file);
        }

        string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        public RenderPaneViewModel RenderPane { get; } = new RenderPaneViewModel();
        public FilesPaneViewModel FilePane { get; } = new FilesPaneViewModel();
        public LearningPaneViewModel LearningPane { get; } = new LearningPaneViewModel();
        public SamplesPaneViewModel SamplePane { get; } = new SamplesPaneViewModel();

        public async void EmailCode()
        {
            var attachment = await _EmailService.CreateAttachment(RenderPane.XamlString);
            var body = $"Hi,{Environment.NewLine}My code question...{Environment.NewLine}{Environment.NewLine}{RenderPane.XamlString}";
            await _EmailService.SendAsync("jnixon@microsoft.com", "Just Code XAML question", body, attachment);
        }

        public async void EmailSupport()
        {
            var attachment = await _EmailService.CreateAttachment(RenderPane.XamlString);
            var body = $"Hi,{Environment.NewLine}My support question...";
            await _EmailService.SendAsync("jnixon@microsoft.com", "Just Code XAML question", body);
        }
    }
}

