using System.Globalization;


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
                        else
                        {
                            Console.WriteLine("Неизвестная команда. Введите help для списка доступных команд.");
                        }
                        break;
                }
            }
        }
        static void ShowHelp()
        {
            Console.WriteLine("profile - выводит данные клиента");
            Console.WriteLine("add - добавляет новую задачу");
            Console.WriteLine("view - выводит все задачи");
            Console.WriteLine("exit - останавливает выполнение программы.");
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
            todos[todosCount++] = taskText;
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
                Console.WriteLine($"{i + 1}. {todos[i]}, (дата: {dates[i]})");
            }
        }
        static void ExitProgram()
        {
            Console.WriteLine("Завершение программы бай бай ...");
            Environment.Exit(0);
        }

    }

}
