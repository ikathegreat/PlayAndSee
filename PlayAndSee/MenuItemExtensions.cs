using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PlayAndSee
{
    public class MenuItemExtensions : DependencyObject
    {
        public static Dictionary<MenuItem, string> ElementToGroupNames = new Dictionary<MenuItem, string>();

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.RegisterAttached("GroupName",
                                         typeof(string),
                                         typeof(MenuItemExtensions),
                                         new PropertyMetadata(string.Empty, OnGroupNameChanged));

        public static void SetGroupName(MenuItem element, string value)
        {
            element.SetValue(GroupNameProperty, value);
        }

        public static string GetGroupName(MenuItem element)
        {
            return element.GetValue(GroupNameProperty).ToString();
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Add an entry to the group name collection

            if (!(d is MenuItem menuItem))
                return;

            var newGroupName = e.NewValue.ToString();
            var oldGroupName = e.OldValue.ToString();
            if (string.IsNullOrEmpty(newGroupName))
            {
                //Removing the toggle button from grouping
                RemoveCheckboxFromGrouping(menuItem);
            }
            else
            {
                //Switching to a new group
                if (newGroupName == oldGroupName)
                    return;

                if (!string.IsNullOrEmpty(oldGroupName))
                {
                    //Remove the old group mapping
                    RemoveCheckboxFromGrouping(menuItem);
                }
                ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                menuItem.Click += MenuItem_Click;
            }
        }
        private static void RemoveCheckboxFromGrouping(MenuItem checkBox)
        {
            ElementToGroupNames.Remove(checkBox);
            checkBox.Click -= MenuItem_Click;
        }

        private static void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            if (menuItem != null && menuItem.IsChecked)
            {
                foreach (var item in ElementToGroupNames)
                {
                    if (item.Key != menuItem && item.Value == GetGroupName(menuItem))
                    {
                        item.Key.IsChecked = false;
                    }
                }
            }
            else // it's not possible for the user to deselect an item
            {
                if (menuItem != null)
                    menuItem.IsChecked = true;
            }
        }

    }
}
