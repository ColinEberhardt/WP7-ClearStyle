using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using LinqToVisualTree;

namespace ClearStyle
{
  public static class ItemsControlExtensions
  {
    /// <summary>
    /// Enumerates all the items that are currently visible in an ItemsControl. This
    /// implementation works for both virtualized and non-virtualized panels.
    /// </summary>
    public static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl)
    {
      // find the panel that hosts our items - this is 'cached'
      // using the ItemsControl.Tag property to minimize visual tree
      // navigation
      Panel itemsHostPanel = itemsControl.Tag as Panel;
      if (itemsHostPanel == null)
      {
        itemsHostPanel = itemsControl.Descendants<Panel>()
                                    .Cast<Panel>()
                                    .Where(p => p.IsItemsHost)
                                    .SingleOrDefault();
        itemsControl.Tag = itemsHostPanel;
      }

      VirtualizingStackPanel vsp = itemsHostPanel as VirtualizingStackPanel;
      if (vsp != null)
      {
        // implementation for virtualizing lists
        return GetItemsInView(itemsControl, vsp);
      }
      else
      {
        // implementation for non-virtualizing lists
        return Enumerable.Range(0, itemsControl.Items.Count)
                      .Select(index => itemsControl.ItemContainerGenerator.ContainerFromIndex(index))
                      .Cast<FrameworkElement>()
                      .Where(container => container.GetRelativePosition(itemsControl).Y + container.ActualHeight > 0)
                      .Where(container => container.GetRelativePosition(itemsControl).Y - container.ActualHeight < itemsControl.ActualHeight);
      }
    }

    /// <summary>
    /// Gets the items in view for a virtualizing list
    /// </summary>
    private static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl, VirtualizingStackPanel vsp)
    {
      // iterate over each of the items in view
      int firstVisibleItem = (int)vsp.VerticalOffset;
      int visibleItemCount = (int)vsp.ViewportHeight;
      for (int index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount + 3; index++)
      {
        var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
        if (item == null)
          continue;

        yield return item;
      }
    }

  }
}
