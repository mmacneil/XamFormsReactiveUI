
using System;
using System.Threading.Tasks;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Services.App
{
    public interface INavigator
    {
        Task<IViewModel> PopAsync();

        Task<IViewModel> PopModalAsync(bool animated = false);

        Task PopToRootAsync();

        Task<TViewModel> PushAsync<TViewModel>(Action<TViewModel> setStateAction = null)
            where TViewModel : class, IViewModel;

        Task<TViewModel> PushAsync<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel;

        Task<TViewModel> PushModalAsync<TViewModel>(Action<TViewModel> setStateAction = null, bool asNavigationPage = false)
            where TViewModel : class, IViewModel;

        Task<TViewModel> PushModalAsync<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel;
    }
}
