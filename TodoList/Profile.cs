namespace TodoList
{
    public class Profile
    {
        public string Name { get; set; } = "Default";

        public Profile() { }

        public Profile(string name)
        {
            Name = name;
        }

        public string GetInfo() => $"Пользователь: {Name}";
    }
}
