using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Model
{
    public class DataService : IDataService
    {
        #region IDataService Members

        public IObservable<Game> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                rootPath = Path.Combine(profile, @"SkyDrive\Documents\Interactive Fiction");
            }

            // iterate the root path, looking for games...
            return Observable.Create<Game>((observer, cancel) =>
            {
                var task = Task.Factory.StartNew(() =>
                {
                    var files = Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (cancel.IsCancellationRequested)
                        {
                            break;
                        }

                        if (file.EndsWith("blorb"))
                        {
                            var game = new Game(file, rootPath);
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
