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
using GameLibrary.Models;

namespace GameLibrary.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
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

            this.CreateStandardSort(null);
            this.Group("Genre");
            this.Sort("Title");
            this.SortAscending();

            this.SortCommand = new RelayCommand<string>(this.Sort, this.CanSort);
            this.GroupCommand = new RelayCommand<string>(this.Group, this.CanGroup);

            this.SortAscendingCommand = new RelayCommand(this.SortAscending, this.CanSortAscending);
            this.SortDescendingCommand = new RelayCommand(this.SortDescending, this.CanSortDescending);

            this.GroupAscendingCommand = new RelayCommand(this.GroupAscending, this.CanGroupAscending);
            this.GroupDescendingCommand = new RelayCommand(this.GroupDescending, this.CanGroupDescending);

            this.dataService = dataService;

            this.PropertyChanged += MainViewModel_PropertyChanged;

            // TODO: Add a persisted setting for the directory to enumerate.
            // TODO: Use the user's document directory by default?
            // For now, we use the "Samples" directory, because we know that
            // we're in a development context. :)
            this.RootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Samples"));
        }

        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "RootPath"))
            {
                this.games.Clear();

                this.dataService.GetGames(this.RootPath)
                    .DelaySubscription(TimeSpan.FromMilliseconds(100)) // a delay of 100ms lets the UI come up quickly
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

        private string currentSort;
        public string CurrentSort
        {
            get { return this.currentSort; }
            private set { this.Set(ref this.currentSort, value); }
        }

        private ListSortDirection currentSortDirection;
        public ListSortDirection CurrentSortDirection
        {
            get { return this.currentSortDirection; }
            private set { this.Set(ref this.currentSortDirection, value); }
        }

        private string currentGroup;
        public string CurrentGroup
        {
            get { return this.currentGroup; }
            private set { this.Set(ref this.currentGroup, value); }
        }

        private ListSortDirection currentGroupDirection;
        public ListSortDirection CurrentGroupDirection
        {
            get { return this.currentGroupDirection; }
            private set { this.Set(ref this.currentGroupDirection, value); }
        }

        public ReadOnlyObservableCollection<GameViewModel> Games { get; private set; }
        public CollectionViewSource GamesView { get; private set; }

        public RelayCommand<string> SortCommand { get; private set; }
        public RelayCommand<string> GroupCommand { get; private set; }

        public RelayCommand SortAscendingCommand { get; private set; }
        public RelayCommand SortDescendingCommand { get; private set; }

        public RelayCommand GroupAscendingCommand { get; private set; }
        public RelayCommand GroupDescendingCommand { get; private set; }

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

            if (!sortIsGroup)
            {
                this.CurrentSort = column;
                this.CurrentSortDirection = direction;
            }
            else
            {
                this.CurrentGroupDirection = direction;
            }

            view.Refresh();
        }

        private bool CanGroup(string column)
        {
            switch (column)
            {
                case "Author":
                case "Genre":
                case null:
                    return true;
            }

            return false;
        }

        private void Group(string column)
        {
            var view = this.GamesView.View;

            view.GroupDescriptions.Clear();

            if (!string.IsNullOrWhiteSpace(column))
            {
                var group = new PropertyGroupDescription(column);
                view.GroupDescriptions.Add(group);

                // If needed, move the matching sort key to the beginning of
                // the list of sort descriptions...
                var sort = view.SortDescriptions.FirstOrDefault();
                if (!string.Equals(sort.PropertyName, column))
                {
                    this.Sort(column);
                }
            }

            this.CurrentGroup = column;
            view.Refresh();
        }

        private bool CanSortAscending()
        {
            return this.CurrentSort != null;
        }

        private void SortAscending()
        {
            if (this.CurrentSortDirection != ListSortDirection.Ascending)
            {
                this.Sort(this.CurrentSort);
            }
        }

        private bool CanSortDescending()
        {
            return this.CurrentSort != null;
        }

        private void SortDescending()
        {
            if (this.CurrentSortDirection != ListSortDirection.Descending)
            {
                this.Sort(this.CurrentSort);
            }
        }

        private bool CanGroupAscending()
        {
            return this.CurrentGroup != null;
        }

        private void GroupAscending()
        {
            if (this.CurrentGroupDirection != ListSortDirection.Ascending)
            {
                this.Sort(this.CurrentGroup);
            }
        }

        private bool CanGroupDescending()
        {
            return this.CurrentGroup != null;
        }

        private void GroupDescending()
        {
            if (this.CurrentGroupDirection != ListSortDirection.Descending)
            {
                this.Sort(this.CurrentGroup);
            }
        }
    }
}