using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;

namespace TreatyOfBabel.TreatyProviders
{
    [Export(typeof(ITreatyProvider))]
    ////[ExportMetadata("TreatyOfBabelPopularity", 2000)]
    internal class Blorb : ITreatyProvider
    {
        public string FormatName { get { return "blorb"; } }
        public string HomePage { get { return "http://eblong.com/zarf/blorb"; } }
        public string[] FileExtensions { get { return new string[] { ".blorb", ".blb", ".zblorb", ".zlb", ".gblorb", ".glb" }; } }
        public uint Popularity { get { return 2000; } }

        public bool ClaimStoryFile(IStoryFile storyFile)
        {
            uint indexTemp;
            if (storyFile.Extent < 16 ||
                !storyFile.TryIndexOf("FORM", 0, 4, out indexTemp) ||
                !storyFile.TryIndexOf("IFRS", 8, 8 + 4, out indexTemp))
            {
                return false;
            }

            return true;
        }

        public IStoryFileHandler GetStoryFileHandler(IStoryFile storyFile)
        {
            return new BlorbHandler(this, storyFile);
        }

        private class BlorbHandler : TreatyStoryFileHandlerBase
        {
            public BlorbHandler(ITreatyProvider provider, IStoryFile storyFile)
                : base(provider, storyFile)
            {
                this.reader = new BlorbReader(storyFile.Stream, false);
            }

            private BlorbReader reader;

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.reader != null)
                    {
                        this.reader.Dispose();
                        this.reader = null;
                    }
                }

                base.Dispose(disposing);
            }

            public override string GetStoryFileExtension()
            {
                //BabelTools.BlorbReader reader = new BlorbReader(storyFile.Stream, false);
                byte ver = this.StoryFile.ReadByte(0);
                return string.Format(".z{0:d}", ver);
            }

            public override string GetStoryFileIfid()
            {
                var metadata = this.reader.GetMetadata();

                XNamespace ns = "http://babel.ifarchive.org/protocol/iFiction/";

                var lameReader = metadata.CreateReader();
                XmlNamespaceManager xmlns = new XmlNamespaceManager(lameReader.NameTable);
                xmlns.AddNamespace("i", ns.NamespaceName);

                var ifid = metadata.XPathSelectElement("/i:ifindex/i:story/i:identification/i:ifid", xmlns);

                if (ifid != null)
                {
                    return ifid.Value;
                }

                return null;
            }

            public override Stream GetStoryFileMetadata()
            {
                return this.reader.GetMetadataStream();
            }

            public override Stream GetStoryFileCover()
            {
                return this.reader.GetCoverImageStream();
            }
        }
    }
}
