using System.Globalization;
namespace OMGKERDICK
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
			Console.ReadKey();
		}
	}
}