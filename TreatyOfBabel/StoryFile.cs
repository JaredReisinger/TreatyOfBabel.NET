using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreatyOfBabel
{
    internal sealed class StoryFile : IStoryFile
    {
        const int InitialBufferSize = 128;

        public StoryFile(string filename)
        {
            this.filename = filename;
            FileInfo info = new FileInfo(filename);

            uint extent = (uint)info.Length;
            var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            this.Init(stream, extent);
        }

        public StoryFile(Stream stream)
        {
            this.Init(stream, (uint)stream.Length);
        }

        public StoryFile(Stream stream, uint extent)
        {
            this.Init(stream, extent);
        }

        private void Init(Stream stream, uint extent)
        {
            this.Stream = stream;
            this.Extent = extent;

            // Cache the first 'InitialBufferSize' bytes for
            // quick access...
            this.initialBuffer = new byte[Math.Min(InitialBufferSize, extent)];

            var read = this.Stream.Read(this.initialBuffer, 0, this.initialBuffer.Length);
        }

        private string filename;
        private byte[] initialBuffer;

        public Stream Stream { get; private set; }
        public uint Extent { get; private set;}

        public byte ReadByte(uint position)
        {
            if (position < this.initialBuffer.Length)
            {
                return this.initialBuffer[position];
            }

            this.Stream.Position = position;
            return (byte)this.Stream.ReadByte();
        }

        public byte[] ReadBytes(uint position, uint length)
        {
            var buffer = new byte[length];

            if (position + length < this.initialBuffer.Length)
            {
                Array.Copy(this.initialBuffer, position, buffer, 0, length);
            }
            else
            {
                this.Stream.Position = position;
                this.Stream.Read(buffer, 0, (int)length);
            }

            return buffer;
        }

        public bool TryIndexOf(string sequence, out uint index)
        {
            return this.TryIndexOf(sequence, 0, out index);
        }

        public bool TryIndexOf(string sequence, uint start, out uint index)
        {
            return this.TryIndexOf(sequence, start, this.Extent, out index);
        }

        public bool TryIndexOf(string sequence, uint start, uint end, out uint index)
        {
            var sequenceBytes = Encoding.ASCII.GetBytes(sequence);
            return this.TryIndexOf(sequenceBytes, start, end, out index);
        }

        public bool TryIndexOf(byte[] sequence, out uint index)
        {
            return this.TryIndexOf(sequence, 0, out index);
        }

        public bool TryIndexOf(byte[] sequence, uint start, out uint index)
        {
            return this.TryIndexOf(sequence, start, this.Extent, out index);
        }

        public bool TryIndexOf(byte[] sequence, uint start, uint end, out uint index)
        {
            bool found = false;
            var testEnd = end - sequence.Length;

            index = uint.MaxValue;

            for (uint pos = start; pos <= testEnd; ++pos)
            {
                // This is a naive search, not a fast one (see Knuth!)
                var b = this.ReadByte(pos);

                if (b == sequence[0])
                {
                    // Special-case short matches!
                    if (sequence.Length == 1)
                    {
                        index = pos;
                        return true;                        
                    }

                    var test = this.ReadBytes(pos, (uint)sequence.Length);
                    if (sequence.SequenceEqual(test))
                    {
                        index = pos;
                        return true;
                    }
                }
            }


            return found;
        }

        private bool disposed;
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (this.Stream != null)
                {
                    this.Stream.Dispose();
                    this.Stream = null;
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}
