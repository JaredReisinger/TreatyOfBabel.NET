using System;
using GameLibrary.Model;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
 
namespace GameLibrary.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService Members

        public IObservable<Game> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                rootPath = Path.Combine(profile, @"SkyDrive\Documents\Interactive Fiction");
            }

            return Observable.Generate(
                0,
                i => i < 4,
                i => ++i,
                i =>
                {
                    switch (i)
                    {
                        case 0:
                            return new Game(Path.Combine(rootPath, @"MiscIFGames\Dual.zblorb"), rootPath);
                        case 1:
                            return new Game(Path.Combine(rootPath, @"MiscIFGames\LostPig.zblorb"), rootPath);
                        case 2:
                            return new Game(Path.Combine(rootPath, @"MiscIFGames\RoTA.zblorb"), rootPath);
                        case 3:
                            return new Game(Path.Combine(rootPath, @"MiscIFGames\Savoir-Faire.zblorb"), rootPath);
                    }

                    return null;
                });
        }

        #endregion
    }
}
