using System.Collections.Immutable;

namespace Mvvm.NestedNav.TabNavigation;

public interface ITabNavigator : INavigator
{
    IObservable<IImmutableDictionary<string, Screen>> Tabs { get; }
    IObservable<string> CurrentTabKey { get; }
    void NavigateToTab(string tabKey);
    void AddTab(string tabKey, Screen tabScreen);
    void RemoveTab(string tabKey);
}