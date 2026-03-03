using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoList.Exceptions;

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
			if (DownloadCount <= 0 || DownloadSize <= 0)
			{
				throw new InvalidArgumentException("Количество и размер скачиваний должны быть положительными числами.");
			}

			Console.Clear();
			Console.CursorVisible = false;

			try
			{
				var progress = new int[DownloadCount];
				var tasks = new List<Task>();
				var random = new Random();

				for (int i = 0; i < DownloadCount; i++)
				{
					int downloadIndex = i; 
					tasks.Add(Task.Run(() =>
					{
						for (int p = 0; p <= DownloadSize; p++)
						{
							progress[downloadIndex] = p;
							Thread.Sleep(random.Next(20, 100));
						}
					}));
				}

				while (tasks.Any(t => !t.IsCompleted))
				{
					DrawAllProgressBars(progress);
					await Task.Delay(100); 
				}

				DrawAllProgressBars(progress);

				Console.SetCursorPosition(0, DownloadCount);
				Console.WriteLine("\nВсе загрузки завершены.");
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}

		private void DrawAllProgressBars(int[] progress)
		{
			Console.SetCursorPosition(0, 0);

			for (int i = 0; i < DownloadCount; i++)
			{
				DrawProgressBar(i + 1, progress[i], DownloadSize);
			}
		}

		private void DrawProgressBar(int index, int current, int total)
		{
			const int barWidth = 50;
			double percentage = total > 0 ? (double)current / total : 0;
			int filledChars = (int)(percentage * barWidth);

			string bar = $"[{new string('█', filledChars)}{new string('-', barWidth - filledChars)}]";

			Console.WriteLine($"Загрузка {index,-2}: {bar} {current,3}/{total} ({percentage:P0}) ".PadRight(Console.WindowWidth - 1));
		}
	}
}