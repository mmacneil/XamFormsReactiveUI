 

using Autofac;
using Xamarin.Forms;
using XamFormsReactiveUI.Bootstrap;

namespace XamFormsReactiveUI
{
    public class App : Application
    {
        public static IContainer Container { get; set; }

        public App()
        {
            var bootstrapper = new Bootstrapper(this);
            Container = bootstrapper.Run();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
