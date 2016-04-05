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
                _MruService.Clear();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await RenderPane.OnNavigatedToAsync(parameter, state);
            await FilePane.OnNavigatedToAsync(parameter, state);
            await LearningPane.OnNavigatedToAsync(parameter, state);
            await SamplePane.OnNavigatedToAsync(parameter, state);

            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Subscribe(LoadXamlFile);

            if (_SettingsService.LoadPreviousFolder)
            {
                var previous = await FilePane.GetLastFolderAsync();
                if (previous != null)
                {
                    FilePane.CurrentFolder = previous;
                }
            }

            if (_SettingsService.LoadPreviousFile)
            {
                var previous = await FilePane.GetLastFileAsync();
                if (previous == null)
                {
                    await RenderPane.ClearAsync();
                }
                else
                {
                    LoadXamlFile(previous);
                }
            }
        }

        async void LoadXamlFile(Models.File file)
        {
            Title = file.Title;
            await RenderPane.RenderAsync(file);
        }

        string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        public RenderPaneViewModel RenderPane { get; } = new RenderPaneViewModel();
        public FilesPaneViewModel FilePane { get; } = new FilesPaneViewModel();
        public LearningPaneViewModel LearningPane { get; } = new LearningPaneViewModel();
        public SamplesPaneViewModel SamplePane { get; } = new SamplesPaneViewModel();
    }
}

