using System.Globalization;

namespace TodoList
{
    internal class Program
    {
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
            Console.WriteLine("Введите команду: ");
            while (true)
            {
                {
                    var input = Console.ReadLine();
                    switch (input)
                    {
                        case "help":
                            Console.WriteLine(todos[1]);
                            Console.WriteLine(todos[2]);
                            Console.WriteLine(todos[3]);
                            Console.WriteLine(todos[4]);
                            break;
                        case string s when s.StartsWith("add "):
                            string taskText = s.Substring(4);
                            if (todosCount >= todos.Length)
                            {
                                string[] newTodos = new string[todos.Length * 2];
                                for (int i = 0; i < todos.Length; i++)
                                    newTodos[i] = todos[i];
                                todos = newTodos;
                            }

                            todos[todosCount++] = taskText;
                            Console.WriteLine("Задача добавлена: " + taskText);
                            break;
                        case "profile":
                            Console.WriteLine(" Пользователь " + fullname + " возраст - " + age);
                            break;
                        case "view":
                            Console.WriteLine("Ваши задачи:");
                            for (int i = 0; i < todosCount; i++)
                            {
                                Console.WriteLine($"{i + 1}. {todos[i]}");
                            }
                            if (todosCount == 0) Console.WriteLine("Нет задач.");
                            break;
                        case "exit":
                            Console.WriteLine("Завершение задачи");
                            return;

                    }
                }
            }
        }
    }
}
