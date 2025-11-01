namespace TodoList
{
    public class RemoveCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Id { get; set; }

        public void Execute()
        {
            TodoList?.Remove(Id);
        }
    }
}
