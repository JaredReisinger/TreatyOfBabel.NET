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
    internal class Tads2 : ITreatyProvider
    {
        public string FormatName { get { return "tads2"; } }
        public string HomePage { get { return "http://www.tads.org"; } }
        public string[] FileExtensions { get { return new string[] { ".gam" }; } }
        public uint Popularity { get { return 900; } }

        //const string Signature = "TADS2 bin\012\015\032";
        const string Signature = "TADS2 bin\x0A\x0D\x1A";

        public bool ClaimStoryFile(IStoryFile storyFile)
        {
            var sig = Encoding.ASCII.GetBytes(Signature);
            var magic = storyFile.ReadBytes(0, (uint)sig.Length);

            return sig.SequenceEqual(magic);
        }

        public IStoryFileHandler GetStoryFileHandler(IStoryFile storyFile)
        {
            return new Tads2Handler(this, storyFile);
        }


        private class Tads2Handler : TreatyStoryFileHandlerBase
        {
            public Tads2Handler(ITreatyProvider provider, IStoryFile storyFile)
                : base(provider, storyFile)
            {
            }

            public override string GetStoryFileExtension()
            {
                return ".gam";
            }

            public override string GetStoryFileIfid()
            {
                return null;
            }
        }
    }
}
