using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList
{
	public class LoadCommand : ICommand
	{
		public int DownloadCount { get; set; }
		public int DownloadSize { get; set; }

		private static readonly object _consoleLock = new();

		public void Execute()
		{
			RunAsync().Wait();
		}

		private async Task RunAsync()
		{
			Console.CursorVisible = false;
			int startRow = Console.CursorTop;

			for (int i = 0; i < DownloadCount; i++)
			{
				Console.WriteLine();
			}

			try
			{
				var tasks = new List<Task>();
				var random = new Random();

				for (int i = 0; i < DownloadCount; i++)
				{
					tasks.Add(DownloadAsync(i, startRow, random));
				}

				await Task.WhenAll(tasks);

				Console.SetCursorPosition(0, startRow + DownloadCount);
				Console.WriteLine("\nВсе загрузки завершены.");
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}

		private async Task DownloadAsync(int index, int startRow, Random random)
		{
			for (int current = 0; current <= DownloadSize; current++)
			{
				double percentage = DownloadSize > 0 ? (double)current / DownloadSize : 0;

				lock (_consoleLock)
				{
					// --- НАЧАЛО ИЗМЕНЕНИЙ ---

					// 1. Устанавливаем ширину бара в 20 делений
					const int barWidth = 20;

					// 2. Логика расчета заполненной части остается прежней, но с новой шириной
					int filledChars = (int)(percentage * barWidth);

					// 3. Создаем бар, используя символы '#' и '-'
					string bar = $"[{new string('#', filledChars)}{new string('-', barWidth - filledChars)}]";

					// 4. Формируем строку вывода в новом формате
					string line = $"Загрузка {index + 1,-2}: {bar} {(int)(percentage * 100),3}%";

					// Устанавливаем курсор и выводим отформатированную строку
					Console.SetCursorPosition(0, startRow + index);
					Console.Write(line.PadRight(Console.WindowWidth - 1));

					// --- КОНЕЦ ИЗМЕНЕНИЙ ---
				}

				await Task.Delay(random.Next(20, 100));
			}
		}
	}
}