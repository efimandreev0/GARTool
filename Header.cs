using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverOasisGAR
{
    class Header
    {
        public string magic { get; set; }
        public int fileSize { get; set; }
        public short fileCount { get; set; }
        public int blockSize { get; set; }
        public int nextStart { get; set; }
        public int nextEnd { get; set; }
        public string gameCodeName { get; set; }
        public Header(BinaryReader reader)
        {
            magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            fileSize = reader.ReadInt32();
            reader.ReadInt16();
            fileCount = reader.ReadInt16();
            blockSize = reader.ReadInt32();
            nextStart = reader.ReadInt32();
            nextEnd = reader.ReadInt32();
            gameCodeName = Utils.ReadString(reader, Encoding.UTF8);
            reader.BaseStream.Position = nextStart;
        }
    }
}
