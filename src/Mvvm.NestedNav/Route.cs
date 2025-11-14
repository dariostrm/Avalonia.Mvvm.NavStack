using Mvvm.NestedNav.TabNavigation;

namespace Mvvm.NestedNav;

public abstract record Route
{
    public static Route Empty => new EmptyRoute();
}

public record EmptyRoute() : Route;

public record RouteToScreenWithTabs(
    IEnumerable<Tab> Tabs,
    string InitialTab
) : Route;