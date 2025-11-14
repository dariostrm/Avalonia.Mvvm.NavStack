using System.Collections.Immutable;

namespace Mvvm.NestedNav.TabNavigation;

public interface ITabNavigator : INavigator
{
    IObservable<IEnumerable<Tab>> Tabs { get; }
    IObservable<IImmutableDictionary<string, Screen>> TabScreens { get; }
    IObservable<string> CurrentTabKey { get; }
    void NavigateToTab(string tabKey);
    void AddTab(Tab tab);
    void RemoveTab(string tabKey);
}