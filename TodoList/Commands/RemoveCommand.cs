namespace TodoList
{
    public class RemoveCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; } 
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null || TodosFilePath == null) return;

            TodoList.Remove(Index); 
            FileManager.SaveTodos(TodoList, TodosFilePath);
        }
    }
}
