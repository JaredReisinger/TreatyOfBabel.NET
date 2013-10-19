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
        private BlorbReader blorb;

        public GameViewModel(Model.Game game)
        {
            this.Path = game.RelativePath;
            this.file = new FileInfo(game.FullPath);

            this.Title = game.Title;
            this.Author = game.Author;

            this.blorb = new BlorbReader(game.FullPath);

            if (this.blorb != null && this.blorb.Metadata != null)
            {
                XNamespace ns = "http://babel.ifarchive.org/protocol/iFiction/";

                var lameReader = this.blorb.Metadata.CreateReader();
                XmlNamespaceManager xmlns = new XmlNamespaceManager(lameReader.NameTable);
                xmlns.AddNamespace("i", ns.NamespaceName);

                var biblio = this.blorb.Metadata.XPathSelectElement("/i:ifindex/i:story/i:bibliographic", xmlns);

                if (biblio != null)
                {
                    this.Title = this.ValueOrDefault(biblio, "i:title", xmlns, this.Title);
                    this.Headline = this.ValueOrDefault(biblio, "i:headline", xmlns, this.Headline);
                    this.Author = this.ValueOrDefault(biblio, "i:author", xmlns, this.Author);
                    this.Genre = this.ValueOrDefault(biblio, "i:genre", xmlns, this.Genre);
                    this.Description = this.ValueOrDefault(biblio, "i:description", xmlns, this.Description);

                    ////<headline>A Metasemantic Construction</headline>
                    ////<genre>Fiction</genre>
                    ////<firstpublished>2010</firstpublished>
                    ////<description>...</description>
                    ////<language>en</language>
                    ////<group>Inform</group>

                }
            }

            if (this.blorb != null && this.blorb.CoverImage != null)
            {
                this.Image = this.blorb.CoverImage;
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

        private ImageSource image;
        public ImageSource Image
        {
            get { return this.image; }
            private set { this.Set(ref this.image, value); }
        }
    }
}
