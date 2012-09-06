using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using ClearStyle.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using ClearStyle.Interactions;
using System.Diagnostics;

namespace ClearStyle
{
  public partial class MainPage : PhoneApplicationPage
  {
    // the model objects
    private ToDoListViewModel _viewModel = new ToDoListViewModel();
    
    private InteractionManager _interactionManager = new InteractionManager();

    // Constructor
    public MainPage()
    {
      InitializeComponent();

      _viewModel.Items.Add(new ToDoItemViewModel("Feed the cat"));
      _viewModel.Items.Add(new ToDoItemViewModel("Buy eggs"));
      _viewModel.Items.Add(new ToDoItemViewModel("Pack bags for WWDC conference"));
      _viewModel.Items.Add(new ToDoItemViewModel("Rule the web"));
      _viewModel.Items.Add(new ToDoItemViewModel("Order business cards"));
      _viewModel.Items.Add(new ToDoItemViewModel("Fix laptop"));
      _viewModel.Items.Add(new ToDoItemViewModel("Get some dollars for trip"));
      _viewModel.Items.Add(new ToDoItemViewModel("Shirts"));
      _viewModel.Items.Add(new ToDoItemViewModel("Shopping"));
      _viewModel.Items.Add(new ToDoItemViewModel("Contact PR company"));
      _viewModel.Items.Add(new ToDoItemViewModel("Extension plans"));
      _viewModel.Items.Add(new ToDoItemViewModel("Choose colour scheme"));
      _viewModel.Items.Add(new ToDoItemViewModel("Create new website"));
      _viewModel.Items.Add(new ToDoItemViewModel("Write-up blog post"));
      _viewModel.Items.Add(new ToDoItemViewModel("Choose life"));
      _viewModel.Items.Add(new ToDoItemViewModel("Simplify my life"));

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