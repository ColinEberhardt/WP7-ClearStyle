using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using ClearStyle.ViewModel;
using LinqToVisualTree;
using System.Linq;

namespace ClearStyle.Interactions
{
  /// <summary>
  /// A base class for interactions.
  /// </summary>
  public abstract class InteractionBase : IInteraction
  {
    private bool _isActive = false;

    protected ItemsControl _todoList;
    protected ResettableObservableCollection<ToDoItemViewModel> _todoItems;
    protected ScrollViewer _scrollViewer;

    public virtual void Initialise(ItemsControl todoList, ResettableObservableCollection<ToDoItemViewModel> todoItems)
    {
      _todoList = todoList;
      _todoItems = todoItems;

      // when the ItemsControl has been rendered, we can locate the ScrollViewer
      // that is within its template.
      _todoList.InvokeOnNextLayoutUpdated(() => LocateScrollViewer());

      IsEnabled = true;
    }

    private void LocateScrollViewer()
    {
      _scrollViewer = _todoList.Descendants<ScrollViewer>()
                              .Cast<ScrollViewer>()
                              .Single();

      // allow interactions to perform some action when the ScrollViewer has been located
      // such as add event handlers
      ScrollViewerLocated(_scrollViewer);
    }

    protected virtual void ScrollViewerLocated(ScrollViewer scrollViewer)
    {
    }

    public virtual void AddElement(FrameworkElement rootElement)
    {
    }

    public bool IsActive
    {
      get
      {
        return _isActive;
      }
      set
      {
        _isActive = value;

        if (_isActive == true)
        {
          if (Activated != null)
          {
            Activated(this, EventArgs.Empty);
          }
        }
        else
        {
          if (DeActivated != null)
          {
            DeActivated(this, EventArgs.Empty);
          }
        }
      }
    }

    public bool IsEnabled { get; set; }
    
    public event EventHandler Activated;

    public event EventHandler DeActivated;

    /// <summary>
    /// Some interactions involve adding transformations or performing other visual modifications
    /// to items within the list. When the interaction is complete, we need to remove these and return
    /// the list to its original state. This method simply forces the ItemsControl to re-render all items.
    /// </summary>
    protected void RefreshView()
    {
      _todoItems.Reset();
    }
  }
}
