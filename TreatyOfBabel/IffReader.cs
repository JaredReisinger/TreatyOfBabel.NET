using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TreatyOfBabel
{
    public class IffReader : IDisposable
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
                // TODO: offsets must be even?  Add 1 if needed?
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

        public uint ReadUint()
        {
            return this.reader.ReadBigEndianUint32();
        }
    }

    public class IffInfo
    {
        public IffInfo(uint offset, string typeId, uint length)
        {
            this.Offset = offset;
            this.TypeId = typeId;
            this.Length = length;
        }

        public uint Offset { get; private set; }
        public string TypeId { get; private set; }  // add sub-type, for FORM?
        public uint Length { get; private set; }
        public uint ContentOffset { get { return this.Offset + 4 + 4; } }
    }

    public static class BinaryReaderExtensions
    {
        public static uint ReadBigEndianUint32(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);

            if (bytes == null || bytes.Length != 4)
            {
                throw new InvalidDataException("Expected to read at least 4 bytes!");
            }

            return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | ((uint)bytes[3]);
        }
    }
}
