using System;
using GameLibrary.Model;

namespace GameLibrary.Design
{
    public class DesignDataService : IDataService
    {
        #region IDataService Members

        public void GetData(Action<DataItem, Exception> callback)
        {
            // use this to create design-time data
            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }

        #endregion
    }
}
