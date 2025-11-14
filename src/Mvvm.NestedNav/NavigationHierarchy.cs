using Mvvm.NestedNav.TabNavigation;

namespace Mvvm.NestedNav;

public static class NavigationHierarchy
{
    public static INavigator? GetChildNavigator(this INavigator navigator)
    {
        //For tab navigators, the child navigator is always the current screen's navigator
        if (navigator is ITabNavigator)
        {
            return navigator.CurrentScreenValue.Navigator;
        }
        //Screen navigators only have a child navigator if the current screen is a ScreenWithTabs
        if (navigator.CurrentScreenValue is ScreenWithTabs screenWithTabs)
        {
            return screenWithTabs.TabNavigator;
        }
        return null;
    }
    
    /// <summary>
    /// Gets the deepest child navigator in the hierarchy (which would handle physical back button presses for example).
    /// </summary>
    /// <returns>The deepest child navigator</returns>
    public static INavigator GetCurrentNavigator(INavigator rootNavigator)
    {
        var currentNavigator = rootNavigator;
        while (true)
        {
            var childNavigator = currentNavigator.GetChildNavigator();
            if (childNavigator == null)
            {
                return currentNavigator;
            }
            currentNavigator = childNavigator;
        }
    }
    
    public static bool HandleBackPress(INavigator rootNavigator)
    {
        var currentNavigator = GetCurrentNavigator(rootNavigator);
        return currentNavigator.GoBack();
    }
}