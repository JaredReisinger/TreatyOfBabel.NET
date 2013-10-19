using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;

namespace GameLibrary.ViewModel
{
    /// <summary>
    /// Extends GalaSoft's ViewModelBase by adding a Set() override that
    /// uses the CallerMemberName attribute to automatically provide the
    /// property name, rather than needing an explicit string or expression.
    /// </summary>
    public class ViewModelBaseEx : ViewModelBase
    {
        public bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            return this.Set(propertyName, ref field, value);
        }
    }
}
