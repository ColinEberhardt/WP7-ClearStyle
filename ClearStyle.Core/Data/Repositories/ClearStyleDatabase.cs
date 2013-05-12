using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearStyle.Core.Data.Models;
using ClearStyle.Core.Providers.SQLite;

namespace ClearStyle.Core.Data.Repositories
{
	public class ClearStyleDatabase : SQLiteConnection 
	{
		static object locker = new object();

		public ClearStyleDatabase(string path) : base(path)
		{
			CreateTable<TodoItem>();

		}

		public IEnumerable<T> GetItems<T>() where T : IBusinessEntity, new() 
		{
			lock (locker)
			{
				return (from i in Table<T>() select i).ToList();
			}
		}

		public IEnumerable<T> GetAllBy<T>(Func<T, bool> pred) where T : IBusinessEntity, new() 
		{
			lock (locker)
			{
				return Table<T>().Where(pred);
			}
		}

		public T GetItem<T>(int id) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return Table<T>().FirstOrDefault(x => x.Id == id);
			}
		}

		public int SaveItem<T>(T item) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				var obj = GetItem<T>(item.Id);
				if (obj != null)
				{
					Update(item);
					return item.Id;
				}
				else
				{
					return Insert(item);
				}
			}
		}

		public int DeleteItem<T>(T item) where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				return Delete<T>(item); // primaryKey
				//return Delete (new Task() {ID = id}); // would also have worked
			}
		}

		public void ClearTable<T>() where T : IBusinessEntity, new()
		{
			lock (locker)
			{
				Execute(string.Format("delete from \"{0}\"", typeof(T).Name));
			}
		}
	}
}
