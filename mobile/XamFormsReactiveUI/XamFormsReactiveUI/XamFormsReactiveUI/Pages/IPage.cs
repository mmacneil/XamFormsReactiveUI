using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamFormsReactiveUI.Services.App;

namespace XamFormsReactiveUI.Pages
{
    public interface IPage : IDialogProvider
    {
        INavigation Navigation { get; }
    }
}
