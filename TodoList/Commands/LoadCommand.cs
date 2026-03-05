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
					const int barWidth = 20;
					int filledChars = (int)(percentage * barWidth);
					string bar = $"[{new string('#', filledChars)}{new string('-', barWidth - filledChars)}]";
					string line = $"Загрузка {index + 1,-2}: {bar} {(int)(percentage * 100),3}%";

					Console.SetCursorPosition(0, startRow + index);
					Console.Write(line.PadRight(Console.WindowWidth - 1));
				}

				await Task.Delay(random.Next(20, 100));
			}
		}
	}
}