using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO; // Убедимся, что using System.IO присутствует
using System.Linq;
using System.Text; // Добавлено для StringBuilder, используется в SplitCsvLine

namespace TodoList
{
    public static class FileManager
    {
        // <<<< ДОБАВЛЕНО: Статические публичные поля для путей <<<<
        public static readonly string DataDirectory = "Data";
        public static readonly string ProfileFilePath = Path.Combine(DataDirectory, "profile.txt");
        public static readonly string TodosFilePath = Path.Combine(DataDirectory, "todo.csv");

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
            string content = $"{profile.Name}\n{profile.YearOfBirth}";
            File.WriteAllText(filePath, content);
            Console.WriteLine($"Профиль сохранен в: {filePath}");
        }

        public static Profile LoadProfile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    string name = lines[0];
                    if (int.TryParse(lines[1], out int yearOfBirth))
                    {
                        Console.WriteLine($"Профиль загружен из: {filePath}");
                        return new Profile(name, yearOfBirth);
                    }
                }
            }
            Console.WriteLine($"Файл профиля не найден или поврежден: {filePath}. Создан профиль по умолчанию.");
            return new Profile();
        }

        public static void SaveTodos(TodoList todos, string filePath)
        {
            var lines = new List<string> { "Index;Text;IsDone;LastUpdate" };
            foreach (var item in todos.GetAllItems())
            {
                string textToSave = item.Text.Replace("\n", "\\n").Replace("\"", "\"\"");
                string formattedText = $"\"{textToSave}\"";
                string formattedDate = item.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                lines.Add($"{item.Id};{formattedText};{item.IsCompleted.ToString().ToLowerInvariant()};{formattedDate}");
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
                        var parts = SplitCsvLine(line, ';');
                        if (parts.Length == 4)
                        {
                            try
                            {
                                int id = int.Parse(parts[0]);
                                string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n");
                                bool isCompleted = bool.Parse(parts[2]);
                                DateTime lastUpdated = DateTime.ParseExact(parts[3], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                                var item = new TodoItem
                                {
                                    Id = id,
                                    Text = text,
                                    IsCompleted = isCompleted,
                                    CreatedAt = DateTime.Now,
                                    LastUpdated = lastUpdated
                                };
                                todoList.AddLoadedItem(item);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка при загрузке задачи: {line}. {ex.Message}");
                            }
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

        private static string[] SplitCsvLine(string line, char separator)
        {
            var parts = new List<string>();
            bool inQuote = false;
            var sb = new StringBuilder();
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
                else if (c == separator && !inQuote)
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