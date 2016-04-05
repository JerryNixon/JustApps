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

namespace JustXaml.ViewModels
{
    public class LearningPaneViewModel : BindableBase
    {
        internal async Task OnNavigatedToAsync(object parameter, IDictionary<string, object> state)
        {
            await Task.CompletedTask;
        }
    }
}