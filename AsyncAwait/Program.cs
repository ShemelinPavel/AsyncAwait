using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static string filesVaultDir;
        static ushort filesCount;
        static ushort fileLineCount;
        static Random rOperation;
        static Random rPartMathOperation;
        static Random rMulti;

        static void Log(string message)
        {
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {

            Init();

            if (!(CreateFilesVault()))
            {
                Console.ReadKey();
                return;
            }

            List<FileInfo> files;

            if (!(GenerateFiles(out files)))
            {
                Console.ReadKey();
                return;
            }

            Log("Подготовка завершена\n Для выполнения процедуры расчета нажмите любую клавишу...");

            Task<double>[] tasks = new Task<double>[files.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ReadAndCalculateFileData(files[i]);
            }

            for (int i = 0; i < 100; i++)
            {
                Log($"!{i}");
            }


            Console.ReadKey();
        }


        static async Task<Double> ReadAndCalculateFileData(FileInfo file)
        {
            double res = await ReadAndCalculateFileDataAsync(file);

            Log($"{file.FullName}  {res.ToString()}");

            return res;
        }

        private static Task<double> ReadAndCalculateFileDataAsync(FileInfo file)
        {

            return Task.Run<double>(() =>
            {
                Double res = 0;

                using (StreamReader sReader = new StreamReader(file.FullName))
                {

                    while (!(sReader.EndOfStream))
                    {
                        string[] curString = sReader.ReadLine().Split('/');
                        if (curString[0] == "1")
                        {
                            res += Double.Parse(curString[1]) * Double.Parse(curString[2]);
                        }
                        else if (curString[0] == "2")
                        {
                            res += Double.Parse(curString[1]) / Double.Parse(curString[2]);
                        }
                    }
                }
                return res;
            }
            );
        }

    static void Init()
        {
            Log("Инициализация...");

            filesCount = 200;
            fileLineCount = 2000;

            rOperation = new Random(0);
            rPartMathOperation = new Random();
            rMulti = new Random(100);
        }

        static bool CreateFilesVault()
        {

            string currentAppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            filesVaultDir = currentAppDir + @"\Files";

            try
            {

                if (!(Directory.Exists(filesVaultDir)))
                {
                    Log($"Создание каталога для файлов: {filesVaultDir} ...");
                    Directory.CreateDirectory(filesVaultDir);
                }
                else
                {
                    Log($"Каталог для файлов: {filesVaultDir} уже существует...");

                    DirectoryInfo directoryInfo = new DirectoryInfo(filesVaultDir);

                    FileInfo[] fInfo = directoryInfo.GetFiles();

                    if (fInfo.Length != 0)
                    {
                        Log("Чистка существующего каталога для файлов...");
                        foreach (FileInfo item in fInfo)
                        {
                            item.Delete();
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log($"Что-то пошло не так: {e.Message}");
                return false;
            }
        }

        static bool GenerateFiles(out List<FileInfo> files)
        {
            try
            {
                Log("Генерация файлов данных...");

                files = new List<FileInfo>();

                for (ushort i = 0; i < filesCount; i++)
                {
                    string filename = filesVaultDir + $"\\{i}.txt";
                    using (StreamWriter sWriter = new StreamWriter(filename))
                    {
                        for (ushort j = 1; j <= fileLineCount; j++)
                        {
                            sWriter.WriteLine($"{rOperation.Next(1, 3)}/{rPartMathOperation.NextDouble() * rMulti.Next(1, 1000000)}/{rPartMathOperation.NextDouble() * rMulti.Next(1, 1000000)}");
                        }
                    }
                    files.Add(new FileInfo(filename));
                }
                return true;
            }
            catch (Exception e)
            {
                Log($"Что-то пошло не так: {e.Message}");

                files = null;

                return false;
            }
        }
    }
}