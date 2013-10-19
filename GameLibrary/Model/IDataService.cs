using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
        IObservable<Game> GetGames(string rootPath);
    }
}
