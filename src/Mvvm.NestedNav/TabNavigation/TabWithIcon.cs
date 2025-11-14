namespace Mvvm.NestedNav.TabNavigation;

public record TabWithIcon (
    string Name,
    Route Route,
    ITabIcon Icon
) : Tab(Name, Route);