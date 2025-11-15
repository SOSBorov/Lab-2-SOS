using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TodoList
{
    public static class FileManager
    {
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
            var lines = new List<string> { "Index;Text;Status;LastUpdate" };
            foreach (var item in todos.GetAllItems())
            {
                string textToSave = item.Text.Replace("\r", "").Replace("\n", "\\n").Replace("\"", "\"\"");
                string formattedText = $"\"{textToSave}\"";
                string statusText = item.Status.ToString();
                string formattedDate = item.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                lines.Add($"{item.Id};{formattedText};{statusText};{formattedDate}");
            }
            File.WriteAllLines(filePath, lines);
            Console.WriteLine($"Задачи сохранены в: {filePath}");
        }

        public static TodoList LoadTodos(string filePath)
        {
            var loadedItems = new List<TodoItem>();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath).Skip(1);
                foreach (var line in lines)
                {
                    var parts = SplitCsvLine(line, ';');
                    if (parts.Length == 4)
                    {
                        try
                        {
                            int id = int.Parse(parts[0]);
                            string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n");
                            string statusFromFile = parts[2];
                            DateTime lastUpdated = DateTime.ParseExact(parts[3], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                            TodoStatus status;

                            if (statusFromFile.Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                status = TodoStatus.Completed;
                            }
                            else if (statusFromFile.Equals("false", StringComparison.OrdinalIgnoreCase))
                            {
                                status = TodoStatus.NotStarted;
                            }
                            else
                            {
                                status = Enum.Parse<TodoStatus>(statusFromFile, true);
                            }

                            var item = new TodoItem
                            {
                                Id = id,
                                Text = text,
                                Status = status,
                                CreatedAt = DateTime.Now,
                                LastUpdated = lastUpdated
                            };
                            loadedItems.Add(item);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при загрузке задачи: {line}. {ex.Message}");
                        }
                    }
                }
                Console.WriteLine($"Задачи загружены из: {filePath}");
            }
            else
            {
                Console.WriteLine($"Файл задач не найден: {filePath}. Список задач пуст.");
            }

            return new TodoList(loadedItems);
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
                    if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else { inQuote = !inQuote; }
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