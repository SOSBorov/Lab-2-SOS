using System;

namespace TodoList
{
    public class ProfileCommand : ICommand
    {
        public Profile? Profile { get; set; }
        public string? Name { get; set; }

        public void Execute()
        {
            if (Profile == null) return;

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Profile.Name = Name!;
                Console.WriteLine($"Имя профиля изменено на: {Profile.Name}");
            }
            else
            {
                Console.WriteLine(Profile.GetInfo());
            }
        }
    }
}
