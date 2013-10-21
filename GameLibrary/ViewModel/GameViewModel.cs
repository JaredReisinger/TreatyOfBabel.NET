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

namespace GameLibrary.ViewModel
{
    public class GameViewModel : ViewModelBaseEx
    {
        private FileInfo file;

        public GameViewModel(Model.Game game)
        {
            this.PlayCommand = new RelayCommand(this.Play);

            this.Path = game.RelativePath;
            this.file = new FileInfo(game.FullPath);

            this.Title = game.Title ?? "An Interactive Fiction";    // why not file name?
            this.Author = game.Author ?? "Anonymous";

            var whitespace = new Regex(@"[ \t\n\v\r]+", RegexOptions.Compiled | RegexOptions.Multiline);

            // Should use treaty API here...
            var helper = App.TreatyHelper;
            IStoryFileHandler handler;
            if (helper.TryGetHandler(game.FullPath, out handler))
            {
                this.Genre = string.Format("(Unknown {0})", handler.Provider.FormatName);

                using (var metadataStream = handler.GetStoryFileMetadata())
                {

                    if (metadataStream != null)
                    {
                        XDocument metadata = XDocument.Load(metadataStream);
                        XNamespace ns = "http://babel.ifarchive.org/protocol/iFiction/";

                        var lameReader = metadata.CreateReader();
                        XmlNamespaceManager xmlns = new XmlNamespaceManager(lameReader.NameTable);
                        xmlns.AddNamespace("i", ns.NamespaceName);

                        var ident = metadata.XPathSelectElement("/i:ifindex/i:story/i:identification", xmlns);
                        var biblio = metadata.XPathSelectElement("/i:ifindex/i:story/i:bibliographic", xmlns);
                        var contact = metadata.XPathSelectElement("/i:ifindex/i:story/i:contacts", xmlns);

                        if (ident != null)
                        {
                            this.Ifid = this.ValueOrDefault(ident, "i:ifid", xmlns, this.Ifid);
                            this.Format = this.ValueOrDefault(ident, "i:format", xmlns, this.Format);
                            this.Bafn = this.ValueOrDefault(ident, "i:bafn", xmlns, this.Bafn);
                        }

                        if (biblio != null)
                        {
                            this.Title = this.ValueOrDefault(biblio, "i:title", xmlns, this.Title);
                            this.Author = this.ValueOrDefault(biblio, "i:author", xmlns, this.Author);
                            this.Language = this.ValueOrDefault(biblio, "i:language", xmlns, this.Language);
                            this.Headline = this.ValueOrDefault(biblio, "i:headline", xmlns, this.Headline);
                            this.FirstPublished = this.ValueOrDefault(biblio, "i:firstpublished", xmlns, this.FirstPublished);
                            this.Genre = this.ValueOrDefault(biblio, "i:genre", xmlns, this.Genre);
                            this.Group = this.ValueOrDefault(biblio, "i:group", xmlns, this.Group);
                            this.Description = this.ValueOrDefault(biblio, "i:description", xmlns, this.Description);
                            this.Series = this.ValueOrDefault(biblio, "i:series", xmlns, this.Series);
                            this.SeriesNumber = this.ValueOrDefault(biblio, "i:seriesnumber", xmlns, this.SeriesNumber);
                            this.Forgiveness = this.ValueOrDefault(biblio, "i:foregiveness", xmlns, this.Forgiveness);

                            if (!string.IsNullOrEmpty(this.Description))
                            {
                                // only <br/> is supported... all other whitespace should
                                // get normalized to single spaces...
                                this.Description = whitespace.Replace(this.Description, " ").Trim();
                                this.Description = this.Description.Replace("<br/>", "\n");
                            }
                        }

                        if (contact != null)
                        {
                            this.Url = this.ValueOrDefault(contact, "i:url", xmlns, this.Url);
                            this.AuthorEmail = this.ValueOrDefault(contact, "i:authoremail", xmlns, this.AuthorEmail);
                        }
                    }
                }

                this.FullImage = this.ImageFromStream(handler.GetStoryFileCover(), 300, 300);
                this.ThumbImage = this.ImageFromStream(handler.GetStoryFileCover(), 60, 60);

                if (string.IsNullOrEmpty(this.Ifid))
                {
                    ////System.Threading.ThreadPool.QueueUserWorkItem(state =>
                    ////    {
                    ////        var ifid = handler.GetStoryFileIfid();
                    ////        if (string.IsNullOrEmpty(this.Ifid))
                    ////        {
                    ////            this.Ifid = ifid;
                    ////        }
                    ////    });
                    //System.Threading.Tasks.Task.Factory.StartNew(() =>
                    //    {
                    //        var ifid = handler.GetStoryFileIfid();
                    //        if (string.IsNullOrEmpty(this.Ifid))
                    //        {
                    //            this.Ifid = ifid;
                    //        }
                    //    });
                    ////this.Ifid = handler.GetStoryFileIfid();
                }
            }
        }

        public BitmapImage ImageFromStream(Stream stream, int? width = null, int? height = null)
        {
            BitmapImage bmp = null;
            if (stream != null)
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = stream;

                if (width.HasValue)
                {
                    bmp.DecodePixelWidth = width.Value;
                }

                if (height.HasValue)
                {
                    bmp.DecodePixelHeight = height.Value;
                }

                bmp.EndInit();
            }

            return bmp;
        }

        private string ValueOrDefault(XNode node, string xpath, XmlNamespaceManager xmlns, string defaultValue)
        {
            var el = node.XPathSelectElement(xpath, xmlns);
            if (el != null && !string.IsNullOrWhiteSpace(el.Value))
            {
                return el.Value;
            }

            return defaultValue;
        }

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
            Process.Start(this.file.FullName);
        }
    }
}
