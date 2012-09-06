using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using SystemColor = System.Windows.Media.Color;

namespace ClearStyle.ViewModel
{
  /// <summary>
  /// A collection of todo items
  /// </summary>
  public class ToDoListViewModel
  {
    private ResettableObservableCollection<ToDoItemViewModel> _todoItems = new ResettableObservableCollection<ToDoItemViewModel>();

    public ToDoListViewModel()
    {
      _todoItems.CollectionChanged += (s, e) => UpdateToDoColors();
    }

    public ResettableObservableCollection<ToDoItemViewModel> Items
    {
      get
      {
        return _todoItems;
      }
    }
    
    private void UpdateToDoColors()
    {
      double itemCount = _todoItems.Count;
      double index = 0;
      foreach (var todoItem in _todoItems)
      {
        double val = (index / itemCount) * 155.0;
        index++;

        if (!todoItem.Completed)
        {
          todoItem.Color = SystemColor.FromArgb(255, 255, (byte)val, 0);
        }
      };
    }
  }
}
