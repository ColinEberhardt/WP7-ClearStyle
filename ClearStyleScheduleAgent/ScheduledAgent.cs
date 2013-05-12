using System;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using ClearStyle.Core.Managers;

namespace ClearStyleScheduleAgent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;
		TodoItemManager _todoManager;

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;
				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
				});
			}

			_todoManager = new TodoItemManager();
		}

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		/// <summary>
		/// Agent that runs a scheduled task
		/// </summary>
		/// <param name="task">
		/// The invoked task
		/// </param>
		/// <remarks>
		/// This method is called when a periodic or resource intensive task is invoked
		/// </remarks>
		protected override void OnInvoke(ScheduledTask task)
		{
			// get application tile
			ShellTile tile = ShellTile.ActiveTiles.First();
			if (null != tile)
			{
				// creata a new data for tile
				StandardTileData data = new StandardTileData();
				// tile foreground data
				data.Count = _todoManager.CountUnCompletedTodos();
				// update tile
				tile.Update(data);
			}
			ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromMinutes(30));
#if DEBUG_AGENT
	ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
	System.Diagnostics.Debug.WriteLine("Periodic task is started again: " + task.Name);
#endif

			NotifyComplete();
		}
	}
}