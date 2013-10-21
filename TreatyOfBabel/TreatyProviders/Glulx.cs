using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace TreatyOfBabel.TreatyProviders
{
    [Export(typeof(ITreatyProvider))]
    internal class Glulx : ITreatyProvider
    {
        public string FormatName { get { return "glulx"; } }
        public string HomePage { get { return "http://eblong.com/zarf/glulx"; } }
        public string[] FileExtensions { get { return new string[] { ".ulx" }; } }
        public uint Popularity { get { return 950; } }

        const string Signature = "Glul";

        public bool ClaimStoryFile(IStoryFile storyFile)
        {
            if (storyFile.Extent > 256)
            {
                var sig = Encoding.ASCII.GetBytes(Signature);
                var magic = storyFile.ReadBytes(0, (uint)sig.Length);

                return sig.SequenceEqual(magic);
            }

            return false;
        }

        public IStoryFileHandler GetStoryFileHandler(IStoryFile storyFile)
        {
            return new GlulxHandler(this, storyFile);
        }

        private class GlulxHandler : TreatyStoryFileHandlerBase
        {
            public GlulxHandler(ITreatyProvider provider, IStoryFile storyFile)
                : base(provider, storyFile)
            {
            }

            public override string GetStoryFileExtension()
            {
                return ".ulx";
            }

            public override string GetStoryFileIfid()
            {
                return null;
            }
        }
    }
}
