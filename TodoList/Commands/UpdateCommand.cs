namespace TodoList
{
    public class UpdateCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Id { get; set; }
        public string NewText { get; set; } = "";

        public void Execute()
        {
            TodoList?.Update(Id, NewText);
        }
    }
}
