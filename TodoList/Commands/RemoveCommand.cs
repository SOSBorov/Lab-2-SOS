using System;

namespace TodoList
{
    public class RemoveCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Id { get; set; }
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null)
                throw new InvalidOperationException("TodoList не установлен");
            if (TodosFilePath == null)
                throw new InvalidOperationException("Путь к файлу задач не установлен");

            var itemToRemove = TodoList.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (itemToRemove != null)
            {
                TodoList.Remove(Id);
                FileManager.SaveTodos(TodoList, TodosFilePath);
            }

        }
    }
}