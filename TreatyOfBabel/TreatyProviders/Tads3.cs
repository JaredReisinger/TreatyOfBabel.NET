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
    internal class Tads3 : ITreatyProvider
    {
        public string FormatName { get { return "tads3"; } }
        public string HomePage { get { return "http://www.tads.org"; } }
        public string[] FileExtensions { get { return new string[] { ".t3" }; } }
        public uint Popularity { get { return 900; } }

        //const string Signature = "T3-image\015\012\032";
        const string Signature = "T3-image\x0D\x0A\x1A";

        public bool ClaimStoryFile(IStoryFile storyFile)
        {
            var sig = Encoding.ASCII.GetBytes(Signature);
            var magic = storyFile.ReadBytes(0, (uint)sig.Length);

            return sig.SequenceEqual(magic);
        }

        public IStoryFileHandler GetStoryFileHandler(IStoryFile storyFile)
        {
            return new Tads3Handler(this, storyFile);
        }


        private class Tads3Handler : TreatyStoryFileHandlerBase
        {
            public Tads3Handler(ITreatyProvider provider, IStoryFile storyFile)
                : base(provider, storyFile)
            {
            }

            public override string GetStoryFileExtension()
            {
                return ".t3";
            }

            public override string GetStoryFileIfid()
            {
                return null;
            }
        }
    }
}
