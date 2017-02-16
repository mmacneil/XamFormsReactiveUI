 

using System.Reactive.Disposables;
using ReactiveUI;
using Xamarin.Forms;
using XamFormsReactiveUI.Extensions;
using XamFormsReactiveUI.ValueConverters;
using XamFormsReactiveUI.Views;


namespace XamFormsReactiveUI.Pages
{
   
    public partial class WordPickPage
    {
        private readonly ContentView _wordChallengeView;
        
        private readonly CommandableListView _wordAnswerItems;
        private readonly Label _challengeWord;

        private readonly Button _nextWord;
        private readonly Badge _countdownBadge;
        private readonly Badge _wordCountBadge;
        private readonly Badge _pctCorrectBadge;

        private readonly CompositeDisposable _bindingsDisposable = new CompositeDisposable();

        public WordPickPage()
        {
            Grid scoreGrid;
            InitializeComponent();
            BackgroundColor = Color.White;

           Title = "Word Up! - XF/RxUI demo";

			Content = new StackLayout
			{
                Padding = new Thickness(0,60,0,20),
                Spacing = 16,
                Children = {

                    (_wordChallengeView = new ContentView
                    {
                        Content = new StackLayout
                        {
                           Children =
                           {
                                (_challengeWord = new Label
                                {
                                   FontFamily = Device.OnPlatform(null,"Helvetica", null),
                                   FontSize = 74,
                                   TextColor = Color.Black,
                                   HorizontalTextAlignment = TextAlignment.Center
                                 }),
                                 (_wordAnswerItems = new CommandableListView {
                                    HeightRequest = 280,
                                    HasUnevenRows = true,
                                    ItemTemplate = new DataTemplate(typeof(WordAnswerItemCell))}),
                                 (_nextWord = new Button
                                 {
                                         Text = "New Word",FontSize = 22
                                     }),
                                 (scoreGrid = new Grid
                                    {
                                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                                        ColumnDefinitions = {
                                         new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                         new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                         new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                                        }
                                    })
                           }
                        }
                    })
				}
			};

            _countdownBadge = new Badge(46, 36) { BoxColor = Color.Orange};
            _wordCountBadge = new Badge(46, 36) { BoxColor = Color.Blue };
            _pctCorrectBadge = new Badge(46, 36) { BoxColor = Color.Green };
            
            scoreGrid.Children.Add(new Label { FontSize = 26,Text = "Timer",  HorizontalTextAlignment = TextAlignment.Center },0, 0);
            scoreGrid.Children.Add(new Label { FontSize = 26, Text = "Answered", HorizontalTextAlignment = TextAlignment.Center },1, 0);
            scoreGrid.Children.Add(new Label { FontSize = 26, Text = "% Correct",  HorizontalTextAlignment = TextAlignment.Center }, 2, 0);
            scoreGrid.Children.Add(_countdownBadge,0,1);
            scoreGrid.Children.Add(_wordCountBadge, 1, 1);
            scoreGrid.Children.Add(_pctCorrectBadge, 2, 1);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //This is a one-way bind
            this.OneWayBind(ViewModel, x => x.WordOptions, c => c._wordAnswerItems.ItemsSource).DisposeWith(_bindingsDisposable);

            //This is a one-way bind
            this.OneWayBind(ViewModel, x => x.ChallengeWord, c => c._challengeWord.Text).DisposeWith(_bindingsDisposable);

            //This is a one-way bind
            this.OneWayBind(ViewModel, x => x.TimerCountdown, c => c._countdownBadge.Text).DisposeWith(_bindingsDisposable);

            //This is a one-way bind
            this.OneWayBind(ViewModel, x => x.WordCount, c => c._wordCountBadge.Text).DisposeWith(_bindingsDisposable);

            //This is a one-way bind
            this.OneWayBind(ViewModel, x => x.CorrectPct, c => c._pctCorrectBadge.Text).DisposeWith(_bindingsDisposable);

            //This is a command binding
            this.BindCommand(ViewModel, x => x.RetrieveWordCommand, c => c._nextWord).DisposeWith(_bindingsDisposable);

            _wordAnswerItems.ItemClickCommand = ViewModel.SelectAnswerCommand;
            ViewModel.Init();
        }

     

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //TODO: RxSUI - Item 3 - We disposae of all of our bindings and any other subscriptions
            _bindingsDisposable.Clear();
        }
    }

    public class WordAnswerItemCell : ViewCell
    {
        public WordAnswerItemCell()
        {
            var grid = new Grid
            {
                Padding = new Thickness(16),
                ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(25, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    }
            };

            var icon = new Image();
            icon.SetBinding(Image.SourceProperty, "Image");
            icon.SetBinding(VisualElement.IsVisibleProperty, "Answered");

            // hide at first
            icon.SetBinding(VisualElement.IsVisibleProperty, new Binding("Answered", BindingMode.OneWay,new CheckVisibleValueConverter()));
            grid.Children.Add(icon, 0, 0);

            var displayText = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                FontSize = 18
            };
            displayText.SetBinding(Label.TextProperty, "Definition");
            grid.Children.Add(displayText,1, 0);
            View = grid;
        }
    }
}
