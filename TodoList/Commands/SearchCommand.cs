using System;
using System.Linq;
using System.Collections.Generic;

namespace TodoList
{
	public class SearchCommand : ICommand
	{
		public string ContainsText { get; set; } = string.Empty;
		public string StartsWithText { get; set; } = string.Empty;
		public string EndsWithText { get; set; } = string.Empty;
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public TodoStatus? Status { get; set; }
		public string SortBy { get; set; } = string.Empty;
		public bool Desc { get; set; }
		public int? Top { get; set; }

		public void Execute()
		{
			if (AppInfo.CurrentUserTodoList == null)
				throw new AuthenticationException("Вы не авторизованы. Войдите в профиль, чтобы работать с задачами.");

			var items = AppInfo.CurrentUserTodoList.GetAllItems().AsQueryable();

			if (!string.IsNullOrWhiteSpace(ContainsText))
				items = items.Where(i => i.Text.Contains(ContainsText, StringComparison.OrdinalIgnoreCase));

			if (!string.IsNullOrWhiteSpace(StartsWithText))
				items = items.Where(i => i.Text.StartsWith(StartsWithText, StringComparison.OrdinalIgnoreCase));

			if (!string.IsNullOrWhiteSpace(EndsWithText))
				items = items.Where(i => i.Text.EndsWith(EndsWithText, StringComparison.OrdinalIgnoreCase));

			if (FromDate.HasValue)
				items = items.Where(i => i.LastUpdated.Date >= FromDate.Value.Date);

			if (ToDate.HasValue)
				items = items.Where(i => i.LastUpdated.Date <= ToDate.Value.Date);

			if (Status.HasValue)
				items = items.Where(i => i.Status == Status.Value);

			if (!string.IsNullOrWhiteSpace(SortBy))
			{
				if (SortBy.Equals("text", StringComparison.OrdinalIgnoreCase))
				{
					items = Desc
						? items.OrderByDescending(i => i.Text)
						: items.OrderBy(i => i.Text);
				}
				else if (SortBy.Equals("date", StringComparison.OrdinalIgnoreCase))
				{
					items = Desc
						? items.OrderByDescending(i => i.LastUpdated)
						: items.OrderBy(i => i.LastUpdated);
				}
			}

			if (Top.HasValue)
				items = items.Take(Top.Value);

			var result = new TodoList(items.ToList());

			if (result.Count == 0)
			{
				Console.WriteLine("Нет задач, удовлетворяющих условиям поиска.");
				return;
			}

			result.ViewCustom(showIndex: true, showStatus: true, showUpdateDate: true, showAll: false);
		}
	}
}