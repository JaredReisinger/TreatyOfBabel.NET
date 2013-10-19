using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Model
{
    public class DataService : IDataService
    {
        #region IDataService Members

        public void GetData(Action<DataItem, Exception> callback)
        {
            // use this to create the actual data
            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        #endregion
    }
}
