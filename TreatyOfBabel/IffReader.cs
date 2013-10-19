using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TreatyOfBabel
{
    public sealed class IffReader : IDisposable
    {
        private BinaryReader reader;

        public IffReader(string file)
        {
            this.reader = new BinaryReader(File.OpenRead(file));
        }

        public IffReader(BinaryReader reader)
        {
            this.reader = reader;
        }

        private bool disposed;
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.reader != null)
                {
                    this.reader.Dispose();
                    this.reader = null;
                }

                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        // Build a (nested?) tree of chunks...
        private void ReadIff()
        {

        }

        // Get chunks from the current offset, or offset passed in?
        public IEnumerable<IffInfo> GetChunks(uint offset)
        {
            uint currentOffset = offset;
            string typeId = "XXXX";

            while (!string.IsNullOrEmpty(typeId))
            {
                // IFF chunks *must* start on even byte boundaries...
                // Is this always true, or just for FORM?
                if ((currentOffset & 0x1) == 1)
                {
                    ++currentOffset;
                }

                this.reader.BaseStream.Position = currentOffset;
                typeId = this.ReadTypeId();

                if (!string.IsNullOrEmpty(typeId))
                {
                    var length = this.ReadUint();
                    yield return new IffInfo(currentOffset, typeId, length);
                    currentOffset += 4 + 4 + length;
                }
            }
        }

        public string ReadTypeId(uint offset)
        {
            this.reader.BaseStream.Position = offset;
            return this.ReadTypeId();
        }

        public string ReadTypeId()
        {
            var chars = this.reader.ReadChars(4);

            if (chars == null || chars.Length == 0)
            {
                return null;
            }

            System.Diagnostics.Debug.Assert(chars.Length == 4);
            return new string(chars);
        }

        public uint ReadUint(uint offset)
        {
            this.reader.BaseStream.Position = offset;
            return this.ReadUint();
        }

        public uint ReadUint()
        {
            return this.reader.ReadBigEndianUint32();
        }

        public byte[] ReadBytes(uint offset, uint length)
        {
            this.reader.BaseStream.Position = offset;
            return this.ReadBytes(length);
        }

        public byte[] ReadBytes(uint length)
        {
            return this.reader.ReadBytes((int)length);
        }
    }
}
