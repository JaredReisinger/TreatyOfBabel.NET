using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace GameLibrary.ViewModel
{
    public class GameViewModel : ViewModelBaseEx
    {
        private FileInfo file;

        public GameViewModel(Model.Game game)
        {
            this.Path = game.RelativePath;
            this.file = new FileInfo(game.FullPath);

            this.Title = game.Title;
            this.Author = game.Author;
            this.Image = game.Image;
        }

        private string path;
        public string Path
        {
            get { return this.path; }
            private set { this.Set(ref this.path, value); }
        }

        private string title;
        public string Title
        {
            get { return this.title; }
            private set { this.Set(ref this.title, value); }
        }

        private string author;
        public string Author
        {
            get { return this.author; }
            private set { this.Set(ref this.author, value); }
        }

        private string image;
        public string Image
        {
            get { return this.image; }
            private set { this.Set(ref this.image, value); }
        }
    }
}
