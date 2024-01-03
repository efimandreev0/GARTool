using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EverOasisGAR
{
    class Entry
    {
        public int fileSize { get; set; }
        public int fileBegin { get; set; }
        public int fileNameOffset { get; set; }
        public int fileNameInCompilationOffset { get; set; }
        public string fileName { get; set; }
        public string fileNameInCompilation{ get; set; }
        public Entry(BinaryReader reader)
        {
            fileSize = reader.ReadInt32();
            fileBegin = reader.ReadInt32();
            fileNameOffset = reader.ReadInt32();
            fileNameInCompilationOffset = reader.ReadInt32();
            var pred = reader.BaseStream.Position;
            reader.BaseStream.Position = fileNameOffset;
            fileName = Utils.ReadString(reader, Encoding.UTF8);
            if (fileNameInCompilationOffset <= 0)
            {
                reader.BaseStream.Position += 4;
            }
            else
            {
                reader.BaseStream.Position = fileNameInCompilationOffset;
                fileNameInCompilation = Utils.ReadString(reader, Encoding.UTF8);
            }
            reader.BaseStream.Position = pred;
        }
    }
}
