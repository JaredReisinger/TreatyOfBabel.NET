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
                    Console.WriteLine("chunk @0x{0:x8}, {1}, length 0x{2:x8}", chunk.Offset, chunk.TypeId, chunk.Length);

                    if (chunk.TypeId == "FORM")
                    {
                        foreach (var chunkInner in reader.GetChunks(chunk.Offset + 4 + 4 + 4))
                        {
                            Console.WriteLine("--chunk @0x{0:x8}, {1}, length 0x{2:x8}", chunkInner.Offset, chunkInner.TypeId, chunkInner.Length);
                        }
                    }
                }
            }
        }
    }
}
