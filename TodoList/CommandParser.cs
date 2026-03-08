using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using TodoList.Exceptions;

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
				{ "search", ParseSearchCommand },
				{ "load", ParseLoadCommand }
			};
		}

		public static ICommand Parse(string inputString)
		{
			if (string.IsNullOrWhiteSpace(inputString))
				throw new InvalidCommandException("Команда не может быть пустой.");

			var parts = SplitArgs(inputString).ToArray();
			if (parts.Length == 0)
				throw new InvalidCommandException("Команда не может быть пустой.");

			var commandName = parts[0].ToLowerInvariant();
			var args = parts.Skip(1).ToArray();

			if (_commandHandlers.TryGetValue(commandName, out var handler))
			{
				return handler(args);
			}

			throw new InvalidCommandException($"Неизвестная команда '{commandName}'. Напишите 'help' для списка команд.");
		}

		private static ICommand ParseAddCommand(string[] args)
		{
			var add = new AddCommand();
			var lowerArgs = args.Select(a => a.ToLowerInvariant()).ToArray();

			if (lowerArgs.Contains("-m") || lowerArgs.Contains("--multiline"))
			{
				add.Multiline = true;
				return add;
			}
			add.Text = string.Join(' ', args);
			return add;
		}

		private static ICommand ParseStatusCommand(string[] args)
		{
			if (args.Length < 2)
				throw new InvalidArgumentException("Использование: status <номер> <новый_статус>");
			if (!int.TryParse(args[0], out int id))
				throw new InvalidArgumentException($"Ошибка: '{args[0]}' не является корректным номером задачи.");
			if (!Enum.TryParse<TodoStatus>(args[1], true, out TodoStatus newStatus))
				throw new InvalidArgumentException($"Ошибка: '{args[1]}' не является корректным статусом.");

			return new StatusCommand { Id = id, NewStatus = newStatus };
		}

		private static ICommand ParseUpdateCommand(string[] args)
		{
			if (args.Length < 2)
				throw new InvalidArgumentException("Использование: update <номер> <новый текст>");
			if (!int.TryParse(args[0], out int id))
				throw new InvalidArgumentException($"Ошибка: '{args[0]}' не является корректным номером задачи.");

			var newText = string.Join(' ', args.Skip(1));
			return new UpdateCommand { Id = id, NewText = newText };
		}

		private static ICommand ParseRemoveCommand(string[] args)
		{
			if (args.Length < 1)
				throw new InvalidArgumentException("Использование: delete <номер>");
			if (!int.TryParse(args[0], out int id))
				throw new InvalidArgumentException($"Ошибка: '{args[0]}' не является корректным номером задачи.");

			return new RemoveCommand { Id = id };
		}

		private static ICommand ParseLoadCommand(string[] args)
		{
			if (args.Length < 2)
				throw new InvalidArgumentException("Использование: load <количество> <размер>");

			if (!int.TryParse(args[0], out int count) || count <= 0)
				throw new InvalidArgumentException("Количество должно быть положительным числом.");

			if (!int.TryParse(args[1], out int size) || size <= 0)
				throw new InvalidArgumentException("Размер должен быть положительным числом.");

			return new LoadCommand { DownloadCount = count, DownloadSize = size };
		}

		private static ICommand ParseSearchCommand(string[] args)
		{
			var cmd = new SearchCommand();
			var knownFlags = new HashSet<string> { "--contains", "--starts-with", "--ends-with", "--from", "--to", "--status", "--sort", "--desc", "--top" };

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i].ToLowerInvariant();

				if (arg.StartsWith("--") && !knownFlags.Contains(arg))
					throw new InvalidArgumentException($"Неизвестный флаг: {arg}");

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
							cmd.FromDate = fromDate;
						else
							throw new InvalidArgumentException("Некорректный формат даты для флага --from.");
						break;
					case "--to":
						if (i + 1 < args.Length && DateTime.TryParse(args[++i], out var toDate))
							cmd.ToDate = toDate;
						else
							throw new InvalidArgumentException("Некорректный формат даты для флага --to.");
						break;
					case "--status":
						if (i + 1 < args.Length && Enum.TryParse<TodoStatus>(args[++i], true, out var status))
							cmd.Status = status;
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
						if (i + 1 < args.Length && int.TryParse(args[++i], out int top))
							cmd.Top = top;
						break;
				}
			}
			return cmd;
		}

		private static ICommand ParseViewCommand(string[] args)
		{
			var view = new ViewCommand();
			var knownFlags = new HashSet<string> { "i", "s", "d", "a", "index", "status", "update-date", "all" };

			var flags = args.Where(p => p.StartsWith("-")).Select(p => p.TrimStart('-')).ToList();

			foreach (var flag in flags)
			{
				if (!knownFlags.Contains(flag) && flag.Length > 1)
					throw new InvalidArgumentException($"Неизвестный флаг: --{flag}");

				foreach (char f in flag)
				{
					if (!knownFlags.Contains(f.ToString()))
						throw new InvalidArgumentException($"Неизвестный флаг: -{f}");
				}
			}

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