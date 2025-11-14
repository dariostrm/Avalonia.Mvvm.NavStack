namespace Mvvm.NestedNav.TabNavigation;

public record ScreenWithTabs(
    Route Route,
    IScreenNavigator Navigator,
    ITabNavigator TabNavigator
) : Screen(Route, Navigator);