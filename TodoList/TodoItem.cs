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
        return $"({Status}) {Text} обновлено {LastUpdated:dd.MM.yyyy HH:mm}";
    }
}