using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions; // Добавлено для Regex

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
            var lines = new List<string> { "Index;Text;IsDone;LastUpdate" }; // Новый заголовок CSV
            foreach (var item in todos.GetAllItems())
            {
                // Подготовка текста: замена \n на \\n и экранирование кавычек, затем обрамление в кавычки
                string textToSave = item.Text.Replace("\n", "\\n").Replace("\"", "\"\"");
                string formattedText = $"\"{textToSave}\"";

                // Форматирование даты: как в примере "2025-08-21T14:30:00"
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
                    foreach (var line in lines.Skip(1)) // Пропускаем заголовок
                    {
                        var parts = SplitCsvLine(line, ';'); // Используем точку с запятой как разделитель
                        if (parts.Length == 4) // Теперь 4 столбца
                        {
                            try
                            {
                                int id = int.Parse(parts[0]);
                                // Де-экранирование: замена \\n обратно на \n и " на ""
                                string text = parts[1].Replace("\"\"", "\"").Replace("\\n", "\n");
                                bool isCompleted = bool.Parse(parts[2]);
                                // Парсинг даты в указанном формате
                                DateTime lastUpdated = DateTime.ParseExact(parts[3], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                                var item = new TodoItem
                                {
                                    Id = id,
                                    Text = text,
                                    IsCompleted = isCompleted,
                                    // CreatedAt при загрузке не восстанавливается из файла,
                                    // так как его нет в CSV. Если нужно, добавьте его в CSV.
                                    // Пока оставляем как есть, будет присвоено DateTime.Now по умолчанию.
                                    LastUpdated = lastUpdated
                                };
                                todoList.AddLoadedItem(item);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine($"Ошибка формата при загрузке задачи: {line}. {ex.Message}");
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                Console.WriteLine($"Ошибка структуры при загрузке задачи: {line}. {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Пропущена строка из-за некорректного количества столбцов: {line}");
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

        // Обновленный вспомогательный метод для корректного парсинга CSV-строки с учетом кавычек и разделителя
        private static string[] SplitCsvLine(string line, char separator)
        {
            var parts = new List<string>();
            bool inQuote = false;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    // Проверка на экранированную кавычку (дважды подряд)
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++; // Пропускаем вторую кавычку
                    }
                    else
                    {
                        inQuote = !inQuote; // Вход/выход из кавычек
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
            parts.Add(sb.ToString()); // Добавляем последнюю часть
            return parts.ToArray();
        }
    }
}