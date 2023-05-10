using System;
using System.IO;

namespace FolderCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Проверяем, что передан аргумент с путём до папки
            if (args.Length == 0)
            {
                Console.WriteLine("Не задан путь до папки");
                return;
            }

            string folderPath = args[0];

            // Проверяем, что папка существует
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Папка {folderPath} не найдена");
                return;
            }

            try
            {
                DirectoryInfo folder = new DirectoryInfo(folderPath);

                // Получаем текущую дату-время
                DateTime now = DateTime.Now;

                // Получаем список файлов и папок в папке
                FileSystemInfo[] files = folder.GetFileSystemInfos();

                long sizeBeforeCleaning = FolderSize(folderPath);

                int filesDeleted = 0;
                long spaceFreed = 0;

                foreach (FileSystemInfo file in files)
                {
                    // Если это папка, рекурсивно вызываем для неё ту же самую функцию
                    if (file.GetType() == typeof(DirectoryInfo))
                    {
                        filesDeleted += CleanDirectory((DirectoryInfo)file, now, ref spaceFreed);
                    }
                    // Если это файл, проверяем время последнего доступа
                    else if (file.GetType() == typeof(FileInfo))
                    {
                        FileInfo currentFile = (FileInfo)file;

                        if (now.Subtract(currentFile.LastAccessTime).TotalMinutes > 30)
                        {
                            spaceFreed += currentFile.Length;
                            currentFile.Delete();
                            filesDeleted++;
                        }
                    }
                }

                long sizeAfterCleaning = FolderSize(folderPath);

                Console.WriteLine($"Очистка завершена. Удалено {filesDeleted} файлов, освобождено {spaceFreed} байт");
                Console.WriteLine($"Размер папки до очистки: {sizeBeforeCleaning} байт");
                Console.WriteLine($"Размер папки после очистки: {sizeAfterCleaning} байт");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Отказано в доступе к файлам на чтение или запись");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Слишком длинный путь до файла или папки");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Указан некорректный путь до файла или папки");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Ошибка ввода-вывода: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        // Рекурсивная функция очистки папки
        static int CleanDirectory(DirectoryInfo folder, DateTime now, ref long spaceFreed)
        {
            int filesDeleted = 0;

            try
            {
                // Получаем список файлов и папок в папке
                FileSystemInfo[] files = folder.GetFileSystemInfos();

                foreach (FileSystemInfo file in files)
                {
                    // Если это папка, рекурсивно вызываем для неё ту же самую функцию
                    if (file.GetType() == typeof(DirectoryInfo))
                    {
                        filesDeleted += CleanDirectory((DirectoryInfo)file, now, ref spaceFreed);
                    }
                    // Если это файл, проверяем время последнего дост else if (file.GetType() == typeof(FileInfo))
                    {
                        FileInfo currentFile = (FileInfo)file;

                        if (now.Subtract(currentFile.LastAccessTime).TotalMinutes > 30)
                        {
                            spaceFreed += currentFile.Length;
                            currentFile.Delete();
                            filesDeleted++;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Отказано в доступе к файлам на чтение или запись в папке {folder.FullName}");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine($"Слишком длинный путь до файла или папки в папке {folder.FullName}");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Указан некорректный путь до файла или папки в папке {folder.FullName}");
            }
            catch (IOException e)
            {
                Console.WriteLine($"Ошибка ввода-вывода: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }

            return filesDeleted;
        }

        // Метод для подсчёта размера папки
        static long FolderSize(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            long size = 0;

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            return size;
        }
    }
}