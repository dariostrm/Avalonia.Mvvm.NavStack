using System.Collections.Immutable;

namespace Mvvm.NestedNav.TabNavigation;

public interface ITabNavigator : INavigator
{
    IObservable<IImmutableDictionary<string, Route>> Tabs { get; }
    IObservable<string> CurrentTabKey { get; }
    void NavigateToTab(string tabKey);
    void AddTab(string tabKey, Route tabRoute);
    void RemoveTab(string tabKey);
}