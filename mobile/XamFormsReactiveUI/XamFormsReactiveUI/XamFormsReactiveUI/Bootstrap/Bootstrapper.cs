 
using Autofac;
using XamFormsReactiveUI.Factories;
using XamFormsReactiveUI.Pages;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Bootstrap
{
    public class Bootstrapper : AutofacBootstrapper
    {
        private readonly App _application;

        public Bootstrapper(App application)
        {
            _application = application;
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.RegisterModule<AppModule>();
        }

        protected override void RegisterPages(IPageFactory pageFactory)
        {
            pageFactory.Register<BeginViewModel, BeginPage>();
            pageFactory.Register<WordPickViewModel, WordPickPage>();
        }

        protected override void ConfigureApplication(IContainer container)
        {
            var pageFactory = container.Resolve<IPageFactory>();
            _application.MainPage = pageFactory.Resolve<BeginViewModel>();
        }
    }
}
