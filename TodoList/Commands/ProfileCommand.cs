using System;

namespace TodoList
{
    public class ProfileCommand : ICommand
    {
        public Profile? Profile { get; set; }
        public string? Name { get; set; }

        public string? ProfileFilePath { get; set; }

        public void Execute()
        {
            if (Profile == null)
                throw new InvalidOperationException("Профиль не установлен");
            if (ProfileFilePath == null)
                throw new InvalidOperationException("Путь к файлу профиля не установлен");

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Profile.Name = Name!;
                Console.WriteLine($"Имя профиля изменено на: {Profile.Name}");

                FileManager.SaveProfile(Profile, ProfileFilePath);
            }
            else
            {
                Console.WriteLine(Profile.GetInfo());
            }
        }
    }
}