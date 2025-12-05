using System;

namespace TodoList
{
    public class ProfileCommand : ICommand
    {
        public string? Name { get; set; }
        public string? ProfileFilePath { get; set; }
        private string _previousName;


        public bool IsLogout { get; set; }

        public void Execute()
        {
            if (IsLogout)
            {
                Console.WriteLine($"Пользователь {AppInfo.CurrentProfile.Login} вышел из системы.");
                AppInfo.CurrentProfile = null; 
                return;
            }

            if (AppInfo.CurrentProfile == null) throw new InvalidOperationException("Профиль не инициализирован");
            if (ProfileFilePath == null) throw new InvalidOperationException("Путь к файлу профиля не установлен");

            if (!string.IsNullOrWhiteSpace(Name))
            {
                _previousName = AppInfo.CurrentProfile.FirstName;
                Console.WriteLine($"Имя профиля изменено на: {AppInfo.CurrentProfile.FirstName}");
               
            }
            else
            {
                Console.WriteLine(AppInfo.CurrentProfile.GetInfo());
            }
        }

        public void Unexecute()
        {
            if (IsLogout) return;

            if (_previousName != null)
            {
  
                Console.WriteLine($"Имя профиля возвращено на: {AppInfo.CurrentProfile.FirstName}");
            }
        }
    }
}