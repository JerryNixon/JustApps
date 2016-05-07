using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Template10.Utils;

namespace JustXaml.ViewModels
{
    public class SamplesPaneViewModel : BindableBase
    {
        Services.FileService _FileService;
        Services.FolderService _FolderService;
        Services.MruService _MruService;
        public async Task OnNavigatedToAsync(object parameter, IDictionary<string, object> state)
        {
            _FileService = Services.FileService.Instance;
            _FolderService = Services.FolderService.Instance;
            _MruService = new Services.MruService();
            await FillUnfilteredSamplesAsync();
        }

        public ObservableCollection<Models.File> RecentSamples { get; } = new ObservableCollection<Models.File>();
        public async Task FillRecentSamplesAsync()
        {
            var files = await _MruService.GetFilesAsync(Models.File.Types.Sample);
            RecentSamples.AddRange(files, true);
        }

        public ObservableCollection<Models.File> FilteredSamples { get; } = new ObservableCollection<Models.File>();
        public void FillFilteredSamples(string filter)
        {
            if (filter?.Length > 0)
            {
                var files = UnfilteredSamples.SelectMany(x => x.Files);
                files = files.Where(x => x.Title.ToLower().Contains(filter.ToLower()));
                FilteredSamples.AddRange(files, true);
            }
            else
            {
                FilteredSamples.Clear();
            }
        }

        public ObservableCollection<Models.Folder> UnfilteredSamples { get; } = new ObservableCollection<Models.Folder>();
        public async Task FillUnfilteredSamplesAsync()
        {
            UnfilteredSamples.Clear();
            var location = Windows.ApplicationModel.Package.Current.InstalledLocation;
            location = await location.GetFolderAsync("Templates");
            var folder = new Models.Folder(location, Models.Folder.Types.Sample);
            var folders = await _FolderService.GetFoldersAsync(folder);
            foreach (var item in folders)
            {
                item.Files = (await _FolderService.GetFilesAsync(item, ".xaml"));
                UnfilteredSamples.Add(item);
            }
        }

        string _FilterText = default(string);
        public string FilterText { get { return _FilterText; } set { Set(ref _FilterText, value); FilterTextPropertyChanged(value); } }
        private void FilterTextPropertyChanged(string filter)
        {
            FillFilteredSamples(filter);
        }

        #region User actions

        public void ClearRecent()
        {
            _MruService.Clear(Models.Folder.Types.Sample);
            RecentSamples.Clear();
        }

        public void RecentItemClicked(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as Models.File;
            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Publish(file);
        }

        public async void FilteredItemClicked(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as Models.File;
            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Publish(file);
            await AddToRecentAsync(file);
        }

        public async void UnfilteredItemClicked(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as Models.File;
            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Publish(file);
            await AddToRecentAsync(file);
        }

        #endregion

        private async Task AddToRecentAsync(Models.File file)
        {
            _MruService.Add(file);
            await FillRecentSamplesAsync();
        }
    }
}