using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using ClearStyle.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using ClearStyle.Interactions;
using System.Diagnostics;
using ClearStyle.Core.Managers;

namespace ClearStyle
{
	public partial class MainPage : PhoneApplicationPage
	{
		// the model objects
		private ToDoListViewModel _viewModel = new ToDoListViewModel();

		private InteractionManager _interactionManager = new InteractionManager();

		TodoItemManager _todoManager;
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

			FrameworkDispatcher.Update();

		}

		private void Border_Loaded(object sender, RoutedEventArgs e)
		{
			_interactionManager.AddElement(sender as FrameworkElement);
		}
	}
}