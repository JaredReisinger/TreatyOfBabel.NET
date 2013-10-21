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
    ////[ExportMetadata("TreatyOfBabelPopularity", 1000)]
    internal class Zcode : ITreatyProvider
    {
        public string FormatName { get { return "zcode"; } }
        public string HomePage { get { return "http://www.inform-fiction.org"; } }
        public string[] FileExtensions { get { return new string[] { ".z3", ".z4", ".z5", ".z6", ".z7", ".z8" }; } }
        public uint Popularity { get { return 1000; } }

        public bool ClaimStoryFile(IStoryFile storyFile)
        {
            byte ver = storyFile.ReadByte(0);

            if (storyFile.Extent < 0x3c ||
                ver < 1 ||
                ver > 8)
            {
                return false;
            }

            for (uint i = 4; i <= 14; i += 2)
            {
                var j = ReadZint(storyFile, i);
                if (j > storyFile.Extent || j < 0x40)
                {
                    return false;
                }
            }

            return true;
        }

        public IStoryFileHandler GetStoryFileHandler(IStoryFile storyFile)
        {
            return new ZcodeHandler(this, storyFile);
        }


        private static UInt16 ReadZint(IStoryFile storyFile, uint offset)
        {
            var b0 = storyFile.ReadByte(offset);
            var b1 = storyFile.ReadByte(offset + 1);

            return (UInt16)((b0 << 8) | b1);
        }

        private class ZcodeHandler : TreatyStoryFileHandlerBase
        {
            public ZcodeHandler(ITreatyProvider provider, IStoryFile storyFile)
                : base(provider, storyFile)
            {
            }

            public override string GetStoryFileExtension()
            {
                byte ver = this.StoryFile.ReadByte(0);
                return string.Format(".z{0:d}", ver);
            }

            public override string GetStoryFileIfid()
            {
                if (this.StoryFile.Extent < 0x1d)
                {
                    throw new InvalidDataException("story file is too small!");
                }

                var ser = Encoding.ASCII.GetChars(this.StoryFile.ReadBytes(0x12, 6));

                // Detect vintage story files, don't bother scanning in that case!
                var vintage = (ser[0] == '8' ||
                               ser[0] == '9' ||
                               (ser[0] == '0' && ser[1] >= '0' && ser[1] <= '5'));

                if (!vintage)
                {
                    uint uuidIndex;
                    if (this.StoryFile.TryIndexOf("UUID://", out uuidIndex))
                    {
                        uuidIndex += 7;
                        uint uuidEnd;
                        if (this.StoryFile.TryIndexOf("/", uuidIndex, out uuidEnd))
                        {
                            var uuidBytes = this.StoryFile.ReadBytes(uuidIndex, uuidEnd - uuidIndex);
                            return Encoding.ASCII.GetString(uuidBytes);
                        }
                    }
                }

                // Did not find intact IFID.  Build one
                var storyVersion = ReadZint(this.StoryFile, 2);

                for (var j = 0; j < 6; ++j)
                {
                    if (!char.IsLetterOrDigit(ser[j]))
                    {
                        ser[j] = '-';
                    }
                }

                var serial = new string(ser);

                // Include the checksum if the first digit of serial is *not* '8',
                // and the serial itself is not all-zeros.
                if (ser[0] != '8' && !ser.All(b => b == '0'))
                {
                    var checksum = ReadZint(this.StoryFile, 0x1c);
                    return string.Format("ZCODE-{0:d}-{1}-{2:X4}", storyVersion, serial, checksum);
                }

                // else
                return string.Format("ZCODE-{0:d}-{1}", storyVersion, serial);
            }
        }
    }
}
