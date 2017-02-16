

using System;
using Autofac;
using Xamarin.Forms;
using XamFormsReactiveUI.DataLayer;
using XamFormsReactiveUI.DataLayer.Abstract;
using XamFormsReactiveUI.Pages;
using XamFormsReactiveUI.Services.App;
using XamFormsReactiveUI.ViewModels;

namespace XamFormsReactiveUI.Bootstrap
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // viewmodels...
            builder.RegisterType<BeginViewModel>().SingleInstance();
            builder.RegisterType<WordPickViewModel>().SingleInstance();

            // pages...
            builder.RegisterType<BeginPage>().SingleInstance();
            builder.RegisterType<WordPickPage>().SingleInstance();

            // app instance
            builder.RegisterInstance(Application.Current).SingleInstance();
            builder.RegisterType<DialogService>().As<IDialogProvider>().SingleInstance();

            // database
            var database = GetPlatformDependency<ISQLite>();
            builder.RegisterInstance(database).AsImplementedInterfaces();
            builder.RegisterType<AppDatabase>().As<IAppDatabase>();

            builder.RegisterType<WordRepository>().As<IWordRepository>();
            
            builder.RegisterInstance<Func<Page>>(() => Application.Current.MainPage);

            // Current PageProxy
            builder.RegisterType<PageProxy>().As<IPage>().SingleInstance();
            // app instance
            builder.RegisterType<AppDatabase>();
        }


        public static T GetPlatformDependency<T>() where T : class
        {
            var dependency = DependencyService.Get<T>();

            if (dependency == null)
            {
                throw new InvalidOperationException($"Missing '{typeof(T).FullName}' implementation! Implementation is required.");
            }

            return dependency;
        }
    }
}
