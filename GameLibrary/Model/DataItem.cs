using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Model
{
    public class DataItem
    {
        public DataItem(string title)
        {
            this.Title = title;
        }

        public string Title { get; private set; }
    }
}
