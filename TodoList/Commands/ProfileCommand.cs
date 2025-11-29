using System;

namespace TodoList
{
    public class ProfileCommand : ICommand
    {
        public string? Name { get; set; }
        public string? ProfileFilePath { get; set; }
        private string _previousName;

        public void Execute()
        {
            if (AppInfo.CurrentProfile == null) throw new InvalidOperationException("Профиль не инициализирован");
            if (ProfileFilePath == null) throw new InvalidOperationException("Путь к файлу профиля не установлен");

            if (!string.IsNullOrWhiteSpace(Name))
            {
                _previousName = AppInfo.CurrentProfile.Name;
                AppInfo.CurrentProfile.Name = Name!;
                Console.WriteLine($"Имя профиля изменено на: {AppInfo.CurrentProfile.Name}");
                FileManager.SaveProfile(AppInfo.CurrentProfile, ProfileFilePath);
            }
            else
            {
                Console.WriteLine(AppInfo.CurrentProfile.GetInfo());
            }
        }

        public void Unexecute()
        {
            if (_previousName != null)
            {
                AppInfo.CurrentProfile.Name = _previousName;
                FileManager.SaveProfile(AppInfo.CurrentProfile, ProfileFilePath);
                Console.WriteLine($"Имя профиля возвращено на: {AppInfo.CurrentProfile.Name}");
            }
        }
    }
}
