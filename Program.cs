using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverOasisGAR
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Rebuild("UICommon.gar", "UICommon");
            if (args.Length > 1)
            {
                Rebuild(args[0], args[1]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string gar)
        {
            string result = "";
            var reader = new BinaryReader(File.OpenRead(gar));
            Header header = new Header(reader);
            Entry[] entries = new Entry[header.fileCount];
            string str = Path.GetFileNameWithoutExtension(gar) + "/";
            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(gar));
            for (int i = 0; i < header.fileCount; i++)
            {
                entries[i] = new Entry(reader);
                var pred = reader.BaseStream.Position;
                reader.BaseStream.Position = entries[i].fileBegin;
                if (entries[i].fileNameInCompilationOffset <= 0)
                {
                    result = "bin";
                }
                else
                {
                    int index = entries[i].fileNameInCompilation.IndexOf(".");
                    result = entries[i].fileNameInCompilation.Substring(index + 1).TrimStart();
                }
                byte[] file = reader.ReadBytes(entries[i].fileSize);
                File.WriteAllBytes(str + entries[i].fileName + "." + result, file);
                reader.BaseStream.Position = pred;
            }
            Console.WriteLine($"File \"{Path.GetFileName(gar)}\" was extracted successfully!\nFile count: {header.fileCount}\nCode name: {header.gameCodeName}\n\nPRESS ANY KEY TO CONTINUE");
            Console.ReadKey();
        }
        public static void Rebuild(string gar, string input)
        {
            string str = Path.GetFileNameWithoutExtension(gar) + "/";
            string[] files = SortFilesByType(input);
            int[] fileSize = new int[files.Length];
            int[] fileOffset = new int[files.Length];
            var reader = new BinaryReader(File.OpenRead(gar));
            Header header = new Header(reader);
            Entry[] entries = new Entry[header.fileCount];
            for (int i = 0; i < files.Length; i++)
            {
                entries[i] = new Entry(reader);
            }
            reader.Close();
            var writer = new BinaryWriter(File.OpenWrite(gar));
            writer.BaseStream.Position = header.nextEnd;
            for (int i = 0; i < files.Length; i++)
            {
                fileOffset[i] = (int)writer.BaseStream.Position;
                byte[] bytes = File.ReadAllBytes(str + files[i]);
                writer.Write(bytes);
                fileSize[i] = bytes.Length;
            }
            writer.BaseStream.Position = header.nextStart;
            for (int i = 0; i < files.Length; i++)
            {
                writer.Write(fileSize[i]);
                writer.Write(fileOffset[i]);
                writer.BaseStream.Position += 4;
                writer.BaseStream.Position += 4;
            }
            writer.BaseStream.Position = 4;
            writer.Write((int)writer.BaseStream.Length);
            Console.WriteLine($"File \"{Path.GetFileName(gar)}\" was rebuild successfully!\n\nPRESS ANY KEY TO CONTINUE");
            Console.ReadKey();
        }
        static string[] SortFilesByType(string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            FileInfo[] files = directory.GetFiles(); // Получение списка файлов

            FileInfo[] sortedFiles = files.OrderBy(file => file.Extension).ToArray(); // Сортировка по типу

            string[] sortedFileNames = sortedFiles.Select(file => file.Name).ToArray();

            return sortedFileNames;
        }
    }
}
