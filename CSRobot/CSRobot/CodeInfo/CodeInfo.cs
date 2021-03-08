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
            if (options.ContainsKey("--dir"))
            {
                Detect(options["--dir"]);
            }
            else
            {
                Detect(Directory.GetCurrentDirectory());
            }
            ShowTable();
            ShowHotMap();
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
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌────────────────────┬────────────────────┬────────────────────┬────────────────────┐ ");
            Console.WriteLine("│     Extension      │      File Count    │     Line Count     │      Legend        │ ");
            Console.WriteLine("├────────────────────┼────────────────────┼────────────────────┼────────────────────┤ ");
            var i = 0;
            foreach (var mes in _fileDir)
            {
                Console.Write("│");
                Console.Write($" {mes.Key}".PadRight(20));
                Console.Write($"│ {mes.Value.Count}".PadRight(21));
                Console.Write($"│ {mes.Value.LineCount}".PadRight(21));
                Console.Write($"│         ");
                Console.ForegroundColor = _colorArr[((i+1) % _colorArr.Length)];
                Console.Write("★".PadRight(10));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("│ ");
                if (mes.Key != _fileDir.Keys.Last())
                {
                    Console.WriteLine("├────────────────────┼────────────────────┼────────────────────┼────────────────────┤ ");
                }
                i++;
            }
            Console.Write("└────────────────────┴────────────────────┴────────────────────┴────────────────────┘ ".PadRight(64));
            Console.ResetColor();
            Console.Write(" ".PadRight(64));
        }


        static ConsoleColor[] _colorArr = new ConsoleColor[] {
                  ConsoleColor.Red,
                  ConsoleColor.Green,
                  ConsoleColor.White,
                  ConsoleColor.Yellow,
                  ConsoleColor.Magenta
                };

        /// <summary>
        /// 显示热点图
        /// </summary>
        static void ShowHotMap()
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            var rang = new Point[42, 42];
            var maxRow = rang.GetLength(0) - 1;
            var maxCol = rang.GetLength(1) - 1;
            //初始化
            for (var r = 0; r <= maxRow; r++)
            {
                for (var c = 0; c <= maxCol; c++)
                {
                    rang[r, c] = new Point { Value = "  " };
                }
            }
            //边框
            for (var r = 0; r < maxRow; r++)
            {
                rang[r, 0] = new Point { Value = "│" };
                rang[r, maxCol] = new Point { Value = "│" };
                rang[0, r] = new Point { Value = "──" };
                rang[maxRow, r] = new Point { Value = "──" };
            }
            rang[maxRow, 0] = new Point { Value = "└" };
            rang[maxRow, maxCol] = new Point { Value = "┘" };
            rang[0, 0] = new Point { Value = "┌" };
            rang[0, maxCol] = new Point { Value = "┐" };
            LoadFileData();
            void LoadFileData()
            {
                var sum = Convert.ToDouble(_fileDir.Sum(s => s.Value.Count));
                var x = 0;
                var y = 0;
                var colorIndex = 1;
                var pointCount = 0;
                var newRan = new Point[maxRow - 1, maxCol - 1];
                var maxNewRow = newRan.GetLength(0);
                var maxNewCol = newRan.GetLength(1);

                foreach (var item in _fileDir)
                {
                    var itemCount = Convert.ToInt32(Math.Round(maxNewRow * maxNewCol * item.Value.Count / sum, MidpointRounding.ToZero));
                    var num = pointCount + itemCount;


                    for (; pointCount < num; pointCount++)
                    {
                        x = pointCount / maxNewRow;
                        y = pointCount % maxNewCol;
                        newRan[x, y] = new Point { Value = "★", Color = _colorArr[(colorIndex % _colorArr.Length)] };
                    }
                    colorIndex++;
                }
                for (var c = 0; c < maxNewCol; c++)
                {
                    if (newRan[maxNewRow - 1, c] == null)
                    {
                        newRan[maxNewRow - 1, c] = new Point { Value = "  " };
                    }
                }
                for (var r = 0; r < maxNewRow; r++)
                {
                    for (var c = 0; c < maxNewCol; c++)
                    {
                        rang[r + 1, c + 1] = newRan[r, c];
                    }
                }
            }
            Console.WriteLine("File Hot Map");
            Show();
            void Show()
            {
                //显示
                for (var r = 0; r <= maxRow; r++)
                {
                    for (var c = 0; c <= maxCol; c++)
                    {
                        if (rang[r, c] != null)
                        {
                            Console.ForegroundColor = rang[r, c].Color;
                            Console.Write(rang[r, c].Value);
                        }
                    }
                    Console.WriteLine();
                }
            }
            void LoadLineData()
            {
                var sum = Convert.ToDouble(_fileDir.Sum(s => s.Value.LineCount));
                var x = 0;
                var y = 0;
                var colorIndex = 1;
                var pointCount = 0;
                var newRan = new Point[maxRow - 1, maxCol - 1];
                var maxNewRow = newRan.GetLength(0);
                var maxNewCol = newRan.GetLength(1);

                foreach (var item in _fileDir)
                {
                    var itemCount = Convert.ToInt32(Math.Round(maxNewRow * maxNewCol * item.Value.LineCount / sum, MidpointRounding.ToZero));
                    var num = pointCount + itemCount;

                    for (; pointCount < num; pointCount++)
                    {
                        x = pointCount / maxNewRow;
                        y = pointCount % maxNewCol;
                        newRan[x, y] = new Point { Value = "★", Color = _colorArr[(colorIndex % _colorArr.Length)] };
                    }
                    colorIndex++;
                }
                for (var c = 0; c < maxNewCol; c++)
                {
                    if (newRan[maxNewRow - 1, c] == null)
                    {
                        newRan[maxNewRow - 1, c] = new Point { Value = "  " };
                    }
                }
                for (var r = 0; r < maxNewRow; r++)
                {
                    for (var c = 0; c < maxNewCol; c++)
                    {
                        rang[r + 1, c + 1] = newRan[r, c];
                    }
                }
            }

            LoadLineData();
            Console.WriteLine("Line Hot Map");
            Show();

        }
    }
    class Point
    {
        internal string Value { get; set; }
        internal ConsoleColor Color { get; set; } = ConsoleColor.White;
    }

    class FileCount
    {
        public long Count { get; set; }
        public long LineCount { get; set; }
    }
}
