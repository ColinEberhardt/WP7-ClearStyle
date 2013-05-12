using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using ClearStyle.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using ClearStyle.Interactions;
using System.Diagnostics;
using ClearStyle.Core.Managers;
using Microsoft.Phone.Scheduler;
using System;
using Microsoft.Phone.Shell;
using System.Linq;

namespace ClearStyle
{
	public partial class MainPage : PhoneApplicationPage
	{
		// the model objects
		private ToDoListViewModel _viewModel = new ToDoListViewModel();

		private InteractionManager _interactionManager = new InteractionManager();

		TodoItemManager _todoManager;
		string periodicTaskName;
		PeriodicTask periodicTask;
		// Constructor
		public MainPage()
		{
			InitializeComponent();
			_todoManager = new TodoItemManager();

			_viewModel.Update(_todoManager.GetTodos());

			this.DataContext = _viewModel.Items;

			var dragReOrderInteraction = new DragReOrderInteraction(dragImageControl);
			dragReOrderInteraction.Initialise(todoList, _viewModel.Items);

			var swipeInteraction = new SwipeInteraction();
			swipeInteraction.Initialise(todoList, _viewModel.Items);

			var tapEditInteraction = new TapEditInteraction();
			tapEditInteraction.Initialise(todoList, _viewModel.Items);

			var addItemInteraction = new PullDownToAddNewInteraction(tapEditInteraction, pullDownItemInFront);
			addItemInteraction.Initialise(todoList, _viewModel.Items);

			var pinchAddNewItemInteraction = new PinchAddNewInteraction(tapEditInteraction, pullDownItemBehind);
			pinchAddNewItemInteraction.Initialise(todoList, _viewModel.Items);

			_interactionManager.AddInteraction(swipeInteraction);
			_interactionManager.AddInteraction(dragReOrderInteraction);
			_interactionManager.AddInteraction(addItemInteraction);
			_interactionManager.AddInteraction(tapEditInteraction);
			_interactionManager.AddInteraction(pinchAddNewItemInteraction);

			periodicTaskName = "ClearStyleTilePeriodicAgent";

			StartPeriodicAgent();

			FrameworkDispatcher.Update();

		}

		private void Border_Loaded(object sender, RoutedEventArgs e)
		{
			_interactionManager.AddElement(sender as FrameworkElement);
		}

		private void StartPeriodicAgent()
		{
			// is old task running, remove it
			periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
			if (periodicTask != null)
			{
				try
				{
					ScheduledActionService.Remove(periodicTaskName);
				}
				catch (Exception)
				{
				}
			}
			// create a new task
			periodicTask = new PeriodicTask(periodicTaskName);
			// load description from localized strings
			periodicTask.Description = "This is LiveTile application update agent.";
			// set expiration days
			periodicTask.ExpirationTime = DateTime.Now.AddDays(14);
			try
			{
				// add thas to scheduled action service
				ScheduledActionService.Add(periodicTask);
				// debug, so run in every 30 secs
				ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromMinutes(30));
#if(DEBUG_AGENT)
		ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
		System.Diagnostics.Debug.WriteLine("Periodic task is started: " + periodicTaskName);
#endif

			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message.Contains("BNS Error: The action is disabled"))
				{
					// load error text from localized strings
					MessageBox.Show("Background agents for this application have been disabled by the user.");
				}
				if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
				{
					// No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
				}
			}
			catch (SchedulerServiceException)
			{
				// No user action required.
			}
		}

		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
		{
			UpdateTile();
			base.OnBackKeyPress(e);
		}

		protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			UpdateTile();

			base.OnNavigatingFrom(e);
		}

		private void UpdateTile()
		{
			// get application tile
			ShellTile tile = ShellTile.ActiveTiles.First();
			if (null != tile)
			{
				// create a new data for tile
				StandardTileData data = new StandardTileData();
				data.Count = _todoManager.CountUnCompletedTodos();
				tile.Update(data);
			}
		}
	}
}