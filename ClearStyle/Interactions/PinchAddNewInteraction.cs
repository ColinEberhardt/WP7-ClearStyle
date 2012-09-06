using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using ClearStyle.ViewModel;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace ClearStyle.Interactions
{
  public class PinchAddNewInteraction : InteractionBase
  {
    private static readonly double ToDoItemHeight = 75;

    private int _itemOneIndex;
    private int _itemTwoIndex;

    private double _initialDelta;

    private double _newItemLocation;

    private bool _addNewThresholdReached;

    private TapEditInteraction _editInteraction;
    private PullDownItem _pullDownItem;

    private SoundEffect _popSound;

    private bool _effectPlayed = false;

    public PinchAddNewInteraction(TapEditInteraction editInteraction, PullDownItem pullDownItem)
    {
      _editInteraction = editInteraction;
      _pullDownItem = pullDownItem;

      _popSound = SoundEffect.FromStream(Microsoft.Xna.Framework.TitleContainer.OpenStream("Sounds/pop.wav"));
    }

    public override void Initialise(ItemsControl todoList, ResettableObservableCollection<ToDoItemViewModel> todoItems)
    {
      base.Initialise(todoList, todoItems);

      Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
    }

    private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
    {
      if (!IsEnabled)
        return;

      if (IsActive)
      {
        var touchPoints = e.GetTouchPoints(_todoList);

        // if we still have two touch points continue the pinch gesture
        if (touchPoints.Count == 2)
        {
          double currentDelta = GetDelta(touchPoints[0], touchPoints[1]);

          double itemsOffset = 0;

          // is the delta bigger than the initial?
          if (currentDelta > _initialDelta)
          {
            double delta = currentDelta - _initialDelta;
            itemsOffset = delta / 2;

            // play a sound effect if the users has pinched far enough to add a new item
            if (delta > ToDoItemHeight && !_effectPlayed)
            {
              _effectPlayed = true;
              _popSound.Play();
            }

            _addNewThresholdReached = delta > ToDoItemHeight;

            // stretch and fade in the new item
            var cappedDelta = Math.Min(ToDoItemHeight, delta);
            ((ScaleTransform)_pullDownItem.RenderTransform).ScaleY = cappedDelta / ToDoItemHeight;
            _pullDownItem.Opacity = cappedDelta / ToDoItemHeight;

            // set the text
            _pullDownItem.Text = cappedDelta < ToDoItemHeight ? "Pull to create new item" : "Release to add new item";
          }
          
          // offset all the items in the list so that they 'part'
          for (int i = 0; i < _todoItems.Count; i++)
          {
            var container = _todoList.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
            var translateTransform = (TranslateTransform)container.RenderTransform;
            translateTransform.Y = i <= _itemOneIndex ? -itemsOffset : itemsOffset;
          }
        }
        else
        {
          // if we no longer have two touch points, end the interactions
          IsActive = false;

          RefreshView();

          // hide the pull-down item
          _pullDownItem.VerticalOffset = -ToDoItemHeight;

          if (_addNewThresholdReached)
          {
            var newItem = new ToDoItemViewModel("");
            _todoItems.Insert(_itemOneIndex, newItem);
          
            // when the new item has been rendered, use the edit interaction to place the UI
            // into edit mode
            _todoList.InvokeOnNextLayoutUpdated(() => _editInteraction.EditItem(newItem));
          }
        }
      }
      else
      {
        var touchPoints = e.GetTouchPoints(_todoList);
        if (touchPoints.Count == 2)
        {
          _addNewThresholdReached = false;
          _effectPlayed = false;

          // find the items that were touched ...
          var itemOne = GetToDoItemAtLocation(touchPoints[0].Position);
          var itemTwo = GetToDoItemAtLocation(touchPoints[1].Position);

          if (itemOne != null && itemTwo != null)
          {
            // find their indices
            _itemOneIndex = _todoItems.IndexOf(itemOne);
            _itemTwoIndex = _todoItems.IndexOf(itemTwo);

            // are the two items next to each other?
            if (Math.Abs(_itemOneIndex - _itemTwoIndex) == 1)
            {
              IsActive = true;

              // determine where to locate the new item placeholder
              var itemOneContainer = _todoList.ItemContainerGenerator.ContainerFromItem(itemOne) as FrameworkElement;
              var itemOneContainerPos = itemOneContainer.GetRelativePosition(_todoList);
              _newItemLocation = itemOneContainerPos.Y + ToDoItemHeight - (ToDoItemHeight / 2);

              // position the placeholder and add a scale transform
              _pullDownItem.VerticalOffset = _newItemLocation;
              _pullDownItem.Opacity = 0;
              _pullDownItem.RenderTransform = new ScaleTransform()
              {
                ScaleY = 1,
                CenterY = ToDoItemHeight / 2
              };

              // record the initial distance between touch point
              _initialDelta = GetDelta(touchPoints[0], touchPoints[1]);

              AddTranslateTransfromToElements();

              _pullDownItem.Opacity = 1;
            }
          }
        }
      }
    }

    private double GetDelta(TouchPoint tpOne, TouchPoint tpTwo)
    {
      double tpOneYPos = tpOne.Position.Y;
      double tpTwoYPos = tpTwo.Position.Y;

      return tpOneYPos > tpTwoYPos ? tpOneYPos - tpTwoYPos : tpTwoYPos - tpOneYPos;
    }

    private void AddTranslateTransfromToElements()
    {
      foreach (var item in _todoItems)
      {
        var container = _todoList.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
        container.RenderTransform = new TranslateTransform();
      }
    }

    private ToDoItemViewModel GetToDoItemAtLocation(Point location)
    {
      var elements = VisualTreeHelper.FindElementsInHostCoordinates(location, _todoList);
      Border border = elements.OfType<Border>()
                              .Where(i => i.Name == "todoItem")
                              .SingleOrDefault();
      
      return border != null ? border.DataContext as ToDoItemViewModel : null;
    }


  }
}
