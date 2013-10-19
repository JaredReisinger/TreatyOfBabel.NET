using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GameLibrary.Model;

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
        private ObservableCollection<GameViewModel> games;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            this.games = new ObservableCollection<GameViewModel>();
            this.Games = new ReadOnlyObservableCollection<GameViewModel>(this.games);

            this.GamesView = new CollectionViewSource();
            this.GamesView.Source = this.Games;
            //this.Sort("Title");
            this.CreateStandardSort(null);
            this.Group("Genre");

            this.SortCommand = new RelayCommand<string>(this.Sort, this.CanSort);
            this.GroupCommand = new RelayCommand<string>(this.Group, this.CanGroup);

            this.dataService = dataService;

            this.PropertyChanged += MainViewModel_PropertyChanged;

            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            this.RootPath = Path.Combine(profile, @"SkyDrive\Documents\Interactive Fiction");
        }

        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "RootPath"))
            {
                this.games.Clear();

                this.dataService.GetGames(this.RootPath)
                    .ObserveOnDispatcher()
                    .Subscribe(this);
            }
        }

        private string rootPath;
        public string RootPath
        {
            get { return this.rootPath; }
            private set { this.Set(ref this.rootPath, value); }
        }

        public ReadOnlyObservableCollection<GameViewModel> Games { get; private set; }
        public CollectionViewSource GamesView { get; private set; }

        public RelayCommand<string> SortCommand { get; private set; }
        public RelayCommand<string> GroupCommand { get; private set; }

        #region IObserver<Game> Members

        public void OnNext(Game game)
        {
            this.games.Add(new GameViewModel(game));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }

        #endregion

        private bool CanSort(string column)
        {
            switch (column)
            {
                case "Title":
                case "Author":
                case "Genre":
                case "Path":
                    return true;
            }

            return false;
        }

        private static readonly string[] StandardSortColumns = { "Title", "Author", "Genre", "Path" };

        private void CreateStandardSort(string columnToSkip)
        {
            var view = this.GamesView.View;

            foreach (var column in StandardSortColumns.Where(c => !string.Equals(c, columnToSkip)))
            {
                view.SortDescriptions.Add(new SortDescription(column, ListSortDirection.Ascending));
            }
        }

        private void Sort(string column)
        {
            var view = this.GamesView.View;

            // "first" depends on whether there's grouping or not...
            var firstNonGroupIndex = view.GroupDescriptions.Count;
            var group = view.GroupDescriptions.FirstOrDefault() as PropertyGroupDescription;
            var sortIsGroup = group == null ? false : string.Equals(group.PropertyName, column);

            var sortIndex = sortIsGroup ? 0 : firstNonGroupIndex;

            var sort = view.SortDescriptions.ElementAtOrDefault(sortIndex);

            var isFirstSort = string.Equals(sort.PropertyName, column);

            var direction = ListSortDirection.Ascending;

            if (!isFirstSort)
            {
                sort = view.SortDescriptions.FirstOrDefault(s => string.Equals(s.PropertyName, column));

                if (!string.IsNullOrEmpty(sort.PropertyName))
                {
                    // remove it so we can add it up front...
                    view.SortDescriptions.Remove(sort);
                }
            }
            else
            {
                // remove it so we can create the opposite and add it up front...
                view.SortDescriptions.Remove(sort);
                direction = (ListSortDirection)(ListSortDirection.Descending - sort.Direction);
            }

            sort = new SortDescription(column, direction);
            view.SortDescriptions.Insert(sortIndex, sort);

            view.Refresh();
        }

        private bool CanGroup(string column)
        {
            switch (column)
            {
                case "Author":
                case "Genre":
                    return true;
            }

            return false;
        }

        private void Group(string column)
        {
            var view = this.GamesView.View;

            view.GroupDescriptions.Clear();
            var group = new PropertyGroupDescription(column);
            view.GroupDescriptions.Add(group);

            // If needed, move the matching sort key to the beginning of
            // the list of sort descriptions...
            var sort = view.SortDescriptions.FirstOrDefault();
            if (!string.Equals(sort.PropertyName, column))
            {
                this.Sort(column);
            }

            view.Refresh();
        }
    }
}