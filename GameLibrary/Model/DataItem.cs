using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Model
{
    public class DataItem
    {
        public DataItem(string rootPath)
        {
            this.RootPath = rootPath;
            this.Games = new List<Game>();
        }

        public void AddGame(Game game)
        {
            this.Games.Add(game);
        }

        public string RootPath { get; private set; }
        public IList<Game> Games { get; private set; }
    }
}
