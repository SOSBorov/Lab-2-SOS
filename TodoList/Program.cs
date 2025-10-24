using System;

namespace TodoListOOP
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
                    todoList.Add(new TodoItem(argument));
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
                    todoList.View(true, true, true);
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
 delete <номер>     - удалить задачу
 done <номер>       - отметить задачу выполненной
 update <номер> <текст> - изменить текст задачи
 view               - показать список задач
 read <номер>       - показать задачу полностью
 exit               - завершить программу
");
        }
    }
}
