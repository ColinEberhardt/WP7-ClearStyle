using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ClearStyle.Core.Data.Models;
using SystemColor = System.Windows.Media.Color;

namespace ClearStyle.ViewModel
{
	/// <summary>
	/// A collection of todo items
	/// </summary>
	public class ToDoListViewModel
	{
		private ResettableObservableCollection<ToDoItemViewModel> _todoItems = new ResettableObservableCollection<ToDoItemViewModel>();

		public ToDoListViewModel()
		{
			_todoItems.CollectionChanged += (s, e) => UpdateToDoColors();
		}

		public ResettableObservableCollection<ToDoItemViewModel> Items
		{
			get
			{
				return _todoItems;
			}
		}

		public void Update(IEnumerable<TodoItem> todos)
		{
			foreach (var todo in todos)
			{
				var toDoItemViewModel = new ToDoItemViewModel(todo);
				Items.Add(toDoItemViewModel);
			}
		}

		private void UpdateToDoColors()
		{
			double itemCount = _todoItems.Count;
			double index = 0;
			foreach (var todoItem in _todoItems)
			{
				double val = (index / itemCount) * 155.0;
				index++;

				if (!todoItem.Completed)
				{
					todoItem.Color = SystemColor.FromArgb(255, 255, (byte)val, 0);
				}
			};
		}
	}
}
