using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList
{
	public class LoadCommand : ICommand
	{
		public int DownloadCount { get; set; }
		public int DownloadSize { get; set; }

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
				var progress = new int[DownloadCount];
				var random = new Random();
				var tasks = new List<Task>();

				for (int i = 0; i < DownloadCount; i++)
				{
					tasks.Add(DownloadAsync(i, progress, random));
				}

				var allDownloadsTask = Task.WhenAll(tasks);

				while (!allDownloadsTask.IsCompleted)
				{
					DrawAllProgressBars(progress, startRow);
					await Task.Delay(100);
				}

				await allDownloadsTask;

				DrawAllProgressBars(progress, startRow);

				Console.SetCursorPosition(0, startRow + DownloadCount);
				Console.WriteLine("\nВсе загрузки завершены.");
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}

		private async Task DownloadAsync(int index, int[] progressArray, Random random)
		{
			for (int p = 0; p <= DownloadSize; p++)
			{
				progressArray[index] = p;
				await Task.Delay(random.Next(20, 100));
			}
		}

		private void DrawAllProgressBars(int[] progress, int startRow)
		{
			for (int i = 0; i < DownloadCount; i++)
			{
				Console.SetCursorPosition(0, startRow + i);
				DrawProgressBar(i + 1, progress[i], DownloadSize);
			}
		}

		private void DrawProgressBar(int index, int current, int total)
		{
			const int barWidth = 50;
			double percentage = total > 0 ? (double)current / total : 0;
			int filledChars = (int)(percentage * barWidth);

			string bar = $"[{new string('█', filledChars)}{new string('-', barWidth - filledChars)}]";

			Console.Write($"Загрузка {index,-2}: {bar} {current,3}/{total} ({percentage:P0}) ".PadRight(Console.WindowWidth - 1));
		}
	}
}