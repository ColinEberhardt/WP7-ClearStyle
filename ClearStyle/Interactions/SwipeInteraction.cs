using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClearStyle.ViewModel;
using LinqToVisualTree;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Input;

namespace ClearStyle.Interactions
{
  /// <summary>
  /// Adds an interaction that allows the user to swipe an item to mark it as complete
  /// or delete it
  /// </summary>
  public class SwipeInteraction : InteractionBase, IInteraction
  {
    private static readonly double FlickVelocity = 2000.0;

    // the drag distance required to consider this a swipe interaction
    private static readonly double DragStartedDistance = 5.0;

    private FrameworkElement _tickAndCrossContainer;
    private SoundEffect _completeSound;
    private SoundEffect _deleteSound;

    public SwipeInteraction()
    {
      _completeSound = SoundEffect.FromStream(TitleContainer.OpenStream("Sounds/Windows XP Exclamation.wav"));
      _deleteSound = SoundEffect.FromStream(TitleContainer.OpenStream("Sounds/Windows XP Notify.wav"));
    }

    public override void AddElement(FrameworkElement element)
    {
      element.ManipulationDelta += Element_ManipulationDelta;
      element.ManipulationCompleted += Element_ManipulationCompleted;
    }

    private void Element_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!IsActive)
        return;
      
      FrameworkElement fe = sender as FrameworkElement;
      if (Math.Abs(e.TotalManipulation.Translation.X) > fe.ActualWidth / 2 ||
        Math.Abs(e.FinalVelocities.LinearVelocity.X) > FlickVelocity)
      {
        if (e.TotalManipulation.Translation.X < 0.0)
        {
          ToDoItemDeletedAction(fe);
        }
        else
        {
          ToDoItemCompletedAction(fe);
        }
      }
      else
      {
        ToDoItemBounceBack(fe);
      }

      IsActive = false;
    }

    private void Element_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (!IsEnabled)
        return;

      if (!IsActive)
      { 
        // has the user dragged far enough?
        if (Math.Abs(e.CumulativeManipulation.Translation.X) < DragStartedDistance)
          return;

        IsActive = true;

        // initialize the drag
        FrameworkElement fe = sender as FrameworkElement;
        fe.SetHorizontalOffset(0);

        // find the container for the tick and cross graphics
        _tickAndCrossContainer = fe.Descendants()
                                   .OfType<FrameworkElement>()
                                   .Single(i => i.Name == "tickAndCross");
      }
      else
      {
        // handle the drag to offset the element
        FrameworkElement fe = sender as FrameworkElement;
        double offset = fe.GetHorizontalOffset().Value + e.DeltaManipulation.Translation.X;
        fe.SetHorizontalOffset(offset);

        _tickAndCrossContainer.Opacity = TickAndCrossOpacity(offset);
      }
    }


    private double TickAndCrossOpacity(double offset)
    {
      offset = Math.Abs(offset);
      if (offset < 50)
        return 0;

      offset -= 50;
      double opacity = offset / 100;

      opacity = Math.Max(Math.Min(opacity, 1), 0);
      return opacity;
    }

    private void ToDoItemBounceBack(FrameworkElement fe)
    {
      var trans = fe.GetHorizontalOffset().Transform;

      trans.Animate(trans.X, 0, TranslateTransform.XProperty, 300, 0, new BounceEase()
      {
        Bounciness = 5,
        Bounces = 2
      });
    }

    private void ToDoItemDeletedAction(FrameworkElement deletedElement)
    {
      _deleteSound.Play();

      var trans = deletedElement.GetHorizontalOffset().Transform;
      trans.Animate(trans.X, -(deletedElement.ActualWidth + 50),
                    TranslateTransform.XProperty, 300, 0, new SineEase()
                    {
                      EasingMode = EasingMode.EaseOut
                    },
      () =>
      {
        // find the model object that was deleted
        ToDoItemViewModel deletedItem = deletedElement.DataContext as ToDoItemViewModel;

        // determine how much we have to 'shuffle' up by
        double elementOffset = -deletedElement.ActualHeight;

        // find the items in view, and the location of the deleted item in this list
        var itemsInView = _todoList.GetItemsInView().ToList();
        var lastItem = itemsInView.Last();
        int startTime = 0;
        int deletedItemIndex = itemsInView.Select(i => i.DataContext)
                                          .ToList().IndexOf(deletedItem);

        // iterate over each item
        foreach (FrameworkElement element in itemsInView.Skip(deletedItemIndex))
        {
          // for the last item, create an action that deletes the model object
          // and re-renders the list
          Action action = null;
          if (element == lastItem)
          {
            action = () =>
            {
                // remove the item
              _todoItems.Remove(deletedItem);

              // re-populate our ObservableCollection
              _todoItems.Reset();    
            };
          }

          // shuffle this item up
          TranslateTransform elementTrans = new TranslateTransform();
          element.RenderTransform = elementTrans;
          elementTrans.Animate(0, elementOffset, TranslateTransform.YProperty, 200, startTime, null, action);
          startTime += 10;
        }
      });
    }

    private void ToDoItemCompletedAction(FrameworkElement fe)
    {
      // set the mode object to complete
      ToDoItemViewModel completedItem = fe.DataContext as ToDoItemViewModel;
      completedItem.Completed = true;
      completedItem.Color = Colors.Green;

      // bounce back into place
      ToDoItemBounceBack(fe);

      _completeSound.Play();
    }
  }
}
