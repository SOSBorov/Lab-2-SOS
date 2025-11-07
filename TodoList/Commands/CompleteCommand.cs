using System;

namespace TodoList
{
    public class CompleteCommand : ICommand
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
            var itemToComplete = TodoList.GetAllItems().FirstOrDefault(x => x.Id == Id);

            if (itemToComplete != null)
            {
                TodoList.MarkComplete(Id);
                FileManager.SaveTodos(TodoList, TodosFilePath);
            }
        }
    }
}