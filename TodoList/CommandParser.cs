using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace TodoList
{
	public static class CommandParser
	{
		private static readonly Dictionary<string, Func<string[], ICommand?>> _commandHandlers;

		static CommandParser()
		{
			_commandHandlers = new Dictionary<string, Func<string[], ICommand?>>
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

			Console.WriteLine("Неизвестная команда. Напишите 'help' для списка команд.");
			return null;
		}

		private static ICommand? ParseSearchCommand(string[] args)
		{
			var cmd = new SearchCommand();
			bool hasError = false;

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i].ToLowerInvariant();

				switch (arg)
				{
					case "--contains":
						if (i + 1 < args.Length) cmd.ContainsText = args[++i];
						break;

					case "--starts-with":
						if (i + 1 < args.Length) cmd.StartsWithText = args[++i];
						break;

					case "--ends-with":
						if (i + 1 < args.Length) cmd.EndsWithText = args[++i];
						break;

					case "--from":
						if (i + 1 < args.Length && DateTime.TryParse(args[++i], out var fromDate))
						{
							cmd.FromDate = fromDate;
						}
						else
						{
							Console.WriteLine($"Ошибка: некорректный формат даты для флага --from. Используйте формат, понятный вашей системе (например, YYYY-MM-DD).");
							hasError = true;
						}
						break;

					case "--to":
						if (i + 1 < args.Length && DateTime.TryParse(args[++i], out var toDate))
						{
							cmd.ToDate = toDate;
						}
						else
						{
							Console.WriteLine($"Ошибка: некорректный формат даты для флага --to. Используйте формат, понятный вашей системе (например, YYYY-MM-DD).");
							hasError = true;
						}
						break;

					case "--status":
						if (i + 1 < args.Length && Enum.TryParse<TodoStatus>(args[++i], true, out var status))
						{
							cmd.Status = status;
						}
						else
						{
							Console.WriteLine($"Ошибка: некорректный статус. Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed.");
							hasError = true;
						}
						break;

					case "--sort":
						if (i + 1 < args.Length)
						{
							string sort = args[++i].ToLowerInvariant();
							if (sort == "text" || sort == "date")
								cmd.SortBy = sort;
						}
						break;

					case "--desc":
						cmd.Desc = true;
						break;

					case "--top":
						if (i + 1 < args.Length && int.TryParse(args[++i], out int top) && top > 0)
						{
							cmd.Top = top;
						}
						else
						{
							Console.WriteLine($"Ошибка: значение для --top должно быть положительным числом.");
							hasError = true;
						}
						break;
				}
				if (hasError) return null;
			}

			return cmd;
		}

		private static ICommand? ParseAddCommand(string[] args)
		{
			var add = new AddCommand { TodosFilePath = AppInfo.CurrentUserTodosFilePath };
			var lowerArgs = args.Select(a => a.ToLowerInvariant()).ToArray();

			if (lowerArgs.Contains("-m") || lowerArgs.Contains("--multiline"))
			{
				add.Multiline = true;
				return add;
			}

			var text = string.Join(' ', args);
			if (string.IsNullOrWhiteSpace(text))
			{
				Console.WriteLine("Ошибка: текст задачи не может быть пустым.");
				return null;
			}
			add.Text = text;
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

		private static ICommand? ParseStatusCommand(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Использование: status <номер_задачи> <новый_статус>");
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
				Console.WriteLine("Доступные статусы: NotStarted, InProgress, Completed, Postponed, Failed");
				return null;
			}
			return new StatusCommand { Id = id, NewStatus = newStatus, TodosFilePath = AppInfo.CurrentUserTodosFilePath };
		}

		private static ICommand? ParseUpdateCommand(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Использование: update <номер_задачи> <новый текст>");
				return null;
			}
			if (!int.TryParse(args[0], out int id))
			{
				Console.WriteLine($"Ошибка: '{args[0]}' не является корректным номером задачи.");
				return null;
			}
			var newText = string.Join(' ', args.Skip(1));
			if (string.IsNullOrWhiteSpace(newText))
			{
				Console.WriteLine("Ошибка: новый текст задачи не может быть пустым.");
				return null;
			}
			return new UpdateCommand { Id = id, NewText = newText, TodosFilePath = AppInfo.CurrentUserTodosFilePath };
		}

		private static ICommand? ParseRemoveCommand(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Использование: delete <номер_задачи>");
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