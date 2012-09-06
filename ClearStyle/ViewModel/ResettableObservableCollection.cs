using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;


namespace ClearStyle.ViewModel
{
  public class ResettableObservableCollection<T> : ObservableCollection<T>
  {
    public void Reset()
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Reset));
    }
  }
}
