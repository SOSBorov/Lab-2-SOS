using System;

namespace TodoList
{
    public class ViewCommand : ICommand
    {
        public TodoList? TodoList { get; set; }

        public bool ShowIndex { get; set; }
        public bool ShowStatus { get; set; }
        public bool ShowUpdateDate { get; set; }
        public bool ShowAll { get; set; }

        public void Execute()
        {
            if (TodoList == null)
            {
                Console.WriteLine("Список задач не найден.");
                return;
            }

            TodoList.ViewCustom(ShowIndex, ShowStatus, ShowUpdateDate, ShowAll);
        }
    }
}
