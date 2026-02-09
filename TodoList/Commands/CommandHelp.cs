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
 search [флаги]         - поиск задач по тексту, дате, статусу и сортировке
 profile                - показать информацию о текущем профиле
 help                   - показать список команд
 exit                   - выход

Флаги для search:
 --contains 'text'         - текст содержит подстроку
 --starts-with 'text'      - текст начинается с подстроки
 --ends-with 'text'        - текст заканчивается подстрокой
 --from yyyy-MM-dd         - дата изменения не раньше указанной
 --to yyyy-MM-dd           - дата изменения не позже указанной
 --status 'status'         - фильтр по статусу
 --sort text|date          - сортировка по тексту или дате
 --desc                    - сортировка по убыванию
 --top число               - ограничить количество результатов

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