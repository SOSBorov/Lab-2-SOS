using System;
using System.Linq;
using System.Collections.Generic;

namespace TodoList
{
    public class SearchCommand : ICommand
    {
        public string ContainsText { get; set; }
        public string StartsWithText { get; set; }
        public string EndsWithText { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public TodoStatus? Status { get; set; }
        public string SortBy { get; set; } 
        public bool Desc { get; set; }
        public int? Top { get; set; }

        public void Execute()
        {
            if (AppInfo.CurrentUserTodoList == null)
            {
                Console.WriteLine("Список задач не найден.");
                return;
            }

            var items = AppInfo.CurrentUserTodoList
                .GetAllItems()
                .AsQueryable();

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
                if (SortBy == "text")
                {
                    items = Desc
                        ? items.OrderByDescending(i => i.Text)
                        : items.OrderBy(i => i.Text);
                }
                else if (SortBy == "date")
                {
                    items = Desc
                        ? items.OrderByDescending(i => i.LastUpdated)
                        : items.OrderBy(i => i.LastUpdated);
                }
            }

            if (Top.HasValue)
                items = items.Take(Top.Value);

            var result = items.ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("Нет задач, удовлетворяющих условиям поиска.");
                return;
            }

            Console.WriteLine("Index | Text                           | Status      | LastUpdate");
            Console.WriteLine("---------------------------------------------------------------------");

            foreach (var item in result)
            {
                string shortText = item.Text.Length > 30
                    ? item.Text.Substring(0, 30) + "..."
                    : item.Text;

                Console.WriteLine(
                    $"{item.Id,-5} | {shortText,-30} | {item.Status,-10} | {item.LastUpdated:yyyy-MM-dd}");
            }
        }

        public void Unexecute() { }
    }
}
