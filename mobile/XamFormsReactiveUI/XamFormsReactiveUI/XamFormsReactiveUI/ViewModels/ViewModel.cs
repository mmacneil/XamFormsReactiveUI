using System;
 
using ReactiveUI;

namespace XamFormsReactiveUI.ViewModels
{
    public abstract class ViewModel : ReactiveObject, IViewModel
    {
        public void SetState<T>(Action<T> action) where T : class, IViewModel
        {
            action(this as T);
        }
    }
}
