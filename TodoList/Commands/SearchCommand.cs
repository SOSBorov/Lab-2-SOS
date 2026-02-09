using System;

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
        }

        public void Unexecute() { }
    }
}
