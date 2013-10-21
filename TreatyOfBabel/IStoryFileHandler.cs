using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreatyOfBabel
{
    public interface IStoryFileHandler : IDisposable
    {
        ITreatyProvider Provider { get; }

        string GetStoryFileExtension();
        string GetStoryFileIfid();
        Stream GetStoryFileMetadata();
        Stream GetStoryFileCover();
    }
}
