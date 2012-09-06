using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ClearStyle.ViewModel;

namespace ClearStyle.Interactions
{
  /// <summary>
  /// An interaction is handles gestures from the UI in order to perform actions
  /// on the model. Interactions have the conpcet of Enabled and Active in order
  /// that the InteractionManager can ensure that only one interaction is
  /// active at one time
  /// </summary>
  public interface IInteraction
  {
    /// <summary>
    /// Initialises the interaction, providing it with todo model items and the UI that renders then.
    /// </summary>
    void Initialise(ItemsControl todoList, ResettableObservableCollection<ToDoItemViewModel> todoItems);

    /// <summary>
    /// Invoked when a new element that is the ItemsContainer for a ToDoItem is added to the list. This allows
    /// the interaction to add event handlers to the element.
    /// </summary>
    void AddElement(FrameworkElement element);

    bool IsActive { get; }

    bool IsEnabled { get; set; }

    /// <summary>
    /// Occurs when this interaction becomes active
    /// </summary>
    event EventHandler Activated;

    /// <summary>
    /// Occurs when this interaction completes
    /// </summary>
    event EventHandler DeActivated;
  }
}
