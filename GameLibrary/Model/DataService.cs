using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace GameLibrary.Model
{
    public class DataService : IDataService
    {
        #region IDataService Members

        public void GetData(Action<DataItem, Exception> callback)
        {
            // use this to create the actual data
            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var gameFolder = Path.Combine(profile, @"SkyDrive\Documents\Interactive Fiction");  // TODO: don't hard-code path!
            var item = new DataItem(gameFolder);

            // iterate and find items...

            foreach (var file in Directory.EnumerateFiles(gameFolder, "*.*blorb", SearchOption.AllDirectories))
            {
                item.AddGame(new Game(file, gameFolder));
            }

            callback(item, null);
        }

        public IObservable<Game> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                rootPath = Path.Combine(profile, @"SkyDrive\Documents\Interactive Fiction");
            }

            // iterate the root path, looking for games...
            return Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories)
                .ToObservable(ThreadPoolScheduler.Instance)
                .SelectMany(s =>
                    {
                        // SelectMany doesn't seem to pass items along
                        // until everything is done... need to find another
                        // way to do this...
                        //Thread.Sleep(1000);
                        if (s.EndsWith("blorb"))
                        {
                            var gameList = new List<Game>();
                            gameList.Add(new Game(s, rootPath));
                            return gameList.AsEnumerable();
                            //return new Game(s, rootPath);
                        }

                        return Enumerable.Empty<Game>();
                        //return null;
                    });
        }

        #endregion
    }
}
