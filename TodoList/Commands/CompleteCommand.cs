namespace TodoList
{
    public class CompleteCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Id { get; set; }

        public void Execute()
        {
            TodoList?.MarkComplete(Id);
        }
    }
}
