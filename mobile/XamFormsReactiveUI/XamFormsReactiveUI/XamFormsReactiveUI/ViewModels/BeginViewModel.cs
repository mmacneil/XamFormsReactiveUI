 
using System.Windows.Input;
using Xamarin.Forms;
using XamFormsReactiveUI.Services.App;

namespace XamFormsReactiveUI.ViewModels
{
    public class BeginViewModel : ViewModel
    {
        private readonly INavigator _navigator;
        public ICommand LaunchCommand { get; }

        public BeginViewModel(INavigator navigator)
        {
            _navigator = navigator;
            LaunchCommand = new Command(async () => await _navigator.PushModalAsync<WordPickViewModel>(null, true));
        }
    }
}
