using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Model
{
    public class Game
    {
        private FileInfo file;

        public Game(string filename, string rootPath)
        {
            this.file = new FileInfo(filename);

            this.Title = Path.GetFileNameWithoutExtension(this.file.Name);
            this.Author = "An Author";
            this.FullPath = filename;
            this.RelativePath = filename.Substring(rootPath.Length + 1);
        }

        public string Title { get; private set; }
        public string Author { get; private set; }
        public string FullPath { get; private set; }
        public string RelativePath { get; private set; }
    }
}
