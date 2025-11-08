using System;
using System.Linq;
using System.Text;
using System.IO;

namespace TodoList
{
    public static class CommandParser
    {

        public static ICommand? Parse(string inputString, TodoList todoList, Profile profile)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return null;

            var parts = SplitArgs(inputString).ToArray();
            if (parts.Length == 0) return null;

            var lower = parts.Select(p => p.ToLowerInvariant()).ToArray();
            var cmd = lower[0];

            switch (cmd)
            {
                case "help":
                    return new HelpCommand();

                case "add":
                    {
                        var add = new AddCommand { TodoList = todoList, TodosFilePath = FileManager.TodosFilePath };
                        if (lower.Contains("-m") || lower.Contains("--multiline"))
                        {
                            add.Multiline = true;
                            return add;
                        }
                        add.Text = string.Join(' ', parts.Skip(1));
                        return add;
                    }

                case "view":
                    {
                        var view = new ViewCommand { TodoList = todoList };
                        var flags = parts.Skip(1)
                                         .Where(p => p.StartsWith("-"))
                                         .Select(p => p.TrimStart('-'))
                                         .ToArray();

                        foreach (var flag in flags)
                        {
                            foreach (char f in flag)
                            {
                                switch (f)
                                {
                                    case 'i': view.ShowIndex = true; break;
                                    case 's': view.ShowStatus = true; break;
                                    case 'd': view.ShowUpdateDate = true; break;
                                    case 'a': view.ShowAll = true; break;
                                }
                            }
                            if (flag == "index") view.ShowIndex = true;
                            if (flag == "status") view.ShowStatus = true;
                            if (flag == "update-date") view.ShowUpdateDate = true;
                            if (flag == "all") view.ShowAll = true;
                        }
                        return view;
                    }

                case "done":
                    {
                        var done = new CompleteCommand { TodoList = todoList, TodosFilePath = FileManager.TodosFilePath };
                        if (parts.Length > 1 && int.TryParse(parts[1], out int id))
                            done.Id = id;
                        else
                            Console.WriteLine("Использование: done <номер задачи>");
                        return done;
                    }

                case "update":
                    {
                        var up = new UpdateCommand { TodoList = todoList, TodosFilePath = FileManager.TodosFilePath };
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Использование: update <номер> <новый текст>");
                            return up;
                        }
                        if (int.TryParse(parts[1], out int id))
                            up.Id = id;
                        up.NewText = string.Join(' ', parts.Skip(2));
                        return up;
                    }

                case "delete":
                case "remove":
                case "rm":
                    {
                        var rm = new RemoveCommand { TodoList = todoList, TodosFilePath = FileManager.TodosFilePath };
                        if (parts.Length > 1 && int.TryParse(parts[1], out int id))
                            rm.Id = id;
                        else
                            Console.WriteLine("Использование: delete <номер>");
                        return rm;
                    }

                case "profile":
                    return new ProfileCommand { Profile = profile, ProfileFilePath = FileManager.ProfileFilePath };

                default:
                    Console.WriteLine("Неизвестная команда. Напишите 'help' для списка команд.");
                    return null;
            }
        }

        private static System.Collections.Generic.IEnumerable<string> SplitArgs(string input)
        {
            bool inQuotes = false;
            var current = new StringBuilder();
            foreach (char c in input)
            {
                if (c == '"') { inQuotes = !inQuotes; continue; }
                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        yield return current.ToString();
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            if (current.Length > 0)
                yield return current.ToString();
        }
    }
}