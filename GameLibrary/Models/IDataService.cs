﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Models
{
    public interface IDataService
    {
        IObservable<GameModel> GetGames(string rootPath);
    }
}
