using System;
using TodoList;

public class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public TodoStatus Status { get; set; } = TodoStatus.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public override string ToString()
    {
        string statusSymbol = GetStatusSymbol(Status);
        return $"{statusSymbol} {Text} обновлено {LastUpdated:dd.MM.yyyy HH:mm}";
    }

    public static string GetStatusSymbol(TodoStatus status)
    {
        switch (status)
        {
            case TodoStatus.NotStarted: return "[ ]";
            case TodoStatus.InProgress: return "[>]";
            case TodoStatus.Completed: return "[x]";
            case TodoStatus.Postponed: return "[-]";
            case TodoStatus.Failed: return "[!]";
            default: return "[?]";
        }
    }
}