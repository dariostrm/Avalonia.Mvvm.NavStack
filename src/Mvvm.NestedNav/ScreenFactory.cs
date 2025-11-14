using Mvvm.NestedNav.TabNavigation;
using NestedNav;

namespace Mvvm.NestedNav;

public class ScreenFactory : IScreenFactory
{
    public virtual Screen CreateScreen(Route route, IScreenNavigator navigator)
    {
        if (route is RouteToScreenWithTabs routeWithTabs)
        {
            return CreateScreenWithTabs(routeWithTabs, navigator);
        }
        return new Screen(route, navigator);
    }

    public virtual Screen CreateTabScreen(Route route, ITabNavigator navigator)
    {
        var screenNavigator = new ScreenNavigator(route, navigator, this);
        return screenNavigator.CurrentScreenValue;
    }

    protected virtual Screen CreateScreenWithTabs(RouteToScreenWithTabs route, IScreenNavigator navigator)
    {
        var tabNavigator = new TabNavigator(route.Tabs, route.InitialTab, navigator, this);
        return new ScreenWithTabs(route, navigator, tabNavigator);
    }
}