using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Model
{
    public interface IDataService
    {
        IObservable<Game> GetGames(string rootPath);
    }
}
