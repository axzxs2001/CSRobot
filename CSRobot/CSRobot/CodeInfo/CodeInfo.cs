using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace CSRobot.CodeInfo
{

    static class CodeInfo
    {
        internal static bool ShowCodeInfo(CommandOptions options)
        {
            Console.WriteLine("Statistics ......");
            if (options.ContainsKey("--dir"))
            {
                Detect(options["--dir"]);

            }
            else
            {
                Detect(Directory.GetCurrentDirectory());
            }
            Console.SetCursorPosition(Console.CursorLeft, 0);
            Console.CursorVisible = true;
            ShowHotBlock();
            ShowTable();
            return true;
        }
        #region 获取数据

        static Dictionary<string, FileCount> _fileDir = new Dictionary<string, FileCount>();
        static void Detect(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                Detect(dir);
            }
            foreach (var file in Directory.GetFiles(path))
            {
                var result = IsText(file, 128);
                if (result)
                {
                    var key = Path.GetExtension(file);
                    if (_fileDir.ContainsKey(key))
                    {
                        var fileCount = _fileDir[key];
                        fileCount.Count += 1;
                        fileCount.LineCount += GetCodeLines(file);
                        _fileDir[key] = fileCount;
                    }
                    else
                    {
                        _fileDir[key] = new FileCount { Count = 1, LineCount = GetCodeLines(file) };
                    }
                }
            }
        }
        static long GetCodeLines(string file)
        {
            var lines = File.ReadAllLines(file);
            var count = 0;
            foreach (var line in lines)
            {
                if (line.Trim() != "")
                {
                    count++;
                }
            }
            return count;
        }
        static bool IsText(string fileName, int windowSize)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                var rawData = new byte[windowSize];
                var text = new char[windowSize];
                var isText = true;
                var rawLength = fileStream.Read(rawData, 0, rawData.Length);
                fileStream.Seek(0, SeekOrigin.Begin);
                Encoding encoding;
                if (rawData[0] == 0xef && rawData[1] == 0xbb && rawData[2] == 0xbf)
                {
                    encoding = Encoding.UTF8;
                }
                else if (rawData[0] == 0xfe && rawData[1] == 0xff)
                {
                    encoding = Encoding.Unicode;
                }
                else if (rawData[0] == 0 && rawData[1] == 0 && rawData[2] == 0xfe && rawData[3] == 0xff)
                {
                    encoding = Encoding.UTF32;
                }
                else
                {
                    encoding = Encoding.Default;
                }
                using (var streamReader = new StreamReader(fileStream))
                {
                    streamReader.Read(text, 0, text.Length);
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream, encoding))
                    {
                        streamWriter.Write(text);
                        streamWriter.Flush();
                        var memoryBuffer = memoryStream.GetBuffer();
                        for (var i = 0; i < rawLength && isText; i++)
                        {
                            isText = rawData[i] == memoryBuffer[i];
                        }
                    }
                }
                return isText;
            }
        }
        #endregion


        /// <summary>
        /// 显示表格
        /// </summary>
        static void ShowTable()
        {
            Console.WriteLine("File and content line statistics");
            Console.WriteLine("┌────────────────────┬────────────────┬─────────────────┬────────────────┬─────────────────┬────────────┐ ");
            Console.WriteLine("│   Extension        │   File Count   │ File Percentage │   Line Count   │ Line Percentage │  Legend    │ ");
            Console.WriteLine("├────────────────────┼────────────────┼─────────────────┼────────────────┼─────────────────┼────────────┤ ");
            var i = 0;
            foreach (var mes in _fileDir)
            {

                Console.Write($"│ {mes.Key}".PadRight(21));
                Console.Write($"│ {mes.Value.Count}".PadRight(17));
                var sum = Convert.ToDouble(_fileDir.Sum(s => s.Value.Count));
                Console.Write($"│ {Math.Round(100 * mes.Value.Count / sum, 2) + "%"}".PadRight(18));
                Console.Write($"│ {mes.Value.LineCount}".PadRight(17));
                sum = Convert.ToDouble(_fileDir.Sum(s => s.Value.LineCount));
                Console.Write($"│ {Math.Round(100 * mes.Value.LineCount / sum, 2) + "%"}".PadRight(18));
                Console.Write($"│     ");
                Console.BackgroundColor = _colorArr[((i + 1) % _colorArr.Length)];
                Console.Write(" ");
                Console.ResetColor();
                Console.Write("".PadRight(6));
                Console.WriteLine("│ ");
                if (mes.Key != _fileDir.Keys.Last())
                {
                    Console.WriteLine("├────────────────────┼────────────────┼─────────────────┼────────────────┼─────────────────┼────────────┤ ");
                }
                i++;
            }
            Console.Write("└────────────────────┴────────────────┴─────────────────┴────────────────┴─────────────────┴────────────┘ ");
            Console.ResetColor();
            Console.Write(" ".PadRight(64));
        }

        static void ShowHotBlock()
        {
            var sum = Convert.ToDouble(_fileDir.Sum(s => s.Value.Count));
            var sumLength = 110;
            Console.WriteLine("");
            Console.WriteLine("Percentage Legend of File");
            ShowFile();
            ShowFile();
            ShowFile();
            void ShowFile()
            {
                var i = 0;
                foreach (var item in _fileDir)
                {
                    var itemCount = Convert.ToInt32(Math.Round(sumLength * item.Value.Count / sum, MidpointRounding.ToZero));
                    Console.BackgroundColor = _colorArr[((i + 1) % _colorArr.Length)];
                    Console.Write("".PadRight(itemCount, ' '));
                    i++;
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine("Percentage Legend of Line");
            var lineSum = Convert.ToDouble(_fileDir.Sum(s => s.Value.LineCount));
            ShowLine();
            ShowLine();
            ShowLine();
            void ShowLine()
            {
                var i = 0;
                foreach (var item in _fileDir)
                {
                    var itemCount = Convert.ToInt32(Math.Round(sumLength * item.Value.LineCount / lineSum, MidpointRounding.ToZero));
                    Console.BackgroundColor = _colorArr[((i + 1) % _colorArr.Length)];
                    Console.Write("".PadRight(itemCount, ' '));
                    i++;
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        static ConsoleColor[] _colorArr = new ConsoleColor[] {
                  ConsoleColor.Red,
                  ConsoleColor.Green,
                  ConsoleColor.Yellow,
                  ConsoleColor.Magenta,
                  ConsoleColor.DarkRed,
                  ConsoleColor.DarkGreen,
                  ConsoleColor.DarkYellow,
                  ConsoleColor.DarkMagenta
                };

        class FileCount
        {
            public long Count { get; set; }
            public long LineCount { get; set; }
        }
    }
}
