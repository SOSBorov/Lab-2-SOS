using System;
using System.Linq;

namespace TodoList
{
    public class StatusCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Id { get; set; }
        public TodoStatus NewStatus { get; set; } 
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null)
                throw new InvalidOperationException("TodoList не установлен");
            if (TodosFilePath == null)
                throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToUpdate = TodoList.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (itemToUpdate != null)
            {
                TodoList.ChangeStatus(Id, NewStatus);
                FileManager.SaveTodos(TodoList, TodosFilePath);
            }
        }
    }
}