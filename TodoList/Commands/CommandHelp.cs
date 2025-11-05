using System;

namespace TodoList
{
    public class HelpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine(@"
Доступные команды:
 add <текст>            - добавить задачу
 add -m / --multiline   - многострочный ввод (!end - завершить)
 done <номер>           - отметить задачу выполненной
 update <номер> <текст> - изменить текст задачи
 delete <номер>         - удалить задачу
 view [флаги]           - показать задачи
 profile                - показать профиль пользователя
 help                   - показать список команд
 exit                   - выход

Флаги для view:
 -i, --index          - показывать индекс задачи
 -s, --status         - показывать статус
 -d, --update-date    - показывать дату изменения
 -a, --all            - показывать всё
");
        }
    }
}