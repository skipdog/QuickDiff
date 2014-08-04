using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Helpers
{

    public class SelectedIndexSynchronizer : DependencyObject
    {

        private static Dictionary<Selector, string> selectors =
            new Dictionary<Selector, string>();

        private static Dictionary<string, int> indexes =
            new Dictionary<string, int>();

        public static readonly DependencyProperty IndexGroupProperty = DependencyProperty.RegisterAttached(
                "IndexGroup", typeof(string), typeof(SelectedIndexSynchronizer), new PropertyMetadata(new PropertyChangedCallback(
                OnIndexGroupChanged)));

        public static void SetIndexGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(IndexGroupProperty, scrollGroup);
        }

        public static string GetIndexGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(IndexGroupProperty);
        }

        private static void OnIndexGroupChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;            
            if (selector != null)
            {
                if (!string.IsNullOrEmpty((string)e.OldValue))
                {
                    if (selectors.ContainsKey(selector))
                    {
                        selector.SelectionChanged -= selector_SelectionChanged;
                        selectors.Remove(selector);
                    }
                }
                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    if (indexes.Keys.Contains((string)e.NewValue))
                    {
                        selector.SelectedIndex = indexes[(string)e.NewValue];
                    }
                    else
                    {
                        indexes.Add((string)e.NewValue, selector.SelectedIndex);
                    }
                    selectors.Add(selector, (string)e.NewValue);
                    selector.SelectionChanged += new SelectionChangedEventHandler(selector_SelectionChanged);
                }
            }
        }

        static void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var changedSelector = sender as Selector;
            if (changedSelector != null)
            {
                Select(changedSelector);
            }
        }

        private static void Select(Selector changedSelector)
        {
            var group = selectors[changedSelector];
            indexes[group] = changedSelector.SelectedIndex;
            foreach (var listControl in selectors.Where((s) => s.Value ==
                                              group && s.Key != changedSelector))
            {
                if (listControl.Key.SelectedIndex != changedSelector.SelectedIndex)
                {
                    listControl.Key.SelectedIndex = changedSelector.SelectedIndex;
                    if (changedSelector.SelectedIndex >= 0)
                        listControl.Key.SelectedItem = changedSelector.Items[changedSelector.SelectedIndex];
                }                
            }
        }
    }

    public class ScrollSynchronizer : DependencyObject
    {
        private static Dictionary<ScrollViewer, string> scrollViewers =
            new Dictionary<ScrollViewer, string>();

        private static Dictionary<string, double> horizontalScrollOffsets =
            new Dictionary<string, double>();

        private static Dictionary<string, double> verticalScrollOffsets =
            new Dictionary<string, double>();

        public static readonly DependencyProperty ScrollGroupProperty = DependencyProperty.RegisterAttached(
        "ScrollGroup", typeof(string), typeof(ScrollSynchronizer), new PropertyMetadata(new PropertyChangedCallback(
        OnScrollGroupChanged)));

        public static void SetScrollGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(ScrollGroupProperty, scrollGroup);
        }

        public static string GetScrollGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(ScrollGroupProperty);
        }

        private static void OnScrollGroupChanged(DependencyObject d,
                            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;            

            if (scrollViewer != null)
            {
                if (!string.IsNullOrEmpty((string)e.OldValue))
                {
                    // Remove scrollviewer
                    if (scrollViewers.ContainsKey(scrollViewer))
                    {
                        scrollViewer.ScrollChanged -=
                          new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                        scrollViewers.Remove(scrollViewer);
                    }
                }

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    // If group already exists, set scrollposition of 
                    // new scrollviewer to the scrollposition of the group
                    if (horizontalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToHorizontalOffset(
                                      horizontalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        horizontalScrollOffsets.Add((string)e.NewValue,
                                                scrollViewer.HorizontalOffset);
                    }

                    if (verticalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToVerticalOffset(verticalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        verticalScrollOffsets.Add((string)e.NewValue, scrollViewer.VerticalOffset);
                    }

                    // Add scrollviewer
                    scrollViewers.Add(scrollViewer, (string)e.NewValue);
                    scrollViewer.ScrollChanged +=
                        new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                }
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0 || e.HorizontalChange != 0)
            {
                var changedScrollViewer = sender as ScrollViewer;
                Scroll(changedScrollViewer);
            }
        }

        private static void Scroll(ScrollViewer changedScrollViewer)
        {
            var group = scrollViewers[changedScrollViewer];
            verticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;
            horizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

            foreach (var scrollViewer in scrollViewers.Where((s) => s.Value ==
                                              group && s.Key != changedScrollViewer))
            {
                if (scrollViewer.Key.VerticalOffset != changedScrollViewer.VerticalOffset)
                {
                    scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
                }

                if (scrollViewer.Key.HorizontalOffset != changedScrollViewer.HorizontalOffset)
                {
                    scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
                }
            }
        }
    }

    public static class DataGridColumnResize
    {

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(DataGridLengthUnitType), typeof(DataGridColumnResize),
                                                new PropertyMetadata(OnSetWidthCallback));

        public static readonly DependencyProperty GridViewColumnResizeBehaviorProperty =
            DependencyProperty.RegisterAttached("GridViewColumnResizeBehavior",
                                                typeof(GridViewColumnResizeBehavior), typeof(DataGridColumnResize), null);

        public static DataGridLengthUnitType GetWidth(DependencyObject obj)
        {
            return (DataGridLengthUnitType)obj.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject obj, DataGridLengthUnitType value)
        {
            obj.SetValue(WidthProperty, value);
        }

        private static void OnSetWidthCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumn element = dependencyObject as DataGridColumn;
            if (element != null)
            {
                GridViewColumnResizeBehavior behavior = GetOrCreateBehavior(element);
                DataGridLengthUnitType dglut = (DataGridLengthUnitType)e.NewValue;
                behavior.Width = dglut;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type GridViewColumn but found " +
                                        dependencyObject.GetType().Name);
            }
        }

        private static GridViewColumnResizeBehavior GetOrCreateBehavior(DataGridColumn element)
        {
            GridViewColumnResizeBehavior behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior;
            if (behavior == null)
            {
                behavior = new GridViewColumnResizeBehavior(element);
                element.SetValue(GridViewColumnResizeBehaviorProperty, behavior);
            }

            return behavior;
        }
        
        public class GridViewColumnResizeBehavior : IDisposable
        {
            private readonly DataGridColumn column;
            private readonly DispatcherTimer dt = null;
            public DataGridLengthUnitType Width { get; set; }

            public GridViewColumnResizeBehavior(DataGridColumn column)
            {
                this.column = column;
                dt = new DispatcherTimer();
                dt.Tick += new EventHandler(dt_Tick);
                dt.Interval = new TimeSpan(0, 0, 3);
                dt.Start();
            }

            void dt_Tick(object sender, EventArgs e)
            {
                column.Width = -1;
                column.Width = new DataGridLength(0, Width);
            }

            public void Dispose()
            {
                dt.Tick -= new EventHandler(dt_Tick);
            }
        }
    }
}
