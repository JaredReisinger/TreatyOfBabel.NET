using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml;
using CommandLine;
using TreatyOfBabel;

namespace IffDump
{
    internal class Program
    {
        private class CommandLineArgs
        {
            [DefaultArgument(ArgumentType.Required,
                HelpText = "The file to analyze.")]
            public string InputFile = null;

            [Argument(ArgumentType.Undocumented,
                ShortName = "",
                HelpText = "Pause before exiting.")]
            public bool PauseBeforeExiting = false;


            public string GetArgValues()
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("args: (InputFile): {0}", this.InputFile));

                return sb.ToString();
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine(Parser.ArgumentsUsage(typeof(CommandLineArgs)));
        }

        private static void WaitToExit()
        {
            Console.WriteLine();
            Console.Write("Press any key to exit...");
            Console.ReadKey(true);
        }

        static void Main(string[] args)
        {
            Program program = new Program(args);
            program.Run();
        }

        private string[] args;

        private Program(string[] args)
        {
            this.args = args;
        }

        private void Run()
        {
            var parsedArgs = new CommandLineArgs();

            try
            {
                if (!Parser.ParseArgumentsWithUsage(this.args, parsedArgs))
                {
                    throw new ArgumentException("** CommandLine.Parser **");
                }

                // analyze the file...
                this.DumpIffFile(parsedArgs);
            }
            catch (Exception ex)
            {
                // Report it?
                if (!(ex is ArgumentException && ex.Message == "** CommandLine.Parser **"))
                {
                    Console.WriteLine("EXCEPTION: {0}", ex.Message);
                }

                // auto-pause on exceptions if we're debugging.
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    parsedArgs.PauseBeforeExiting = true;
                }
            }
            finally
            {
                if (parsedArgs.PauseBeforeExiting)
                {
                    WaitToExit();
                }
            }
        }

        private void DumpIffFile(CommandLineArgs commandLine)
        {
            // header info...
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("IffDump v0.1");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine();

            Console.WriteLine("Getting IFF information from {0}...", commandLine.InputFile);

            using (IffReader reader = new IffReader(commandLine.InputFile))
            {
                foreach (var chunk in reader.GetChunks(0))
                {
                    this.HandleIffInfo(chunk, reader);
                }

                // show extracted resources...
                Console.WriteLine();
                Console.WriteLine("metadata - {0}", this.metadata.DocumentElement.Name);
                Console.WriteLine(
                    "resources - {0}, {1}, {2}, {3}",
                    this.pictResources == null ? "--" : this.pictResources.Count.ToString(),
                    this.sndResources == null ? "--" : this.sndResources.Count.ToString(),
                    this.dataResources == null ? "--" : this.dataResources.Count.ToString(),
                    this.execResources == null ? "--" : this.execResources.Count.ToString());

                if (!this.coverImage.HasValue)
                {
                    Console.WriteLine("file has no cover art");
                }
                else
                {
                    uint artOffset;
                    if (this.pictResources == null || !this.pictResources.TryGetValue(this.coverImage.Value, out artOffset))
                    {
                        Console.WriteLine("could not find cover art resource (pict {0})", this.coverImage.Value);
                    }
                    else
                    {
                        var artTypeId = reader.ReadTypeId(artOffset);
                        var artLength = reader.ReadUint();
                        Console.WriteLine("cover art is {0} resource at offset 0x{1:x8}, length 0x{2:x8}", artTypeId, artOffset, artLength);
                        using (var stream = new MemoryStream(reader.ReadBytes(artLength)))
                        {
                            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                            var frame = decoder.Frames[0];
                            Console.WriteLine("cover art is {0}x{1}", frame.PixelWidth, frame.PixelHeight);
                        }
                    }
                }
            }
        }

        // resource index data...
        private bool sawResources = false;
        private Dictionary<uint, uint> pictResources = null;
        private Dictionary<uint, uint> sndResources = null;
        private Dictionary<uint, uint> dataResources = null;
        private Dictionary<uint, uint> execResources = null;

        private XmlDocument metadata = null;

        private uint? coverImage;

        private void HandleIffInfo(IffInfo info, IffReader reader, int depth = 0)
        {
            string padding = new string(' ', depth * 4);
            string nestedPadding = new string(' ', 10 + 1 + 1 + 10 + 1 + 1 + (depth * 4) + 2);

            Console.Write("0x{0:x8} (0x{1:x8}) {2}{3}", info.Offset, info.Length, padding, info.TypeId);

            switch (info.TypeId)
            {
                case "FORM":
                    var formType = reader.ReadTypeId(info.ContentOffset);
                    Console.WriteLine(" - {0}", formType);
                    foreach (var chunk in reader.GetChunks(info.ContentOffset + 4))
                    {
                        this.HandleIffInfo(chunk, reader, depth + 1);
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
                            Console.WriteLine("{0}{1} {2:d2} @0x{3:x8}", nestedPadding, indexType, indexId, indexOffset);

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
                        var xml = new XmlDocument();
                        var bytes = reader.ReadBytes(info.Offset + 8, info.Length);
                        using (var stream = new MemoryStream(bytes))
                        {
                            xml.Load(stream);
                        }
                        Console.WriteLine(" - extracted ({0})", xml.DocumentElement.Name);
                        this.metadata = xml;
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
                        Console.WriteLine(" - image {0}", this.coverImage.Value);
                    }
                    break;

                default:
                    Console.WriteLine();
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
