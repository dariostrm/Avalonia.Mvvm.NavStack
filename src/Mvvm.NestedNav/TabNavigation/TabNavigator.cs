using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav.TabNavigation;

public class TabNavigator : ITabNavigator
{
    private readonly BehaviorSubject<IImmutableDictionary<string, Screen>> _tabsSubject;
    private readonly BehaviorSubject<string> _currentTabKeySubject;

    // Emits the current screen of the current tab whenever either
    // the tabs,
    // the current tab key,
    // or the current screen of the current tab changes
    public IObservable<Screen> CurrentScreen =>
        _tabsSubject.CombineLatest(_currentTabKeySubject,
            (tabs, tabKey) => tabs[tabKey]);
        

    public Screen CurrentScreenValue => _tabsSubject.Value[_currentTabKeySubject.Value];
    public INavigator? ParentNavigator { get; }
    public bool GoBack()
    {
        return ParentNavigator?.GoBack() ?? false;
    }

    public IObservable<IImmutableDictionary<string, Screen>> Tabs => _tabsSubject.AsObservable();
    public IObservable<string> CurrentTabKey => _currentTabKeySubject.AsObservable();

    public TabNavigator(IDictionary<string, Screen> tabs, string initialTabKey, INavigator? parentNavigator)
    {
        ParentNavigator = parentNavigator;
        if (tabs.Count == 0)
            throw new ArgumentException("Tabs cannot be empty.");
        _tabsSubject = new BehaviorSubject<IImmutableDictionary<string, Screen>>(tabs.ToImmutableDictionary());
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

    public void AddTab(string tabKey, Screen tabScreen)
    {
        if (_tabsSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' already exists.");
        }
        var updatedTabScreens = _tabsSubject.Value.Add(tabKey, tabScreen);
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