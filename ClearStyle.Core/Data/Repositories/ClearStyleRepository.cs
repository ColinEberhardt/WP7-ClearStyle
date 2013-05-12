using System;
using System.Collections.Generic;
using System.IO;
using ClearStyle.Core.Data.Models;
using ClearStyle.Core.Data.Repositories;

namespace ClearStyle.Core.Repositories
{
	/// <summary>
	/// The repository is responsible for providing an abstraction to actual data storage mechanism
	/// whether it be SQLite, XML or some other method
	/// </summary>
	public class ClearStyleRepository<T> : IDisposable where T : IBusinessEntity, new()
	{
		ClearStyleDatabase db = null;
		protected static string dbLocation;
		protected static ClearStyleRepository<T> me;

		static ClearStyleRepository()
		{
			me = new ClearStyleRepository<T>();
		}

		protected ClearStyleRepository()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new ClearStyleDatabase(dbLocation);
		}

		public static string DatabaseFilePath
		{
			get
			{
				return "ClearStyle.db";
				//var path = sqliteFilename;
//#if SILVERLIGHT
//				// Windows Phone expects a local path, not absolute
//				var path = sqliteFilename;
//#else

//#if __ANDROID__
//				// Just use whatever directory SpecialFolder.Personal returns
//				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
//				var path = Path.Combine (libraryPath, sqliteFilename);
//#else
//#if __IOS_
//				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
//				// (they don't want non-user-generated data in Documents)
//				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
//				string libraryPath = Path.Combine (documentsPath, "../Library/"); // Library folder
//				var path = Path.Combine (libraryPath, sqliteFilename);
//#endif
//#endif
//#endif

				//return path;
			}
		}

		public static T Get(int id)
		{
			return me.db.GetItem<T>(id);
		}

		public static T Get(T id)
		{
			return me.db.GetItem<T>(id.Id);
		}

		public static IEnumerable<T> GetAll()
		{
			return me.db.GetItems<T>();
		}

		public static int Save(T item)
		{
			return me.db.SaveItem<T>(item);
		}

		public static int Delete(T id)
		{
			return me.db.DeleteItem(id);
		}

		public static void ClearTable()
		{
			me.db.ClearTable<T>();
		}

		public static IEnumerable<T> GetAllBy(Func<T, bool> pred)
		{
			return me.db.GetAllBy<T>(pred);
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}