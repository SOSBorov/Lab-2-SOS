using System.Collections.Generic; 

namespace TodoList
{
    public static class AppInfo
    {
        public static TodoList Todos { get; set; }
        public static Profile CurrentProfile { get; set; }
        public static Stack<ICommand> UndoStack { get; set; }
        public static Stack<ICommand> RedoStack { get; set; }
    }
}