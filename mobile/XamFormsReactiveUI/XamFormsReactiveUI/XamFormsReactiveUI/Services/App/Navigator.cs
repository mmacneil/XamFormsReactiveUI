using System;
 
using System.Threading.Tasks;
using Xamarin.Forms;
using XamFormsReactiveUI.Factories;
using XamFormsReactiveUI.Pages;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Services.App
{
    public class Navigator : INavigator
    {
        private readonly IPageFactory _pageFactory;
        private readonly IPage _page;

        public Navigator(IPageFactory pageFactory, IPage page)
        {
            _pageFactory = pageFactory;
            _page = page;
        }

        public async Task<IViewModel> PopAsync()
        {
            var page = await _page.Navigation.PopAsync();
            return page.BindingContext as IViewModel;
        }

        public async Task<IViewModel> PopModalAsync(bool animated = false)
        {
            var page = await _page.Navigation.PopModalAsync(animated);
            return page.BindingContext as IViewModel;
        }

        public async Task PopToRootAsync()
        {
            await _page.Navigation.PopToRootAsync();
        }

        public async Task<TViewModel> PushAsync<TViewModel>(Action<TViewModel> setStateAction = null) where TViewModel : class, IViewModel
        {
            TViewModel viewModel;
            var page = _pageFactory.Resolve(out viewModel, setStateAction);
            await _page.Navigation.PushAsync(page);
            return viewModel;
        }

        public async Task<TViewModel> PushAsync<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel
        {
            var page = _pageFactory.Resolve(viewModel);
            await _page.Navigation.PushAsync(page);
            return viewModel;
        }

        public async Task<TViewModel> PushModalAsync<TViewModel>(Action<TViewModel> setStateAction = null, bool asNavigationPage = false)
            where TViewModel : class, IViewModel
        {
            TViewModel viewModel;
            var page = _pageFactory.Resolve(out viewModel, setStateAction);
            await _page.Navigation.PushModalAsync(asNavigationPage ? new NavigationPage(page) : page);
            return viewModel;
        }

        public async Task<TViewModel> PushModalAsync<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel
        {
            var page = _pageFactory.Resolve(viewModel);
            await _page.Navigation.PushModalAsync(page);
            return viewModel;
        }
    }
}
