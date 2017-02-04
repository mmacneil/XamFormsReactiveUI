 
using XamFormsReactiveUI.DataLayer.Abstract;
using XamFormsReactiveUI.Factories;

namespace XamFormsReactiveUI.ViewModels
{
    public class BeginViewModel : ViewModel
    {
        private readonly IPageFactory _pageFactory;
        private readonly IWordRepository _wordRepository;
        
        public BeginViewModel(IPageFactory pageFactory, IWordRepository wordRepository)
        {
            _pageFactory = pageFactory;
            _wordRepository = wordRepository;
        }
    }
}
