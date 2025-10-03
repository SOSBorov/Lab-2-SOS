using System.Globalization;
namespace todolist
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
		            string[] tasks = new string[2];
            int taskCount = 0;
            string[] todos = { "help", "profile", "add", "view", "exit" };
            todos[1] = "profile - выводит данные пользователя";
            todos[2] = "add - добавляет новую задачу";
            todos[3] = "view - выводит все задачи";
            todos[4] = "exit - завершает цикл.";
            Console.WriteLine("Введите команду: ");
            while (true)
            {
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.Write("перезапустите приложение");
                    break;
                }

                switch (input)
                {
                    case "help":
                        Console.WriteLine(todos[1]);
                        Console.WriteLine(todos[2]);
                        Console.WriteLine(todos[3]);
                        Console.WriteLine(todos[4]);
                        break;
						case "profile":
    		Console.WriteLine(" Пользователь " + fullname  + " возраст - " + age);
    					break;
						case "add":
            Console.WriteLine("введите команду");
                        string task = Console.ReadLine();
                        string[] parts = task.Split(' ', 2);
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Ошибка: используйте формат add текст задачи");
                        }
                        else
                        {
                            if (taskCount >= task.Length) ;
                            {
                                string[] newTask = new string[tasks.Length * 2];
                                for (int i = 0; i < task.Length; i++) ;
                                tasks = newTask;
                            }
                            tasks[taskCount++] = parts[1];
                            Console.WriteLine("Задача добавлена: " + parts[1]);
                        }
                        break;
						case "view":
                Console.WriteLine("Ваши задачи:");
                        for (int i = 0; i < taskCount; i++)
                        {
                            Console.WriteLine($"{i + 1}. {tasks[i]}");
                        }
                        if (taskCount == 0) Console.WriteLine("Нет задач.");
                        break;
						 case "exit":
                Console.WriteLine("Завершение задачи");
                        return;
						
                }
            }
        }
    }
}
