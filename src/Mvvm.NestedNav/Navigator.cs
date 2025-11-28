
using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public class Navigator : INavigator
{
    private readonly IViewModelFactory _viewModelFactory;
    
    public IImmutableStack<NavEntry> BackStack { get; private set; }
    public INavigator? ParentNavigator { get; }
    
    public event EventHandler<NavigatedEventArgs>? Navigated;
    
    public Navigator(IViewModelFactory viewModelFactory, Route initialRoute, INavigator? parentNavigator = null)
    {
        _viewModelFactory = viewModelFactory;
        ParentNavigator = parentNavigator;
        var initialEntry = CreateEntry(initialRoute);
        BackStack = ImmutableStack.Create(initialEntry);
    }

    public virtual bool CanGoBack() => BackStack.Count() > 1;

    public NavEntry OverrideBackStack(IEnumerable<Route> routes)
    {
        var newBackStack = routes
            .Select(CreateEntry)
            .Aggregate(ImmutableStack<NavEntry>.Empty, (backstack, entry) => backstack.Push(entry));
        return SetBackStack(newBackStack);
    }
    
    private NavEntry SetBackStack(IImmutableStack<NavEntry> newBackStack)
    {
        var oldEntry = BackStack.CurrentEntry();
        var oldRoute = oldEntry.Route;
        var newEntry = newBackStack.CurrentEntry();
        BackStack = newBackStack;
        oldEntry.OnNavigatedFrom();
        DestroyRemovedEntries(BackStack, newBackStack);
        newEntry.OnNavigatedTo();
        Navigated?.Invoke(this, new NavigatedEventArgs(oldRoute, newEntry));
        return newEntry;
    }
    
    private void DestroyRemovedEntries(IImmutableStack<NavEntry> oldStack, IImmutableStack<NavEntry> newStack)
    {
        var entriesToDestroy = oldStack.Except(newStack).ToList();
        foreach (var entry in entriesToDestroy)
        {
            entry.OnDestroy();
        }
    }

    public NavEntry Navigate(Route route)
    {
        var newBackStack = BackStack.Push(CreateEntry(route));
        return SetBackStack(newBackStack);
    }

    public NavEntry GoBack()
    {
        if (!CanGoBack())
            return BackStack.CurrentEntry();
        var newBackStack = BackStack.Pop();
        return SetBackStack(newBackStack);
    }

    public NavEntry GoBackTo(Route route)
    {
        if (!BackStack.Any(entry => entry.Route.Equals(route)))
            return BackStack.CurrentEntry();
        var newBackStack = BackStack;
        while (!newBackStack.CurrentEntry().Route.Equals(route))
        {
            newBackStack = newBackStack.Pop();
        }
        return SetBackStack(newBackStack);
    }

    public NavEntry ClearAndSet(Route route) => OverrideBackStack([route]);
    

    public NavEntry ReplaceCurrent(Route route)
    {
        var newBackStack = BackStack.Pop();
        newBackStack = newBackStack.Push(CreateEntry(route));
        return SetBackStack(newBackStack);
    }
    
    private NavEntry CreateEntry(Route route)
    {
        var vm = _viewModelFactory.CreateViewModel(route, this);
        return new NavEntry(route, vm);
    }
}