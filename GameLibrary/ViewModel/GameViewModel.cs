using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using GalaSoft.MvvmLight;
using TreatyOfBabel;

namespace GameLibrary.ViewModel
{
    public class GameViewModel : ViewModelBaseEx
    {
        private FileInfo file;

        public GameViewModel(Model.Game game)
        {
            this.Path = game.RelativePath;
            this.file = new FileInfo(game.FullPath);

            this.Title = game.Title;
            this.Author = game.Author;

            using (var blorb = new BlorbReader(game.FullPath))
            {
                if (blorb != null && blorb.Metadata != null)
                {
                    XNamespace ns = "http://babel.ifarchive.org/protocol/iFiction/";

                    var lameReader = blorb.Metadata.CreateReader();
                    XmlNamespaceManager xmlns = new XmlNamespaceManager(lameReader.NameTable);
                    xmlns.AddNamespace("i", ns.NamespaceName);

                    var biblio = blorb.Metadata.XPathSelectElement("/i:ifindex/i:story/i:bibliographic", xmlns);

                    if (biblio != null)
                    {
                        this.Title = this.ValueOrDefault(biblio, "i:title", xmlns, this.Title);
                        this.Headline = this.ValueOrDefault(biblio, "i:headline", xmlns, this.Headline);
                        this.Author = this.ValueOrDefault(biblio, "i:author", xmlns, this.Author);
                        this.Genre = this.ValueOrDefault(biblio, "i:genre", xmlns, this.Genre);
                        this.Description = this.ValueOrDefault(biblio, "i:description", xmlns, this.Description);

                        ////<firstpublished>2010</firstpublished>
                        ////<language>en</language>
                        ////<group>Inform</group>
                    }
                }

                if (blorb != null)
                {
                    this.FullImage = blorb.GetCoverImage(300, 300);
                    this.ThumbImage = blorb.GetCoverImage(60, 60);
                }
            }
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

        private string title;
        public string Title
        {
            get { return this.title; }
            private set { this.Set(ref this.title, value); }
        }

        private string headline;
        public string Headline
        {
            get { return this.headline; }
            private set { this.Set(ref this.headline, value); }
        }

        private string author;
        public string Author
        {
            get { return this.author; }
            private set { this.Set(ref this.author, value); }
        }

        private string genre;
        public string Genre
        {
            get { return this.genre; }
            private set { this.Set(ref this.genre, value); }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            private set { this.Set(ref this.description, value); }
        }

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
    }
}
