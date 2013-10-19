using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    this.DumpIffInfo(chunk, reader);
                }
            }
        }

        private void DumpIffInfo(IffInfo info, IffReader reader, int depth = 0)
        {
            string padding = new string(' ', depth * 4);

            Console.Write("0x{0:x8} (0x{1:x8}) {2}{3}", info.Offset, info.Length, padding, info.TypeId);

            switch (info.TypeId)
            {
                case "FORM":
                    var formType = reader.ReadTypeId(info.ContentOffset);
                    Console.WriteLine(" - {0}", formType);
                    foreach (var chunk in reader.GetChunks(info.ContentOffset + 4))
                    {
                        this.DumpIffInfo(chunk, reader, depth + 1);
                    }
                    break;

                case "RIdx":
                    Console.WriteLine();
                    reader.ReadTypeId(info.Offset);
                    reader.ReadUint();
                    uint expectedCount = info.Length / 12;
                    for (uint i = 0; i < expectedCount; ++i)
                    {
                        var indexType = reader.ReadTypeId();
                        var indexId = reader.ReadUint();
                        var indexOffset = reader.ReadUint();
                    }
                    break;

                default:
                    Console.WriteLine();
                    break;
            }
        }
    }
}
