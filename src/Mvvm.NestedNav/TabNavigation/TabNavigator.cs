using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mvvm.NestedNav.TabNavigation;

public class TabNavigator : ITabNavigator
{
    private IScreenFactory _screenFactory;
    
    private readonly BehaviorSubject<IEnumerable<Tab>> _tabsSubject;
    private readonly BehaviorSubject<IImmutableDictionary<string, Screen>> _tabScreensSubject;
    private readonly BehaviorSubject<string> _currentTabKeySubject;

    // Emits the current screen of the current tab whenever either
    // the tabs,
    // the current tab key,
    // or the current screen of the current tab changes
    public IObservable<Screen> CurrentScreen => 
        _tabScreensSubject.CombineLatest(_currentTabKeySubject, 
            (tabs, tabKey) => tabs[tabKey].Navigator.CurrentScreen)
            .Switch();
        

    public Screen CurrentScreenValue => _tabScreensSubject.Value[_currentTabKeySubject.Value];
    public INavigator? ParentNavigator { get; }
    public bool GoBack()
    {
        return ParentNavigator?.GoBack() ?? false;
    }

    public IObservable<IEnumerable<Tab>> Tabs => _tabsSubject.AsObservable();
    public IObservable<IImmutableDictionary<string, Screen>> TabScreens => _tabScreensSubject.AsObservable();
    public IObservable<string> CurrentTabKey => _currentTabKeySubject.AsObservable();

    public TabNavigator(IEnumerable<Tab> tabs, string initialTabKey, INavigator? parentNavigator, IScreenFactory screenFactory)
    {
        _screenFactory = screenFactory;
        ParentNavigator = parentNavigator;
        var tabsList = tabs.ToList();
        if (tabsList.Count == 0)
            throw new ArgumentException("Tab routes cannot be empty.");
        _tabsSubject = new BehaviorSubject<IEnumerable<Tab>>(tabsList);
        var tabsDict = tabsList.ToImmutableDictionary(
            keySelector: tab => tab.Name,
            elementSelector: tab => _screenFactory.CreateTabScreen(tab.Route, this)
        );
        _tabScreensSubject = new BehaviorSubject<IImmutableDictionary<string, Screen>>(tabsDict);
        _currentTabKeySubject = new BehaviorSubject<string>(initialTabKey);
    }
    
    public void NavigateToTab(string tabKey)
    {
        if (!_tabScreensSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' does not exist.");
        }
        _currentTabKeySubject.OnNext(tabKey);
    }

    public void AddTab(Tab tab)
    {
        var tabKey = tab.Name;
        var screen = _screenFactory.CreateTabScreen(tab.Route, this);
        if (_tabScreensSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' already exists.");
        }
        var updatedTabScreens = _tabScreensSubject.Value.Add(tabKey, screen);
        _tabScreensSubject.OnNext(updatedTabScreens);
        var updatedTabs = _tabsSubject.Value.Append(tab);
        _tabsSubject.OnNext(updatedTabs);
    }

    public void RemoveTab(string tabKey)
    {
        if (!_tabScreensSubject.Value.ContainsKey(tabKey))
        {
            throw new ArgumentException($"Tab with key '{tabKey}' does not exist.");
        }
        var updatedTabScreens = _tabScreensSubject.Value.Remove(tabKey);
        _tabScreensSubject.OnNext(updatedTabScreens);
        var updatedTabs = _tabsSubject.Value.Where(tab => tab.Name != tabKey);
        _tabsSubject.OnNext(updatedTabs);
        
        // If the removed tab was the current tab, switch to another tab if available
        if (_currentTabKeySubject.Value == tabKey && updatedTabScreens.Count > 0)
        {
            var newCurrentTabKey = updatedTabScreens.Keys.First();
            _currentTabKeySubject.OnNext(newCurrentTabKey);
        }
    }
}