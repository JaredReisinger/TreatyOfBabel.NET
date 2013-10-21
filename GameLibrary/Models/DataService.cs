using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class DataService : IDataService
    {
        #region IDataService Members

        public IObservable<GameModel> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                return Observable.Empty<GameModel>();
            }

            // iterate the path, looking for games...
            return Observable.Create<GameModel>((observer, cancel) =>
            {
                var task = Task.Factory.StartNew(() =>
                {
                    // Use the Treaty of Babel helper to understand the files...
                    var helper = App.TreatyHelper;
                    var files = Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (cancel.IsCancellationRequested)
                        {
                            break;
                        }

                        if (helper.IsTreatyFile(file))
                        {
                            var game = new GameModel(file, rootPath);
                            observer.OnNext(game);
                        }
                    }

                    observer.OnCompleted();
                },
                cancel);

                return task;
            });
        }

        #endregion
    }
}
