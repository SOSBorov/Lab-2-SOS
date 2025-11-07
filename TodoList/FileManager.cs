using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TodoList
{
    public static class FileManager
    {
        public static void EnsureDataDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Console.WriteLine($"Создана директория: {dirPath}");
            }
        }

        public static void SaveProfile(Profile profile, string filePath)
        {
            File.WriteAllText(filePath, profile.Name);
            Console.WriteLine($"Профиль сохранен в: {filePath}");
        }

        public static Profile LoadProfile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string name = File.ReadAllText(filePath);
                Console.WriteLine($"Профиль загружен из: {filePath}");
                return new Profile(name);
            }
            Console.WriteLine($"Файл профиля не найден: {filePath}. Создан профиль по умолчанию.");
            return new Profile();
        }

        public static void SaveTodos(TodoList todos, string filePath)
        {
            var lines = new List<string> { "Id,Text,IsCompleted,CreatedAt,LastUpdated" }; 
            foreach (var item in todos.GetAllItems())
            {
       
                lines.Add($"{item.Id},\"{item.Text.Replace("\"", "\"\"")}\",{item.IsCompleted},{item.CreatedAt.ToString("o", CultureInfo.InvariantCulture)},{item.LastUpdated.ToString("o", CultureInfo.InvariantCulture)}");
            }
            File.WriteAllLines(filePath, lines);
            Console.WriteLine($"Задачи сохранены в: {filePath}");
        }

        public static TodoList LoadTodos(string filePath)
        {
            var todoList = new TodoList();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length > 0) 
                {
                    foreach (var line in lines.Skip(1)) 
                    {
                        var parts = SplitCsvLine(line);
                        if (parts.Length == 5)
                        {
                            var item = new TodoItem
                            {
                                Id = int.Parse(parts[0]),
                                Text = parts[1].Replace("\"\"", "\""), 
                                IsCompleted = bool.Parse(parts[2]),
                                CreatedAt = DateTime.Parse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                                LastUpdated = DateTime.Parse(parts[4], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                            };
                            todoList.AddLoadedItem(item);
                        }
                    }
                }
                Console.WriteLine($"Задачи загружены из: {filePath}");
            }
            else
            {
                Console.WriteLine($"Файл задач не найден: {filePath}. Список задач пуст.");
            }
            return todoList;
        }

       
        private static string[] SplitCsvLine(string line)
        {
            var parts = new List<string>();
            bool inQuote = false;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
               
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuote = !inQuote; 
                    }
                }
                else if (c == ',' && !inQuote)
                {
                    parts.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            parts.Add(sb.ToString());
            return parts.ToArray();
        }
    }
}