using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoList
{
    public static class FileManager
    {
        public static readonly string DataDirectory = "Data";
        public static readonly string ProfileFilePath = Path.Combine(DataDirectory, "profile.csv");

        public static void EnsureDataDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Console.WriteLine($"Создана директория: {dirPath}");
            }
        }

        public static void SaveProfiles(List<Profile> profiles, string filePath)
        {
            var lines = new List<string> { "Id;Login;Password;FirstName;LastName;BirthYear" };
            foreach (var profile in profiles)
            {
                lines.Add($"{profile.Id};{profile.Login};{profile.Password};{profile.FirstName};{profile.LastName};{profile.BirthYear}");
            }
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
            Console.WriteLine($"Профили сохранены в: {filePath}");
        }

        public static List<Profile> LoadProfiles(string filePath)
        {
            var profiles = new List<Profile>();
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "Id;Login;Password;FirstName;LastName;BirthYear\n", Encoding.UTF8);
                Console.WriteLine($"Файл профилей не найден: {filePath}. Создан новый файл.");
                return profiles;
            }

            var lines = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';');
                if (parts.Length == 6)
                {
                    try
                    {
                        var profile = new Profile
                        {
                            Id = Guid.Parse(parts[0]),
                            Login = parts[1],
                            Password = parts[2],
                            FirstName = parts[3],
                            LastName = parts[4],
                            BirthYear = int.Parse(parts[5])
                        };
                        profiles.Add(profile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке профиля из строки: '{line}'. {ex.Message}");
                    }
                }
            }
            Console.WriteLine($"Загружено {profiles.Count} профилей из: {filePath}");
            return profiles;
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
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
            Console.WriteLine($"Задачи сохранены в: {filePath}");
        }

        public static TodoList LoadTodos(string filePath)
        {
            var loadedItems = new List<TodoItem>();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1);
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
                            if (statusFromFile.Equals("true", StringComparison.OrdinalIgnoreCase)) status = TodoStatus.Completed;
                            else if (statusFromFile.Equals("false", StringComparison.OrdinalIgnoreCase)) status = TodoStatus.NotStarted;
                            else status = Enum.Parse<TodoStatus>(statusFromFile, true);

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
                Console.WriteLine($"Файл задач не найден: {filePath}. Будет создан новый список задач.");
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
