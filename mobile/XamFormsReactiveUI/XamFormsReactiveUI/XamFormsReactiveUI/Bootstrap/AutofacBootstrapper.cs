 
using Autofac;
using XamFormsReactiveUI.Factories;

namespace XamFormsReactiveUI.Bootstrap
{
    public abstract class AutofacBootstrapper
    {
        public IContainer Run()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);
            var container = builder.Build();
            var pageFactory = container.Resolve<IPageFactory>();
            RegisterPages(pageFactory);
            ConfigureApplication(container);
            return container;
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();
        }

        protected abstract void RegisterPages(IPageFactory pageFactory);

        protected abstract void ConfigureApplication(IContainer container);
    }
}
