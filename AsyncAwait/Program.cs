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
        static readonly ushort filesCount = 1;
        static readonly ushort fileLineCount = 20;
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

            if (!(GenerateFiles()))
            {
                Console.ReadKey();
                return;
            }

            Log("Подготовка завершена\n Для выполнения процедуры расчета нажмите любую клавишу...");

            Console.ReadKey();



        }

        static void Init()
        {
            Log("Инициализация...");

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

        static bool GenerateFiles()
        {
            try
            {
                Log("Генерация файлов данных...");

                for (ushort i = 0; i <= filesCount; i++)
                {
                    string filename = filesVaultDir + $"\\{i}.txt";
                    using (StreamWriter sWriter = new StreamWriter(filename))
                    {
                        for (ushort j = 1; j <= fileLineCount; j++)
                        {
                            sWriter.WriteLine($"{rOperation.Next(1, 3)}/{rPartMathOperation.NextDouble() * rMulti.Next(1, 1000000)}/{rPartMathOperation.NextDouble() * rMulti.Next(1, 1000000)}");
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
    }
}