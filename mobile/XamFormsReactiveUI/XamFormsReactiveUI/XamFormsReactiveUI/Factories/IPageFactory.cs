

using System;
using Xamarin.Forms;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Factories
{
    public interface IPageFactory
    {
        void Register<TViewModel, TPage>()
            where TViewModel : class, IViewModel
            where TPage : Page;

        Page Resolve<TViewModel>(Action<TViewModel> setStateAction = null)
            where TViewModel : class, IViewModel;

        Page Resolve<TViewModel>(out TViewModel viewModel, Action<TViewModel> setStateAction = null)
            where TViewModel : class, IViewModel;

        Page Resolve<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel;
    }
}
