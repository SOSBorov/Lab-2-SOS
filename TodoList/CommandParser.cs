using System.Text;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace TodoList
{
    public static class CommandParser
    {
        private static readonly Dictionary<string, Func<string[], ICommand>> _commandHandlers;

        static CommandParser()
        {
            _commandHandlers = new Dictionary<string, Func<string[], ICommand>>
            {
                { "help", args => new HelpCommand() },
                { "add", ParseAddCommand },
                { "view", ParseViewCommand },
                { "status", ParseStatusCommand },
                { "update", ParseUpdateCommand },
                { "delete", ParseRemoveCommand },
                { "remove", ParseRemoveCommand },
                { "rm", ParseRemoveCommand },
                { "profile", ParseProfileCommand },
                { "undo", args => new UndoCommand() },
                { "redo", args => new RedoCommand() },
                { "search", ParseSearchCommand }
            };
        }

        public static ICommand? Parse(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return null;

            var parts = SplitArgs(inputString).ToArray();
            if (parts.Length == 0) return null;

            var commandName = parts[0].ToLowerInvariant();
            var args = parts.Skip(1).ToArray();

            if (_commandHandlers.TryGetValue(commandName, out var handler))
            {
                return handler(args);
            }

            Console.WriteLine("Неизвестная командa. Напишите 'help' для списка команд.");
            return null;
        }

        private static ICommand ParseSearchCommand(string[] args)
        {
            return new SearchCommand();
        }

        private static ICommand ParseAddCommand(string[] args)
        {
            var add = new AddCommand { TodosFilePath = AppInfo.CurrentUserTodosFilePath };
            var lowerArgs = args.Select(a => a.ToLowerInvariant()).ToArray();

            if (lowerArgs.Contains("-m") || lowerArgs.Contains("--multiline"))
            {
                add.Multiline = true;
                return add;
            }
            add.Text = string.Join(' ', args);
            return add;
        }

        private static ICommand ParseViewCommand(string[] args)
        {
            var view = new ViewCommand();
            var flags = args.Where(p => p.StartsWith("-")).Select(p => p.TrimStart('-')).ToArray();
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

        private static ICommand ParseStatusCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Использование: status <номер> <новый_статус>");
                return null;
            }
            if (!int.TryParse(args[0], out int id))
            {
                Console.WriteLine($"Ошибка: '{args[0]}' не является корректным номером задачи.");
                return null;
            }
            if (!Enum.TryParse<TodoStatus>(args[1], true, out TodoStatus newStatus))
            {
                Console.WriteLine($"Ошибка: '{args[1]}' не является корректным статусом.");
                return null;
            }
            return new StatusCommand { Id = id, NewStatus = newStatus, TodosFilePath = AppInfo.CurrentUserTodosFilePath };
        }

        private static ICommand ParseUpdateCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Использование: update <номер> <новый текст>");
                return null;
            }
            if (!int.TryParse(args[0], out int id))
            {
                Console.WriteLine($"Ошибка: '{args[0]}' не является корректным номером задачи.");
                return null;
            }
            var newText = string.Join(' ', args.Skip(1));
            return new UpdateCommand { Id = id, NewText = newText, TodosFilePath = AppInfo.CurrentUserTodosFilePath };
        }

        private static ICommand ParseRemoveCommand(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Использование: delete <номер>");
                return null;
            }
            if (!int.TryParse(args[0], out int id))
            {
                Console.WriteLine($"Ошибка: '{args[0]}' не является корректным номером задачи.");
                return null;
            }
            return new RemoveCommand { Id = id, TodosFilePath = AppInfo.CurrentUserTodosFilePath };
        }

        private static ICommand ParseProfileCommand(string[] args)
        {
            var profileCmd = new ProfileCommand();
            var flags = args.Select(a => a.ToLowerInvariant()).ToArray();

            if (flags.Contains("-o") || flags.Contains("--out"))
            {
                profileCmd.IsLogout = true;
            }
            else if (flags.Contains("-s") || flags.Contains("--switch"))
            {
                profileCmd.IsSwitching = true;
            }
            return profileCmd;
        }

        private static IEnumerable<string> SplitArgs(string input)
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
