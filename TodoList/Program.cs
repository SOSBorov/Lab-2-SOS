using System.Globalization;
namespace lab2
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
                }
            }
        }
    }
}
