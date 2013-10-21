using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TreatyOfBabel;

namespace GameLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static System.Lazy<TreatyHelper> LazyTreatyHelper = new System.Lazy<TreatyHelper>(() => new TreatyHelper());

        public static TreatyHelper TreatyHelper { get { return LazyTreatyHelper.Value; } }
    }
}
