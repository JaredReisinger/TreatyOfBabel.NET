using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreatyOfBabel
{
    public abstract class TreatyStoryFileHandlerBase : IStoryFileHandler
    {
        public TreatyStoryFileHandlerBase(ITreatyProvider provider, IStoryFile storyFile)
        {
            this.Provider = provider;
            this.StoryFile = storyFile;
        }

        public IStoryFile StoryFile { get; private set; }
        public ITreatyProvider Provider { get; private set; }

        // All implementations must provide this...
        public abstract string GetStoryFileExtension();

        // These methods are optional, not all formats support them.
        public virtual string GetStoryFileIfid()
        {
            return null;
        }

        public virtual Stream GetStoryFileMetadata()
        {
            return null;
        }

        public virtual Stream GetStoryFileCover()
        {
            return null;
        }

        private bool disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Not sure if we want to do this here!
                    ////if (this.StoryFile != null)
                    ////{
                    ////    this.StoryFile.Dispose();
                    ////    this.StoryFile = null;
                    ////}
                }

                this.disposed = true;
            }
        }
    }
}
