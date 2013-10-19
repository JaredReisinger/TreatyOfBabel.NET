using GalaSoft.MvvmLight;
using GameLibrary.Model;

namespace GameLibrary.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService dataService;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            this.dataService.GetData((item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    this.WelcomeText = item.Title;
                });
        }

        /// <summary>
        /// The <see cref="WelcomeText" /> property's name.
        /// </summary>
        public const string WelcomeTextPropertyName = "WelcomeText";

        private string welcomeText = string.Empty;

        /// <summary>
        /// Sets and gets the WelcomeText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeText
        {
            get
            {
                return welcomeText;
            }

            set
            {
                this.Set(WelcomeTextPropertyName, ref this.welcomeText, value);
            }
        }
    }
}