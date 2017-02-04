

using System;
using System.Collections.Generic;
using Autofac;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Factories
{
    public class PageFactory : IPageFactory
    {
        public readonly IDictionary<Type, Type> Map = new Dictionary<Type, Type>();
        private readonly IComponentContext _componentContext;

        public PageFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : Page
        {
            Map[typeof(TViewModel)] = typeof(TView);
        }

        public Page Resolve<TViewModel>(Action<TViewModel> setStateAction = null) where TViewModel : class, IViewModel
        {
            TViewModel viewModel;
            return Resolve(out viewModel, setStateAction);
        }

        public Page Resolve<TViewModel>(out TViewModel viewModel, Action<TViewModel> setStateAction = null)
            where TViewModel : class, IViewModel
        {
            viewModel = _componentContext.Resolve<TViewModel>();

            var pageType = Map[typeof(TViewModel)];

            var page = _componentContext.Resolve(pageType) as Page;

            if (setStateAction != null)
                viewModel.SetState(setStateAction);

            var contentPage = page as ReactiveContentPage<TViewModel>;
            if (contentPage != null)
            {
                contentPage.ViewModel = viewModel;
            }
            page.BindingContext = viewModel;

            return page;
        }

        public Page Resolve<TViewModel>(TViewModel viewModel)
            where TViewModel : class, IViewModel
        {
            var pageType = Map[typeof(TViewModel)];
            var page = _componentContext.Resolve(pageType) as Page;
            page.BindingContext = viewModel;
            return page;
        }
    }
}
