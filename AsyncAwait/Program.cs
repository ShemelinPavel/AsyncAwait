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
        /// <summary>
        /// директория где будут находится файла
        /// </summary>
        static string filesVaultDir;

        /// <summary>
        /// количество файлов с данными
        /// </summary>
        static ushort filesCount;

        /// <summary>
        /// количество строк в каждом файле
        /// </summary>
        static ushort fileLineCount;

        /// <summary>
        /// рандомайзер для вида операции
        /// </summary>
        static Random rOperation;

        /// <summary>
        /// рандомайзер для чисел
        /// </summary>
        static Random rPartMathOperation;

        /// <summary>
        /// рандомайзер для множителя 
        /// </summary>
        static Random rMulti;

        /// <summary>
        /// имя файля куда записывается результат
        /// </summary>
        static string resulFileName;

        /// <summary>
        /// поток для записи результата обработки
        /// </summary>
        static StreamWriter resulFileStreamWriter;

        /// <summary>
        /// сообщение лога в консоль
        /// </summary>
        /// <param name="message">текст собщения</param>
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

            Log("Подготовка завершена\nДля выполнения нажмите любую клавишу...");

            Console.ReadKey();

            _ = CalculateData(files);

            Console.ReadKey();
        }

        /// <summary>
        /// обработка файлов с данными
        /// </summary>
        /// <param name="files">коллекция описаний файлов</param>
        static async Task<bool> CalculateData(List<FileInfo> files)
        {
            try
            {
                resulFileStreamWriter = new StreamWriter(resulFileName);

                List<Task> tasks = new List<Task>();

                foreach (var item in files)
                {
                    tasks.Add(ReadAndCalculateFileDataAsync(item));
                }

                await Task.WhenAll(tasks);

                resulFileStreamWriter.Close();

                Log("Обработка завершена...");

                return true;

            }
            catch (Exception e)
            {
                Log($"Что-то пошло не так: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// задача для обработки файла:
        /// обойти все строки
        /// записать результат в общий файл
        /// </summary>
        /// <param name="file">файл</param>
        private static Task ReadAndCalculateFileDataAsync(FileInfo file)
        {
            return Task.Run(() =>
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

                lock (resulFileStreamWriter)
                {
                    resulFileStreamWriter.WriteLine($"Файл: {file.Name} -> {res.ToString()}");
                }
            }
            );
        }

        /// <summary>
        /// инициализация
        /// </summary>
        static void Init()
        {
            Log("Инициализация...");

            filesCount = 200;
            fileLineCount = 2000;

            rOperation = new Random(0);
            rPartMathOperation = new Random();
            rMulti = new Random(100);

            string currentAppDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            filesVaultDir = currentAppDir + @"\Files";
            resulFileName = filesVaultDir + "\\result.dat";
        }

        /// <summary>
        /// подготовка каталога хранилища файлов
        /// </summary>
        /// <returns></returns>
        static bool CreateFilesVault()
        {
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

        /// <summary>
        /// генерация файлов данных в хранилище
        /// </summary>
        /// <param name="files">коллекция описаний файлов</param>
        /// <returns>результат работы</returns>
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