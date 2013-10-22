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

            if (model.CoverImageStream != null)
            {
                // Ideally, delay creating full cover image until we actually need it!
                this.FullImage = this.ImageFromStream(model.CoverImageStream, 300, 300);
                this.ThumbImage = this.ImageFromStream(model.CoverImageStream, 60, 60);
            }
        }

        // NOTE: property-change notifications may be overkill here, as the data can't
        // be changed.  These could simply be get-only wrappers around the underlying
        // data model, rather than having the overhead of the Set() call...
        private string path;
        public string Path
        {
            get { return this.path; }
            private set { this.Set(ref this.path, value); }
        }

        // identification
        private string ifid;
        public string Ifid
        {
            get { return this.ifid; }
            private set { this.Set(ref this.ifid, value); }
        }

        private string format;
        public string Format
        {
            get { return this.format; }
            private set { this.Set(ref this.format, value); }
        }

        private string bafn;
        public string Bafn
        {
            get { return this.bafn; }
            private set { this.Set(ref this.bafn, value); }
        }

        // bibliographic
        private string title;
        public string Title
        {
            get { return this.title; }
            private set { this.Set(ref this.title, value); }
        }

        private string author;
        public string Author
        {
            get { return this.author; }
            private set { this.Set(ref this.author, value); }
        }

        private string language;
        public string Language
        {
            get { return this.language; }
            private set { this.Set(ref this.language, value); }
        }

        private string headline;
        public string Headline
        {
            get { return this.headline; }
            private set { this.Set(ref this.headline, value); }
        }

        private string firstPublished;
        public string FirstPublished
        {
            get { return this.firstPublished; }
            private set { this.Set(ref this.firstPublished, value); }
        }

        private string genre;
        public string Genre
        {
            get { return this.genre; }
            private set { this.Set(ref this.genre, value); }
        }

        private string group;
        public string Group
        {
            get { return this.group; }
            private set { this.Set(ref this.group, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            private set { this.Set(ref this.description, value); }
        }

        private string series;
        public string Series
        {
            get { return this.series; }
            private set { this.Set(ref this.series, value); }
        }

        private string seriesNumber;
        public string SeriesNumber
        {
            get { return this.seriesNumber; }
            private set { this.Set(ref this.seriesNumber, value); }
        }

        private string forgiveness;
        public string Forgiveness
        {
            get { return this.forgiveness; }
            private set { this.Set(ref this.forgiveness, value); }
        }

        //// resources/auxiliary not used...

        //contact
        private string url;
        public string Url
        {
            get { return this.url; }
            private set { this.Set(ref this.url, value); }
        }

        private string authorEmail;
        public string AuthorEmail
        {
            get { return this.authorEmail; }
            private set { this.Set(ref this.authorEmail, value); }
        }

        // images
        private ImageSource fullImage;
        public ImageSource FullImage
        {
            get { return this.fullImage; }
            private set { this.Set(ref this.fullImage, value); }
        }

        private ImageSource thumbImage;
        public ImageSource ThumbImage
        {
            get { return this.thumbImage; }
            private set { this.Set(ref this.thumbImage, value); }
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
