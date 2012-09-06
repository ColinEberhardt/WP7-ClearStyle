using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ClearStyle
{
  public static class Util
  {
    public static void Animate(this DependencyObject target, double? from, double to,
                              object propertyPath, int duration, int startTime,
                              IEasingFunction easing = null, Action completed = null)
    {
      if (easing == null)
      {
        easing = new SineEase();
      }

      var db = new DoubleAnimation();
      db.To = to;
      db.From = from;
      db.EasingFunction = easing;
      db.Duration = TimeSpan.FromMilliseconds(duration);
      Storyboard.SetTarget(db, target);
      Storyboard.SetTargetProperty(db, new PropertyPath(propertyPath));

      var sb = new Storyboard();
      sb.BeginTime = TimeSpan.FromMilliseconds(startTime);

      if (completed != null)
      {
        sb.Completed += (s, e) => completed();
      }

      sb.Children.Add(db);
      sb.Begin();
    }

    public static void SetHorizontalOffset(this FrameworkElement fe, double offset)
    {
      var translateTransform = fe.RenderTransform as TranslateTransform;
      if (translateTransform == null)
      {
        // create a new transform if one is not alreayd present
        var trans = new TranslateTransform()
        {
          X = offset
        };
        fe.RenderTransform = trans;
      }
      else
      {
        translateTransform.X = offset;
      }
    }

    public static Offset GetHorizontalOffset(this FrameworkElement fe)
    {
      var trans = fe.RenderTransform as TranslateTransform;
      if (trans == null)
      {
        // create a new transform if one is not alreayd present
        trans = new TranslateTransform()
        {
          X = 0
        };
        fe.RenderTransform = trans;
      }
      return new Offset()
      {
        Transform = trans,
        Value = trans.X
      };
    }

    public static void SetVerticalOffset(this FrameworkElement fe, double offset)
    {
      var translateTransform = fe.RenderTransform as TranslateTransform;
      if (translateTransform == null)
      {
        var trans = new TranslateTransform()
        {
          Y = offset
        };
        fe.RenderTransform = trans;
      }
      else
      {
        translateTransform.Y = offset;
      }
    }

    public static Offset GetVerticalOffset(this FrameworkElement fe)
    {
      var trans = fe.RenderTransform as TranslateTransform;
      if (trans == null)
      {
        trans = new TranslateTransform()
        {
          Y = 0
        };
        fe.RenderTransform = trans;
      }
      return new Offset()
      {
        Transform = trans,
        Value = trans.Y
      };
    }

    public struct Offset
    {
      public double Value { get; set; }
      public TranslateTransform Transform { get; set; }
    }

    public static void InvokeOnNextLayoutUpdated(this FrameworkElement element, Action action)
    {
      EventHandler handler = null;
      handler = (s, e2) =>
      {
        element.LayoutUpdated -= handler;
        action();
      };
      element.LayoutUpdated += handler;
    }
  }
}
