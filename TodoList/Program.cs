using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;


namespace TodoList
{
    internal class Program
    {
        static string fullName;
        static int age;
        static string[] todos = new string[2];
        static bool[] statuses = new bool[2];
        static DateTime[] dates = new DateTime[2];
        static int todosCount = 0;

        static void Main(string[] args)

        {
            Console.WriteLine("Работу выполнили Vasilevich и Garmash");
            Console.WriteLine("Продиктуйте ваше имя и фамилию мессир: ");
            string fullname = Console.ReadLine();
            Console.WriteLine("Продиктуйте ваш год рождения: ");
            DateTime birthdayDate = DateTime.ParseExact(Console.ReadLine(), "yyyy", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.Today;
            int age = currentDate.Year - birthdayDate.Year;
            Console.WriteLine(" Добавлен пользователь " + fullname + ", " + "возраст - " + age);
            string[] todos = new string[2];
            int todosCount = 0;
            while (true)
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "help":
                        ShowHelp();
                        break;

                    case "profile":
                        ShowProfile();
                        break;

                    case "view":
                        ViewTasks();
                        break;

                    case "exit":
                        ExitProgram();
                        return;

                    default:
                        if (input.StartsWith("add "))
                        {
                            AddTask(input.Substring(4));
                        }
                        else if (input.StartsWith("done "))
                        {
                            CompleteTask(input.Substring(5));
                        }
                        else if (input.StartsWith("delete "))
                        {
                            DeleteTask(input.Substring(7));
                        }
                        else if (input.StartsWith("update "))
                        {
                            UpdateTask(input.Substring(7));
                        }
                        else
                        {
                            Console.WriteLine("Неизвестная команда введите help для просмотра комманд.");
                        }
                        break;
                }
            }
        }
        static void CompleteTask(string indexStr)
        {
            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неправильный номер задачи");
                return;
            }

            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет");
                return;
            }

            int i = index - 1;
            statuses[i] = true;
            dates[i] = DateTime.Now;
            Console.WriteLine($"Задача {index} сделана");
        }
        static void DeleteTask(string indexStr)
        {
            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неправильный номер задачи");
                return;
            }

            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет");
                return;
            }

            int i = index - 1;
            for (int j = i; j < todosCount - 1; j++)
            {
                todos[j] = todos[j + 1];
                statuses[j] = statuses[j + 1];
                dates[j] = dates[j + 1];
            }
            todosCount--;
            Console.WriteLine($"Задача {index} удалена");
        }
        static void UpdateTask(string input)
        {
            int firstSpace = input.IndexOf(' ');
            if (firstSpace == -1)
            {
                Console.WriteLine("Неверный формат команды. Используйте: update 'номер задачи' текст");
                return;
            }

            string indexStr = input.Substring(0, firstSpace);
            string newText = input.Substring(firstSpace + 1).Trim();

            if (newText.StartsWith("\"") && newText.EndsWith("\""))
            {
                newText = newText[1..^1];
            }

            if (!int.TryParse(indexStr, out int index))
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }

            if (index < 1 || index > todosCount)
            {
                Console.WriteLine("Задачи с таким номером нет.");
                return;
            }

            int i = index - 1;
            todos[i] = newText;
            dates[i] = DateTime.Now;

            Console.WriteLine($"Задача {index} обновлена: {newText}");
        }
        static void ShowHelp()
        {
            Console.WriteLine("profile - выводит данные клиента");
            Console.WriteLine("add - добавляет новую задачу: add 'описание задачи'");
            Console.WriteLine("view - выводит все задачи");
            Console.WriteLine("exit - останавливает выполнение программы.");
            Console.WriteLine("delete - удаляет задачу: delete 'номер задачи'");
            Console.WriteLine("update - обновляет задачу: update 'номер задачи' текст");
            Console.WriteLine("done - выполняет задачу");
        }
        static void ShowProfile()
        {
            Console.WriteLine($"{fullName}, возраст: {age} лет");
        }
        static void AddTask(string taskText)
        {
            if (todosCount >= todos.Length) ;
            {
                ExpandArray();
            }
            todos[todosCount] = taskText;
            statuses[todosCount] = false;
            dates[todosCount] = DateTime.Now;
            todosCount++;
            Console.WriteLine($"Задача добавлена: {taskText}");

        }
        static void ExpandArray()
        {
            int newSize = todos.Length * 2;
            string[] newTodos = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];
            for (int i = 0; i < todos.Length; i++)
            {
                newTodos[i] = todos[i];
                newStatuses[i] = statuses[i];
                newDates[i] = dates[i];
            }
            todos = newTodos;
            statuses = newStatuses;
            dates = newDates;
        }
        static void ViewTasks()
        {
            Console.WriteLine("Ваши задачи:");
            if (todosCount == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }

            for (int i = 0; i < todosCount; i++)
            {
                string status = statuses[i] ? "сделано" : "не сделано";
                Console.WriteLine($"{i + 1}. {status}  {todos[i]}, (дата: {dates[i]})");
            }
        }
        static void ExitProgram()
        {
            Console.WriteLine("Завершение программы бай бай ...");
            Environment.Exit(0);
        }

    }

}
