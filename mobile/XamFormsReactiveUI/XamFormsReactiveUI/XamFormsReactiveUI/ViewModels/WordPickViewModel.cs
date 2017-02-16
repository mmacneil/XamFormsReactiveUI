

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using XamFormsReactiveUI.DataLayer.Abstract;

namespace XamFormsReactiveUI.ViewModels
{
    public class WordOption : ReactiveObject
    {
        private string _word;
        public string Word
        {
            get { return _word; }
            set { this.RaiseAndSetIfChanged(ref _word, value); }
        }

        private string _definition;
        public string Definition
        {
            get { return _definition; }
            set { this.RaiseAndSetIfChanged(ref _definition, value); }
        }

        private bool _isAnswer;
        public bool IsAnswer
        {
            get { return _isAnswer; }
            set { this.RaiseAndSetIfChanged(ref _isAnswer, value); }
        }

        private bool _answered;
        public bool Answered
        {
            get { return _answered; }
            set { this.RaiseAndSetIfChanged(ref _answered, value); }
        }

        private int _wordId;
        public int WordId
        {
            get { return _wordId; }
            set { this.RaiseAndSetIfChanged(ref _wordId, value); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { this.RaiseAndSetIfChanged(ref _image, value); }
        }
    }


    public class WordPickViewModel : ViewModel
    {
        private int _correct;
        private const int RangeLength = 10;
        private readonly IWordRepository _wordRepository;

        private CancellationTokenSource _timerCancellationToken;

        public ReactiveCommand<Unit> SelectAnswerCommand { get; protected set; }

        private int _rangeFloor;
        
        private int _timerCountdown;
        public int TimerCountdown
        {
            get { return _timerCountdown; }
            set { this.RaiseAndSetIfChanged(ref _timerCountdown, value); }
        }

        private string _correctPct;
        public string CorrectPct
        {
            get { return _correctPct; }
            set { this.RaiseAndSetIfChanged(ref _correctPct, value); }
        }

        private int _wordCount;
        public int WordCount
        {
            get { return _wordCount; }
            set { this.RaiseAndSetIfChanged(ref _wordCount, value); }
        }

        private int _rangeCeiling;
        public int RangeCeiling
        {
            get { return _rangeCeiling; }
            set { this.RaiseAndSetIfChanged(ref _rangeCeiling, value); }
        }

        private string _challengeWord;
        public string ChallengeWord
        {
            get { return _challengeWord; }
            set { this.RaiseAndSetIfChanged(ref _challengeWord, value); }
        }

        private ObservableCollection<WordOption> _wordOptions;
        public ObservableCollection<WordOption> WordOptions
        {
            get { return _wordOptions; }
            private set { this.RaiseAndSetIfChanged(ref _wordOptions, value); }
        }

        private ReactiveCommand<List<WordOption>> _retrieveWordCommand;
        public ReactiveCommand<List<WordOption>> RetrieveWordCommand
        {
            get { return _retrieveWordCommand; }
            private set { this.RaiseAndSetIfChanged(ref _retrieveWordCommand, value); }
        }

        private bool _canRetrieve;
        public bool CanRetrieve
        {
            get { return _canRetrieve; }
            set { this.RaiseAndSetIfChanged(ref _canRetrieve, value); }
        }

        private bool _begin;
        public bool Begin
        {
            get { return _begin; }
            set { this.RaiseAndSetIfChanged(ref _begin, value); }
        }

        public WordPickViewModel(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
            WordOptions = new ObservableCollection<WordOption>();
            CorrectPct = "0%";

            var canRetrieve = this.WhenAnyValue(x => x.CanRetrieve).Select(x => x);
            var canSelect = this.WhenAnyValue(x => x.CanRetrieve).Select(x => !x);

            RetrieveWordCommand = ReactiveCommand.CreateAsyncTask(canRetrieve, async arg =>
            {
                var wordResults = await _wordRepository.GetWords(_rangeFloor,RangeCeiling);

                return wordResults.Select(wr =>
                    new WordOption
                    {
                        Word = wr.Name,
                        Definition = wr.Definition,
                        WordId = wr.Id
                    }).ToList();
            });

            SelectAnswerCommand = ReactiveCommand.CreateAsyncTask(canSelect,async arg =>
            {
                await HandleItemSelectedAsync(arg);
            });

             RetrieveWordCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(wordOptions =>
                 {
                     _timerCancellationToken = new CancellationTokenSource();
                     NextRange();
                     CanRetrieve = false;
                     WordOptions.Clear();

                // randomly determine the word to challenge user with
                var rand = new Random();
                var challengeWord = wordOptions[rand.Next(wordOptions.Count)];
                     ChallengeWord = $"\"{challengeWord.Word}\"";

                foreach (var item in wordOptions)
                {
                    var isAnswer = item.WordId == challengeWord.WordId;
                    item.IsAnswer = isAnswer;
                    item.Image = isAnswer ? "check.png" : "x.png";
                    WordOptions.Add(item);
                }

                TimerCountdown = 10;
                Device.StartTimer(new TimeSpan(0, 0, 1), () =>
                {
                    if (_timerCancellationToken.IsCancellationRequested)
                    { 
                        return false;
                    }
                    if (TimerCountdown == 0)
                    {
                        ProcessAnswer();
                        return false;
                    }
                    TimerCountdown--;
                    return true;
                });
            });

            //Behaviors
            this.WhenAnyValue(x => x.Begin).InvokeCommand(RetrieveWordCommand);
        }

        public async Task HandleItemSelectedAsync(object parameter)
        {
            _timerCancellationToken.Cancel();
            var answer = (WordOption)parameter;
            await Task.Delay(1);
            ProcessAnswer(answer.IsAnswer);
        }

        private void ProcessAnswer(bool isAnswer=false)
        {
            WordCount++;

            if (isAnswer)
            {
                _correct++;
            }
            var pctCorrect = Math.Round(_correct /(float)WordCount * 100, 0);

            CorrectPct = $"{pctCorrect}%";

            foreach (var i in WordOptions)
            {
                i.Answered = true;
            }

            CanRetrieve = true;
        }

        public void Init()
        {
            NextRange();
            Begin = true;
            CanRetrieve = true;
        }

        private void NextRange()
        {
            RangeCeiling += RangeLength;
            _rangeFloor = RangeCeiling - 9;
        }
    }
}
