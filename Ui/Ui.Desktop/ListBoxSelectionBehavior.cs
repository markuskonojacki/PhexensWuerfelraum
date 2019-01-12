using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PhexensWuerfelraum.Ui.Desktop
{
    // https://stackoverflow.com/a/5144092/7557790
    public static class ListBoxSelectionBehavior
    {
        public static readonly DependencyProperty ClickSelectionProperty =
            DependencyProperty.RegisterAttached("ClickSelection",
                                                typeof(bool),
                                                typeof(ListBoxSelectionBehavior),
                                                new UIPropertyMetadata(false, OnClickSelectionChanged));

        public static bool GetClickSelection(DependencyObject obj)
        {
            return (bool)obj.GetValue(ClickSelectionProperty);
        }

        public static void SetClickSelection(DependencyObject obj, bool value)
        {
            obj.SetValue(ClickSelectionProperty, value);
        }

        private static void OnClickSelectionChanged(DependencyObject dpo,
                                                                 DependencyPropertyChangedEventArgs e)
        {
            if (dpo is ListBox listBox)
            {
                if ((bool)e.NewValue == true)
                {
                    listBox.SelectionMode = SelectionMode.Multiple;
                    listBox.SelectionChanged += OnSelectionChanged;
                }
                else
                {
                    listBox.SelectionChanged -= OnSelectionChanged;
                }
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox listBox = sender as ListBox;
                var valid = e.AddedItems[0];
                foreach (var item in new ArrayList(listBox.SelectedItems))
                {
                    if (item != valid)
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
            }
        }
    }
}