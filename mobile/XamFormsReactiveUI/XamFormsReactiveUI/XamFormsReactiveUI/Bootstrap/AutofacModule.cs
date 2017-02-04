 
using Autofac;
using Xamarin.Forms;
using XamFormsReactiveUI.Factories;
using XamFormsReactiveUI.Services.App;

namespace XamFormsReactiveUI.Bootstrap
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // service registration
            builder.RegisterType<PageFactory>().As<IPageFactory>().SingleInstance();
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();

            // navigation registration
            builder.Register(context => Application.Current.MainPage.Navigation).SingleInstance();
        }
    }
}
