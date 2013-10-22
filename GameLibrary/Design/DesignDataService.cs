using System;
using GameLibrary.Models;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameLibrary.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService Members

        public IObservable<GameModel> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Samples"));
                ////rootPath = Path.Combine(
                ////                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ////                @"Documents\GitHub\TreatyOfBabel.NET\Samples");
            }

            var games = new List<GameModel>();

            // Use the Treaty of Babel helper to understand the files...
            var helper = App.TreatyHelper;

            var files = Directory.EnumerateFiles(rootPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files.Take(10))
            {
                if (helper.IsTreatyFile(file))
                {
                    var game = new GameModel(file, rootPath);
                    games.Add(game);
                    ////if (games.Count >= 1)
                    ////{
                    ////    break;
                    ////}
                }
            }

            return games.ToObservable();
        }

        #endregion
    }
}
