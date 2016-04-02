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

namespace JustXaml.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Render.Clear();
            await FilePane.OnNavigatedToAsync(parameter, state);
            await LearningPane.OnNavigatedToAsync(parameter, state);
            await SamplePane.OnNavigatedToAsync(parameter, state);
        }

        public Services.RenderService Render { get; } = new Services.RenderService();
        public FilesPaneViewModel FilePane { get; } = new FilesPaneViewModel();
        public LearningPaneViewModel LearningPane { get; } = new LearningPaneViewModel();
        public SamplesPaneViewModel SamplePane { get; } = new SamplesPaneViewModel();
    }
}

