using System;
using System.Linq;

namespace TreatyOfBabel
{
    [System.Diagnostics.DebuggerDisplay("{TypeId} @{Offset}, {Length}")]
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
}
