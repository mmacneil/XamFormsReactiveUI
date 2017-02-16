 
using System.Windows.Input;
using Xamarin.Forms;

namespace XamFormsReactiveUI.Views
{
    public class CommandableListView : ListView
    {
        public static readonly BindableProperty ItemClickCommandProperty = BindableProperty.Create("ItemClickCommand", typeof(ICommand), typeof(CommandableListView));

        public CommandableListView()
        {
            ItemTapped += OnItemTapped;
        }


        public ICommand ItemClickCommand
        {
            get { return (ICommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null || ItemClickCommand == null || !ItemClickCommand.CanExecute(e)) return;

            ItemClickCommand.Execute(e.Item);
            SelectedItem = null;
        }
    }
}
