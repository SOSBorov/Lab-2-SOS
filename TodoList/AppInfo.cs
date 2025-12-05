﻿using System;
using System.Collections.Generic;

namespace TodoList
{
    public static class AppInfo
    {

        public static Dictionary<Guid, TodoList> UserTodos { get; set; }

        public static List<Profile> AllProfiles { get; set; }
        public static Profile CurrentProfile { get; set; }

        public static string CurrentUserTodosFilePath { get; set; }

        public static Stack<ICommand> UndoStack { get; set; }
        public static Stack<ICommand> RedoStack { get; set; }

        public static TodoList CurrentUserTodoList
        {
            get
            {
                if (CurrentProfile == null || UserTodos == null || !UserTodos.ContainsKey(CurrentProfile.Id))
                {
                    return null;
                }
                return UserTodos[CurrentProfile.Id];
            }
        }
    }
}