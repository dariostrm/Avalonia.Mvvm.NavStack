using System.Collections.Immutable;

namespace Mvvm.NavStack;

public class Navigator : INavigator
{
    private IViewModelFactory ViewModelFactory { get; }
    
    public IImmutableStack<NavEntry> BackStack { get; private set; }
    public NavEntry CurrentEntry => BackStack.Peek();
    
    public event Action<IImmutableStack<NavEntry>>? BackStackChanged;
    public event Action<NavEntry>? CurrentEntryChanged;
    
    public Navigator(IViewModelFactory viewModelFactory, Route initialRoute)
    {
        ViewModelFactory = viewModelFactory;
        var initialEntry = CreateEntry(initialRoute);
        BackStack = ImmutableStack.Create(initialEntry);
    }

    public bool CanGoBack() => BackStack.Count() > 1;

    public void OverrideBackStack(IEnumerable<Route> routes)
    {
        var routeList = routes.ToList();
        if (routeList.Count == 0)
            throw new ArgumentException("Back stack cannot be empty.", nameof(routes));
        var newBackStack = routeList
            .Select(CreateEntry)
            .Aggregate(ImmutableStack<NavEntry>.Empty, (backstack, entry) => backstack.Push(entry));
        SetBackStack(newBackStack);
    }
    
    private void SetBackStack(IImmutableStack<NavEntry> newBackStack)
    {
        var oldEntry = CurrentEntry;
        BackStack = newBackStack;
        oldEntry.OnNavigatedFrom();
        DestroyRemovedEntries(BackStack, newBackStack);
        CurrentEntry.OnNavigatedTo();
        BackStackChanged?.Invoke(BackStack);
        CurrentEntryChanged?.Invoke(CurrentEntry);
    }
    
    private void DestroyRemovedEntries(IImmutableStack<NavEntry> oldStack, IImmutableStack<NavEntry> newStack)
    {
        var entriesToDestroy = oldStack.Except(newStack).ToList();
        foreach (var entry in entriesToDestroy)
        {
            entry.OnDestroy();
        }
    }

    public void Navigate(Route route)
    {
        var newBackStack = BackStack.Push(CreateEntry(route));
        SetBackStack(newBackStack);
    }

    public bool GoBack()
    {
        if (!CanGoBack())
            return false;
        var newBackStack = BackStack.Pop();
        SetBackStack(newBackStack);
        return true;
    }
    
    /// <exception cref="InvalidOperationException">Thrown if the route does not exist in the back stack.</exception>
    public bool GoBackTo(Route route)
    {
        if (!BackStack.Any(entry => entry.Route.Equals(route)))
            return false;
        var newBackStack = BackStack;
        while (!newBackStack.Peek().Route.Equals(route))
        {
            newBackStack = newBackStack.Pop();
        }
        SetBackStack(newBackStack);
        return true;
    }

    public void ClearAndSet(Route route) => OverrideBackStack([route]);
    

    public void ReplaceCurrent(Route route)
    {
        var newBackStack = BackStack.Pop();
        newBackStack = newBackStack.Push(CreateEntry(route));
        SetBackStack(newBackStack);
    }
    
    protected NavEntry CreateEntry(Route route)
    {
        var vm = ViewModelFactory.CreateViewModel(route, this);
        return new NavEntry(route, vm);
    }
}