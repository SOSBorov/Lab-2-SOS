using System.Windows.Input;
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
 status <номер> <статус> - изменить статус задачи
 update <номер> <текст> - изменить текст задачи
 delete <номер>         - удалить задачу
 undo                   - отменить последнее действие
 redo                   - повторить последнее отмененное действие
 view [флаги]           - показать задачи
 profile                - показать информацию о текущем профиле
 help                   - показать список команд
 exit                   - выход

Флаги для view:
 -i, --index          - показывать индекс задачи
 -s, --status         - показывать статус
 -d, --update-date    - показывать дату изменения
 -a, --all            - показывать всё

Статусы для команды status:
 NotStarted, InProgress, Completed, Postponed, Failed
");
        }

        public void Unexecute() { }
    }
}