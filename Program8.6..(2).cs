using System;
using System.IO;

class Program
{
    static long GetDirectorySize(string path)
    {
        long size = 0;

        try
        {
            // Получаем список файлов в текущей директории
            string[] files = Directory.GetFiles(path);

            // Добавляем размер всех файлов в директории
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            // Получаем список подпапок в текущей директории
            string[] dirs = Directory.GetDirectories(path);

            // Рекурсивно вызываем эту же функцию для каждой подпапки
            foreach (string dir in dirs)
            {
                size += GetDirectorySize(dir);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }

        return size;
    }

    static void Main(string[] args)
    {
        // Указываем путь к директории
        string path = @"C:\Games";

        // Получаем размер директории в байтах
        long size = GetDirectorySize(path);

        Console.WriteLine("Размер папки {0} составляет: {1} байт", path, size);
        Console.ReadKey();
    }
}