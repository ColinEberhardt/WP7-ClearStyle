using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearStyle.Core.Providers.SQLite;

namespace ClearStyle.Core.Data.Models
{
	public class TodoItem : IBusinessEntity
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Text { get; set; }
		public bool Completed { get; set; }
	}
}
