using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace TreatyOfBabel
{
    public class BlorbReader
    {
        public BlorbReader(string file)
        {
            using (var reader = new IffReader(file))
            {
                this.ReadBlorb(reader);
                this.CoverImage = this.GetCoverImage(reader);
            }
        }

        private void ReadBlorb(IffReader reader)
        {
            foreach (var chunk in reader.GetChunks(0))
            {
                var node = new IffInfoNode(null, chunk);
                this.HandleIffInfoNode(node, reader);
            }
        }

        // Get metadata
        public XDocument Metadata { get { return this.metadata; } }
        public BitmapImage CoverImage { get; private set; }

        private BitmapImage GetCoverImage(IffReader reader)
        {
            if (!this.coverImage.HasValue)
            {
                return null;
            }

            uint coverOffset;
            if (this.pictResources == null || !this.pictResources.TryGetValue(this.coverImage.Value, out coverOffset))
            {
                return null;
            }

            var artTypeId = reader.ReadTypeId(coverOffset);
            var artLength = reader.ReadUint();

            var bytes = reader.ReadBytes(artLength);

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = new MemoryStream(bytes);
            bmp.EndInit();

            return bmp;
        }

        List<IffInfoNode> infoNodes = new List<IffInfoNode>();

        // resource index data...
        private bool sawResources = false;
        private Dictionary<uint, uint> pictResources = null;
        private Dictionary<uint, uint> sndResources = null;
        private Dictionary<uint, uint> dataResources = null;
        private Dictionary<uint, uint> execResources = null;

        private XDocument metadata = null;

        private uint? coverImage;

        private void HandleIffInfoNode(IffInfoNode info, IffReader reader, int depth = 0)
        {
            this.infoNodes.Add(info);

            switch (info.TypeId)
            {
                case "FORM":
                    var formType = reader.ReadTypeId(info.ContentOffset);
                    if (depth == 0 && formType != "IFRS")
                    {
                        Console.WriteLine("Unexpected FORM subtype!");
                    }

                    foreach (var chunk in reader.GetChunks(info.ContentOffset + 4))
                    {
                        var node = new IffInfoNode(info, chunk);
                        this.HandleIffInfoNode(node, reader, depth + 1);
                    }
                    break;

                case "RIdx":
                    if (this.sawResources)
                    {
                        Console.WriteLine(" - already saw resource index!  Unexpected!");
                    }
                    else
                    {
                        Console.WriteLine();
                        var count = reader.ReadUint(info.Offset + 8);
                        uint expectedCount = (info.Length - 4) / 12;
                        System.Diagnostics.Debug.Assert(count == expectedCount);
                        for (uint i = 0; i < expectedCount; ++i)
                        {
                            var indexType = reader.ReadTypeId();
                            var indexId = reader.ReadUint();
                            var indexOffset = reader.ReadUint();

                            switch (indexType)
                            {
                                case "Pict":
                                    this.UpdateResourceIndex(ref this.pictResources, indexId, indexOffset);
                                    break;
                                case "Snd ":
                                    this.UpdateResourceIndex(ref this.sndResources, indexId, indexOffset);
                                    break;
                                case "Data":
                                    this.UpdateResourceIndex(ref this.dataResources, indexId, indexOffset);
                                    break;
                                case "Exec":
                                    this.UpdateResourceIndex(ref this.execResources, indexId, indexOffset);
                                    break;
                                default:
                                    Console.WriteLine("* unknown resource type {0}", indexType);
                                    break;
                            }
                        }

                        this.sawResources = true;
                    }
                    break;

                case "IFmd":
                    if (metadata != null)
                    {
                        Console.WriteLine(" - extra ID metadata? This is unexpected!");
                    }
                    else
                    {
                        var bytes = reader.ReadBytes(info.Offset + 8, info.Length);
                        using (var stream = new MemoryStream(bytes))
                        {
                            this.metadata = XDocument.Load(stream);
                        }
                    }
                    break;

                case "Fspc":
                    if (this.coverImage.HasValue)
                    {
                        Console.WriteLine(" - already have a Frontspiece!  Unexpected!");
                    }
                    else
                    {
                        this.coverImage = reader.ReadUint(info.Offset + 8);
                    }
                    break;

                default:
                    break;
            }
        }

        private void UpdateResourceIndex(ref Dictionary<uint, uint> resources, uint index, uint offset)
        {
            if (resources == null)
            {
                resources = new Dictionary<uint, uint>();
            }

            if (resources.ContainsKey(index))
            {
                Console.WriteLine("* resource dictionary already has an item with index {0}!", index);
                return;
            }

            resources.Add(index, offset);
        }
    }
}
