// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
// --------------------------------------------------------------------------------------------------------------------

namespace ClearStyle
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public static class UIElementExtensions
    {
        /// <summary>
        /// Gets the relative position of the given UIElement to this.
        /// </summary>
        public static Point GetRelativePosition(this UIElement element, UIElement other)
        {
            return element.TransformToVisual(other)
                          .Transform(new Point(0, 0));
        }


    }
}