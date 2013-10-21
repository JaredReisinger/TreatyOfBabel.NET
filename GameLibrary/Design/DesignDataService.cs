using System;
using GameLibrary.Models;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GameLibrary.Design
{
    // REVIEW: Better to use Blend-style DesignData rather than including code
    // that exists purely for design-time support?
    public class DesignDataService : IDataService
    {
        #region IDataService Members

        public IObservable<Game> GetGames(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Samples"));
            }

            return Directory.EnumerateFiles(rootPath).Select(file => new Game(file, rootPath)).ToObservable();
        }

        #endregion
    }
}
