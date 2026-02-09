using System;

namespace TodoList
{
    public class SearchCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.CurrentUserTodoList == null)
            {
                return;
            }
        }

        public void Unexecute() { }
    }
}
