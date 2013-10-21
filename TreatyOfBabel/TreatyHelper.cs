using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreatyOfBabel
{
    // Helps clients read/understand treaty files...
    public class TreatyHelper
    {
        public TreatyHelper()
        {
            var catalog = new AssemblyCatalog(this.GetType().Assembly);
            var container = new CompositionContainer(catalog);
            
            this.treaties = container.GetExports<ITreatyProvider>().ToList();
        }

        List<Lazy<ITreatyProvider>> treaties;

        public bool IsTreatyFile(string filename)
        {
            using (var storyFile = new StoryFile(filename))
            {
                foreach (var treaty in this.treaties)
                {
                    if (treaty.Value.ClaimStoryFile(storyFile))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryGetHandler(string filename, out IStoryFileHandler handler)
        {
            handler = null;
            StoryFile storyFile = null;
            try
            {
                storyFile = new StoryFile(filename);

                // Group the treaty providers into "likely" and "unlikely" buckets,
                // based on file-extension matching, and sort by popularity.  *Then*
                // see who's the first to claim it.
                var ext = Path.GetExtension(filename);

                var orderedTreaties = this.treaties
                    .OrderBy(t => t.Value.FileExtensions.Contains(ext) ? 0 : 1)
                    .ThenByDescending(t => t.Value.Popularity);

                foreach (var treaty in orderedTreaties)
                {
                    if (treaty.Value.ClaimStoryFile(storyFile))
                    {
                        handler = treaty.Value.GetStoryFileHandler(storyFile);
                        storyFile = null;
                        return true;
                    }
                }
            }
            finally
            {
                if (storyFile != null)
                {
                    storyFile.Dispose();
                }
            }

            return false;
        }
    }
}

