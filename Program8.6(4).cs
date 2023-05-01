using System;
using System.IO;
using System.Collections.Generic;

namespace FinalTask
{
    class Student
    {
        public string Name;
        public string Group;
        public DateTime DateOfBirth;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Получаем путь к файлу из аргументов командной строки
            string inputPath = @"";
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: FinalTask.exe <input file>");
                return;
            }
           

            // Проверяем, что файл существует
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Input file does not exist: {0}", inputPath);
                return;
            }

            // Создаем директорию Students на рабочем столе
            string studentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");
            Directory.CreateDirectory(studentsDir);

            // Читаем данные из файла и конвертируем их в список студентов
            List<Student> students = new List<Student>();
            using (BinaryReader reader = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    Student student = new Student();
                    student.Name = reader.ReadString();
                    student.Group = reader.ReadString();
                    student.DateOfBirth = new DateTime(reader.ReadInt64());
                    students.Add(student);
                }
            }

            // Группируем студентов по группам
            Dictionary<string, List<Student>> groups = new Dictionary<string, List<Student>>();
            foreach (Student student in students)
            {
                if (!groups.ContainsKey(student.Group))
                {
                    groups.Add(student.Group, new List<Student>());
                }
                groups[student.Group].Add(student);
            }

            // Записываем данные в файлы групп
            foreach (var group in groups)
            {
                string groupFile = Path.Combine(studentsDir, group.Key + ".txt");
                using (StreamWriter writer = new StreamWriter(groupFile))
                {
                    foreach (Student student in group.Value)
                    {
                        writer.WriteLine("{0}, {1}", student.Name, student.DateOfBirth.ToString("dd.MM.yyyy"));
                    }
                }
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}