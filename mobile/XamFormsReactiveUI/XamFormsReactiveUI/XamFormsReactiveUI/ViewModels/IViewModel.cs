 

using System;

namespace XamFormsReactiveUI.ViewModels
{
    public interface IViewModel
    {
        void SetState<T>(Action<T> action) where T : class, IViewModel;
    }
}
