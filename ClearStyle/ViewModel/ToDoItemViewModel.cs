using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace ClearStyle.ViewModel
{
	/// <summary>
	/// A single todo item.
	/// </summary>
	public class ToDoItemViewModel : INotifyPropertyChanged
	{
		public int Id { get; private set; }

		private string _text;

		private bool _completed;

		private Color _color = Colors.Red;

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public bool Completed
		{
			get { return _completed; }
			set
			{
				_completed = value;
				OnPropertyChanged("Completed");
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				OnPropertyChanged("Color");
			}
		}

		public ToDoItemViewModel(string text)
		{
			Text = text;
		}

		public ToDoItemViewModel(Core.Data.Models.TodoItem model)
		{
			Update(model);
		}

		public void Update(Core.Data.Models.TodoItem model)
		{
			Id = model.Id;
			Text = model.Text;
			Completed = model.Completed;
			if (Completed)
			{
				Color = Colors.Green;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}


		internal Core.Data.Models.TodoItem ToModel()
		{
			return new Core.Data.Models.TodoItem
			{
				Id = this.Id,
				Text = this.Text,
				Completed = this.Completed
			};
		}
	}
}
