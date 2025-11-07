public class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastUpdated { get; set; } = DateTime.Now;


    public override string ToString()
    {
        string status = IsCompleted ? "[x]" : "[ ]";
        return $"{status} {Text}";
    }
}