using Mvvm.NestedNav.TabNavigation;

namespace Mvvm.NestedNav;

public abstract record Screen
{
    public static Screen Empty => new EmptyScreen();
}

public record EmptyScreen() : Screen;

public record ScreenWithTabs(
    IDictionary<string, Screen> Tabs,
    string InitialTab
) : Screen;