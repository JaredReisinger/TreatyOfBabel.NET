using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TreatyOfBabel;

namespace GameLibrary.ViewModels
{
    public class GameViewModel : ViewModelBaseEx
    {
        private Models.GameModel model;

        public GameViewModel(Models.GameModel model)
        {
            this.model = model;
            this.PlayCommand = new RelayCommand(this.Play);

            // Copy fields from the model...
            this.Path = model.RelativePath;

            this.Ifid = model.Ifid;
            this.Format = model.Format;
            this.Bafn = model.Bafn;
            this.Title = model.Title;
            this.Author = model.Author;
            this.Language = model.Language;
            this.Headline = model.Headline;
            this.FirstPublished = model.FirstPublished;
            this.Genre = model.Genre;
            this.Group = model.Group;
            this.Description = model.Description;
            this.Series = model.Series;
            this.SeriesNumber = model.SeriesNumber;
            this.Forgiveness = model.Forgiveness;
            this.Url = model.Url;
            this.AuthorEmail = model.AuthorEmail;
        }

        public string Path { get; private set; }

        // identification
        public string Ifid { get; private set; }
        public string Format { get; private set; }
        public string Bafn { get; private set; }

        // bibliographic
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Language { get; private set; }
        public string Headline { get; private set; }
        public string FirstPublished { get; private set; }
        public string Genre { get; private set; }
        public string Group { get; private set; }
        public string Description { get; private set; }
        public string Series { get; private set; }
        public string SeriesNumber { get; private set; }
        public string Forgiveness { get; private set; }

        //// resources/auxiliary not used...

        //contact
        public string Url { get; private set; }
        public string AuthorEmail { get; private set; }

        // images
        private ImageSource fullImage;
        public ImageSource FullImage
        {
            get 
            {
                if (this.fullImage == null && this.model.CoverImageStream != null)
                {
                    this.fullImage = this.ImageFromStream(model.CoverImageStream, 300, 300);
                }

                return this.fullImage;
            }
        }

        private ImageSource thumbImage;
        public ImageSource ThumbImage
        {
            get 
            {
                if (this.thumbImage == null && this.model.CoverImageStream != null)
                {
                    this.thumbImage = this.ImageFromStream(model.CoverImageStream, 60, 60);
                }

                return this.thumbImage;
            }
        }

        public RelayCommand PlayCommand { get; private set; }

        private void Play()
        {
            // Launch it!
            Process.Start(this.model.FullPath);
        }

        private BitmapImage ImageFromStream(MemoryStream stream, int width, int height)
        {
            BitmapImage bmp = null;
            if (stream != null)
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(stream.GetBuffer());
                bmp.DecodePixelWidth = width;
                bmp.DecodePixelHeight = height;
                bmp.EndInit();
            }

            return bmp;
        }
    }
}
