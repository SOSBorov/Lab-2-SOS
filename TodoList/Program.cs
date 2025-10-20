using System.Globalization; 


namespace TodoList 
{
    internal class Program
    {
        private const int InitialTasksCapacity = 2;
        private const string DateFormat = "yyyy";

        private static string[] tasks = new string[InitialTasksCapacity];
        private static bool[] taskStatuses = new bool[InitialTasksCapacity];
        private static DateTime[] taskDates = new DateTime[InitialTasksCapacity];

        private static int taskCount = 0;
        static string fullName;
        static int age;
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

            while (true)

            {
                string input = Console.ReadLine();

                if (input == "help")
                {
                    ShowHelp();
                }
                else if (input == "profile")
                {
                    ShowProfile();
                }
                else if (input == "view")
                {
                    ViewTasks("");
                }
                else if (input == "exit")
                {
                    ExitProgram();
                    return;
                }
                else if (input.StartsWith("add "))
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
                else if (input.StartsWith("view "))
                {
                    ViewTasks(input.Substring(5));
                }
                else if (input.StartsWith("read "))
                {
                    ReadTask(input.Substring(5));
                }
                else if (input.StartsWith("update "))
                {
                    UpdateTask(input.Substring(7));
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите 'help' для просмотра команд.");

    
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
            taskStatuses[i] = true;
            taskDates[i] = DateTime.Now;
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
                tasks[j] = tasks[j + 1];
                taskStatuses[j] = taskStatuses[j + 1];
                taskDates[j] = taskDates[j + 1];
            }
            todosCount--;
            Console.WriteLine($"Задача {index} удалена");
        }
        static void UpdateTask(string input)
        {
            string[] parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries); 

            if (parts.Length < 2)
            {
                Console.WriteLine("Неверный формат команды. Используйте: update 'номер задачи' текст");
                return;
            }

            string indexStr = parts[0];
            string newText = parts[1].Trim();

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
            tasks[i] = newText;
            taskDates[i] = DateTime.Now;

            Console.WriteLine($"Задача {index} обновлена: {newText}");
        }

        static void ShowHelp()
        {
            string[] commands =
                {
                @"
                Базовые команды
            profile - выводит данные клиента
            add - добавляет новую задачу: add 'описание задачи
            view - выводит все задачи
            read <номер> - показать полную задачу с деталями
            exit - останавливает выполнение программы
            delete - удаляет задачу: delete 'номер задачи'
            update - обновляет задачу
            done - выполняет задачу

                Флаги к команде add
            add -m / --multiline - многострочный ввод задачи (!end для завершения
            
                Флаги к команде view
            view --index / -i показать индексы задач
            view --status / -s - показать статус задач
            view --update-date / -d - показать дату изменения
            view --all / -a - показать все данные
            "

        };

            Console.WriteLine("Доступные команды:");
            foreach (var line in commands)
            {
                Console.WriteLine("  " + line);
            }
        }

        static void ShowProfile()
        {
            Console.WriteLine($"{fullName}, возраст: {age} лет");
        }
        static void AddTask(string taskText)
        {
            bool isMultiline = taskText.StartsWith("--multiline") || taskText.StartsWith("-m");

            if (isMultiline)
            {
                Console.WriteLine("Введите строки задачи (введите !end для завершения):");

                string[] lines = new string[0];
                while (true)
                {
                    Console.Write("> ");
                    string line = Console.ReadLine();

                    if (line.Trim() == "!end")
                        break;

                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = line;
                }

                taskText = string.Join("\n", lines);
            }
            else
            {
                taskText = taskText.Trim();

                if (taskText.StartsWith("\"") && taskText.EndsWith("\""))
                {
                    taskText = taskText.Substring(1, taskText.Length - 2);
                }

                if (tasks == null || taskStatuses == null || taskDates == null)
                {
                    Console.WriteLine("Ошибка: массивы задач не инициализированы");
                    return;
                }
            }

            if (todosCount >= tasks.Length)
                ExpandArray();

            tasks[todosCount] = taskText;
            taskStatuses[todosCount] = false;
            taskDates[todosCount] = DateTime.Now;
            todosCount++;

            Console.WriteLine($"Задача добавлена: {taskText}");
        }

        static void ExpandArray()
        {
            int newSize = tasks.Length * 2;
            string[] newTodos = new string[newSize];
            bool[] newStatuses = new bool[newSize];
            DateTime[] newDates = new DateTime[newSize];
            for (int i = 0; i < tasks.Length; i++)
            {
                newTodos[i] = tasks[i];
                newStatuses[i] = taskStatuses[i];
                newDates[i] = taskDates[i];
            }
            tasks = newTodos;
            taskStatuses = newStatuses;
            taskDates = newDates;
        }
        static void ViewTasks(string flags)
        {
            if (todosCount == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }

            string combined = string.Concat(
                flags.Split(' ')
                     .Where(f => f.StartsWith("-"))
                     .Select(f => f.TrimStart('-'))
            );

            bool showAll = flags.Contains("--all") || combined.Contains('a');

            bool showIndex = showAll || flags.Contains("--index") || combined.Contains('i');
            bool showStatus = showAll || flags.Contains("--status") || combined.Contains('s');
            bool showDate = showAll || flags.Contains("--update-date") || combined.Contains('d');

            Console.WriteLine("СПИСОК ЗАДАЧ:");
            Console.WriteLine("------------------------------------------------------------");

            for (int i = 0; i < todosCount; i++)
            {
                string taskText = tasks[i];
                if (taskText.Length > 30)
                    taskText = taskText.Substring(0, 30) + "...";

                string line = "";
                if (showIndex) line += $"{i + 1,-5} ";
                line += $"{taskText,-35}";
                if (showStatus) line += $"{(taskStatuses[i] ? "выполнена" : "не выполнена"),-12}";
                if (showDate) line += $"{taskDates[i]}";

                Console.WriteLine(line);
            }

            Console.WriteLine("------------------------------------------------------------");
        }

        static void ReadTask(string indexStr)
        {
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
            Console.WriteLine($"Задача {index}:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(tasks[i]);
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Статус: {(taskStatuses[i] ? "выполнена" : "не выполнена")}");
            Console.WriteLine($"Дата последнего изменения: {taskDates[i]}");
        }

        static void ExitProgram()
        {
            Console.WriteLine("Завершение программы бай бай ...");
            Environment.Exit(0);
        }

    }

}
