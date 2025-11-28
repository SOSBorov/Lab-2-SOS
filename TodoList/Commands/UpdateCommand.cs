namespace TodoList
{
    public class UpdateCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; } 
        public string NewText { get; set; } = "";
        public string? TodosFilePath { get; set; }

        public void Execute()
        {
            if (TodoList == null || TodosFilePath == null) return;

            TodoList.Update(Index, NewText); 
            FileManager.SaveTodos(TodoList, TodosFilePath);
        }
    }
}