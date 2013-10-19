using GalaSoft.MvvmLight;
using GameLibrary.Model;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System;

namespace GameLibrary.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBaseEx, IObserver<Game>
    {
        private readonly IDataService dataService;
        private ObservableCollection<Game> games;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            this.games = new ObservableCollection<Game>();
            this.Games = new ReadOnlyObservableCollection<Game>(this.games);

            this.RootPath = "???";  // Get root path from service?

            this.dataService.GetGames(null)
                .ObserveOnDispatcher()
                .Subscribe(this);
        }

        private string rootPath;
        public string RootPath
        {
            get { return this.rootPath; }
            private set { this.Set(ref this.rootPath, value); }
        }

        public ReadOnlyObservableCollection<Game> Games { get; private set; }

        #region IObserver<Game> Members

        public void OnNext(Game game)
        {
            this.games.Add(game);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}