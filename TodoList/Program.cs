using System;

namespace TodoList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили Vasilevich и Garmash");
            Console.WriteLine("Введите ваше имя: ");
            string firstName = Console.ReadLine();

            Console.WriteLine("Введите вашу фамилию: ");
            string lastName = Console.ReadLine();

            Console.WriteLine("Введите ваш год рождения (в формате yyyy): ");
            int birthYear = int.Parse(Console.ReadLine());

            Profile profile = new Profile(firstName, lastName, birthYear);
            TodoList todoList = new TodoList();

            Console.WriteLine($"Добавлен пользователь {profile.GetInfo()}");
            Console.WriteLine("Введите 'help' для списка команд.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                string command = parts[0];
                string argument = parts.Length > 1 ? parts[1] : "";

                if (command == "help")
                {
                    ShowHelp();
                }
                else if (command == "profile")
                {
                    Console.WriteLine(profile.GetInfo());
                }

                else if (command == "add")
                {
                    bool isMultiline = argument.StartsWith("--multiline") || argument.StartsWith("-m");
                    string taskText = "";

                    if (isMultiline)
                    {
                        Console.WriteLine("Введите строки задачи (введите !end для завершения):");
                        string line;
                        while (true)
                        {
                            Console.Write("> ");
                            line = Console.ReadLine();
                            if (line.Trim() == "!end")
                                break;

                            taskText += line + Environment.NewLine;
                        }
                    }
                    else
                    {
                        taskText = argument;
                    }

                    if (string.IsNullOrWhiteSpace(taskText))
                    {
                        Console.WriteLine("Текст задачи не может быть пустым!");
                    }
                    else
                    {
                        todoList.Add(new TodoItem(taskText.Trim()));
                        Console.WriteLine("Задача успешно добавлена!");
                    }
                }

                else if (command == "delete")
                {
                    if (int.TryParse(argument, out int delIndex))
                        todoList.Delete(delIndex);
                    else
                        Console.WriteLine("Введите корректный номер задачи.");
                }
                else if (command == "done")
                {
                    if (int.TryParse(argument, out int doneIndex))
                    {
                        var item = todoList.GetItem(doneIndex);
                        if (item != null)
                        {
                            item.MarkDone();
                            Console.WriteLine($"Задача {doneIndex} отмечена выполненной.");
                        }
                        else
                        {
                            Console.WriteLine("Такой задачи не существует.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Введите корректный номер задачи.");
                    }
                }
                else if (command == "update")
                {
                    string[] args2 = argument.Split(' ', 2);
                    if (args2.Length == 2 && int.TryParse(args2[0], out int updIndex))
                    {
                        var item = todoList.GetItem(updIndex);
                        if (item != null)
                        {
                            item.UpdateText(args2[1]);
                            Console.WriteLine($"Задача {updIndex} обновлена.");
                        }
                        else
                        {
                            Console.WriteLine("Такой задачи не существует.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Используйте: update <номер> <новый текст>");
                    }
                }

                else if (command == "view")
                {
                    bool showAll = argument.Contains("--all") || argument.Contains("-a");
                    bool showIndex = showAll || argument.Contains("--index") || argument.Contains("-i");
                    bool showStatus = showAll || argument.Contains("--status") || argument.Contains("-s");
                    bool showDate = showAll || argument.Contains("--update-date") || argument.Contains("-d");

                    todoList.View(showIndex, showStatus, showDate);
                }

                else if (command == "read")
                {
                    if (int.TryParse(argument, out int readIndex))
                        todoList.Read(readIndex);
                    else
                        Console.WriteLine("Введите корректный номер задачи.");
                }
                else if (command == "exit")
                {
                    Console.WriteLine("Завершение программы...");
                    return;
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите 'help' для списка команд.");
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine(@"
Доступные команды:
 help               - показать это меню
 profile            - показать информацию о пользователе
 add <текст>        - добавить задачу
 add --multiline    - многострочный ввод задачи (!end для завершения)
 delete <номер>     - удалить задачу
 done <номер>       - отметить задачу выполненной
 update <номер> <текст> - изменить текст задачи
 view               - показать список задач
 view -i -s -d -a   - флаги для отображения индексов, статусов, дат и всех данных
 read <номер>       - показать задачу полностью
 exit               - завершить программу
");
        }
    }
}
