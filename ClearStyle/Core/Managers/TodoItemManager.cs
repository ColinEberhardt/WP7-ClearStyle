using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearStyle.Core.Data.Models;
using ClearStyle.Core.Repositories;

namespace ClearStyle.Core.Managers
{
	public class TodoItemManager
	{
		public IEnumerable<TodoItem> GetTodos()
		{
			return ClearStyleRepository<TodoItem>.GetAll();
		}

		public void Save(TodoItem todo)
		{
			ClearStyleRepository<TodoItem>.Save(todo);
		}

		public void DeleteTodo(TodoItem todo)
		{
			ClearStyleRepository<TodoItem>.Delete(todo);
		}
	}
}
