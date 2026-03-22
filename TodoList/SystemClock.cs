using System;
using System.Collections.Generic;
using System.Text;
using TodoList.Interfaces;

namespace TodoList;

public class SystemClock : IClock
{
	public DateTime Now => DateTime.Now;
}