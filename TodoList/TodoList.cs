using System;

namespace TodoList
{
    public class TodoList
    {
        private TodoItem[] items;
        private int count;

        public TodoList(int initialCapacity = 2)
        {
            items = new TodoItem[initialCapacity];
            count = 0;
        }

        public void Add(TodoItem item)
        {
            if (count >= items.Length)
                IncreaseArray();

            items[count] = item;
            count++;
            Console.WriteLine($"Задача добавлена: {item.Text}");
        }

        public void Delete(int index)
        {
            if (index < 1 || index > count)
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }

            for (int i = index - 1; i < count - 1; i++)
                items[i] = items[i + 1];

            count--;
            Console.WriteLine($"Задача {index} удалена.");
        }

        public void View(bool showIndex, bool showDone, bool showDate)
        {
            if (count == 0)
            {
                Console.WriteLine("Нет задач.");
                return;
            }

            Console.WriteLine("СПИСОК ЗАДАЧ:");
            Console.WriteLine("------------------------------------------------------------");

            for (int i = 0; i < count; i++)
            {
                string line = "";
                if (showIndex) line += $"{i + 1,-5} ";
                line += items[i].GetShortInfo();
                Console.WriteLine(line);
            }

            Console.WriteLine("------------------------------------------------------------");
        }

        public void Read(int index)
        {
            if (index < 1 || index > count)
            {
                Console.WriteLine("Неверный номер задачи.");
                return;
            }

            Console.WriteLine($"Задача {index}:");
            Console.WriteLine(items[index - 1].GetFullInfo());
        }

        private void IncreaseArray()
        {
            int newSize = items.Length * 2;
            TodoItem[] newItems = new TodoItem[newSize];
            for (int i = 0; i < items.Length; i++)
                newItems[i] = items[i];
            items = newItems;
        }

        public TodoItem GetItem(int index)
        {
            if (index < 1 || index > count)
                return null;
            return items[index - 1];
        }

        public int Count => count;
    }
}
