using System;

namespace TodoList
{
    public class TodoItem
    {
        public string Text { get; private set; }
        public bool IsDone { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TodoItem(string text)
        {
            Text = text;
            IsDone = false;
            LastUpdate = DateTime.Now;
        }

        public void MarkDone()
        {
            IsDone = true;
            LastUpdate = DateTime.Now;
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            LastUpdate = DateTime.Now;
        }

        public string GetShortInfo()
        {
            string shortText = Text.Length > 30 ? Text.Substring(0, 30) + "..." : Text;
            string status = IsDone ? "выполнена" : "не выполнена";
            return $"{shortText,-35} {status,-12} {LastUpdate}";
        }

        public string GetFullInfo()
        {
            string status = IsDone ? "выполнена" : "не выполнена";
            return $"Описание: {Text}\nСтатус: {status}\nДата изменения: {LastUpdate}";
        }
    }
}
