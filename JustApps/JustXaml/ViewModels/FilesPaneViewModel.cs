using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using JustXaml.Models;
using Template10.Utils;

namespace JustXaml.ViewModels
{
    public class FilesPaneViewModel : BindableBase
    {
        Services.FolderService _FolderService;
        Services.MruService _MruService;
        Services.FutureService _FutureService;

        internal async Task OnNavigatedToAsync(object parameter, IDictionary<string, object> state)
        {
            _FolderService = Services.FolderService.Instance;
            _FutureService = new Services.FutureService();
            _MruService = new Services.MruService();
            await Task.CompletedTask;
        }

        Models.Folder _CurrentFolder = default(Models.Folder);
        public Models.Folder CurrentFolder { get { return _CurrentFolder; } set { Set(ref _CurrentFolder, value); CurrentFolderPropertyChanged(value); } }
        private async void CurrentFolderPropertyChanged(Models.Folder folder)
        {
            if (folder == null)
                return;
            _MruService.Add(folder);
            _FutureService.Add(folder);
            await FillFolderFilesAsync(folder);
        }

        public ObservableCollection<Models.File> RecentFiles { get; } = new ObservableCollection<Models.File>();
        async Task FillRecentFilesAsync()
        {
            var files = await _MruService.GetFilesAsync(File.Types.File);
            RecentFiles.AddRange(files, true);
        }

        public ObservableCollection<Models.File> FolderFiles { get; } = new ObservableCollection<Models.File>();
        async Task FillFolderFilesAsync(Folder folder)
        {
            var files = await _FolderService.GetFilesAsync(folder);
            FolderFiles.AddRange(files, true);
        }

        #region user actions

        public async void PickFolder()
        {
            var folder = await _FolderService.PickFolderAsync();
            if (folder != null)
            {
                FolderFiles.Clear();
                CurrentFolder = folder;
            }
        }

        public void ClearRecent()
        {
            RecentFiles.Clear();
        }

        public void RefreshFolderFiles()
        {
            CurrentFolder = CurrentFolder;
        }

        public void RecentFilesItemClicked(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as Models.File;
            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Publish(file);
        }

        public async void FolderFilesItemClicked(object sender, ItemClickEventArgs e)
        {
            var file = e.ClickedItem as Models.File;
            Messages.Messenger.Instance.GetEvent<Messages.LoadXamlFile>().Publish(file);
            _MruService.Add(file);
            await FillRecentFilesAsync();
        }

        #endregion
    }
}