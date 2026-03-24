using System;
using System.Collections.Generic;
using System.Text;

namespace TodoList.Interfaces;

public interface IClock
{
	DateTime Now { get; }
}
