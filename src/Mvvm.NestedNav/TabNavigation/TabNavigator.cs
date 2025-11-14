using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav.TabNavigation;

public class TabNavigator : ITabNavigator
{
    private readonly BehaviorSubject<IImmutableDictionary<string, Route>> _tabsSubject;
    private readonly BehaviorSubject<string> _currentTabKeySubject;

    // Emits the current screen of the current tab whenever either
    // the tabs,
    // the current tab key,
    // or the current screen of the current tab changes
    public IObservable<Route> CurrentRoute =>
        _tabsSubject.CombineLatest(_currentTabKeySubject,
            (tabs, tabKey) => tabs[tabKey]);
        

    public Route CurrentRouteValue => _tabsSubject.Value[_currentTabKeySubject.Value];
    public INavigator? ParentNavigator { get; }
    public bool GoBack()
    {
        return ParentNavigator?.GoBack() ?? false;
    }

    public IObservable<IImmutableDictionary<string, Route>> Tabs => _tabsSubject.AsObservable();
    public IObservable<string> CurrentTabKey => _currentTabKeySubject.AsObservable();

    public TabNavigator(IDictionary<string, Route> tabs, string initialTabKey, INavigator? parentNavigator)
    {
        ParentNavigator = parentNavigator;
        if (tabs.Count == 0)
            throw new ArgumentException("Tab routes cannot be empty.");
        _tabsSubject = new BehaviorSubject<IImmutableDictionary<string, Route>>(tabs.ToImmutableDictionary());
        _currentTabKeySubject = new BehaviorSubject<string>(initialTabKey);
    }
    
    public void NavigateToTab(string tabKey)
    {
        if (!_tabsSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' does not exist.");
        }
        _currentTabKeySubject.OnNext(tabKey);
    }

    public void AddTab(string tabKey, Route tabRoute)
    {
        if (_tabsSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' already exists.");
        }
        var updatedTabScreens = _tabsSubject.Value.Add(tabKey, tabRoute);
        _tabsSubject.OnNext(updatedTabScreens);
    }

    public void RemoveTab(string tabKey)
    {
        if (!_tabsSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' does not exist.");
        }
        var updatedTabScreens = _tabsSubject.Value.Remove(tabKey);
        _tabsSubject.OnNext(updatedTabScreens);
        
        // If the removed tab was the current tab, switch to another tab if available
        if (_currentTabKeySubject.Value != tabKey || updatedTabScreens.Count <= 0) 
            return;
        var newCurrentTabKey = updatedTabScreens.Keys.First();
        _currentTabKeySubject.OnNext(newCurrentTabKey);
    }
}