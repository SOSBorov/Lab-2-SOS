using System;

namespace TodoList
{
    public class StatusCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; } 
        public TodoStatus NewStatus { get; set; }
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null || TodosFilePath == null) return;

            TodoList.SetStatus(Index, NewStatus); 
            FileManager.SaveTodos(TodoList, TodosFilePath);
        }
    }
}
