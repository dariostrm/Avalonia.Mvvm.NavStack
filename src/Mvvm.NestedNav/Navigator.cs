
using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public class Navigator : INavigator
{
    private readonly IViewModelResolver _viewModelResolver;
    
    public IImmutableList<NavEntry> BackStack { get; private set; }
    public INavigator? ParentNavigator { get; }
    
    public event EventHandler<NavigatingEventArgs>? Navigating;
    public event EventHandler<NavigatedEventArgs>? Navigated;
    
    public Navigator(IViewModelResolver viewModelResolver, Screen initialScreen, INavigator? parentNavigator = null)
    {
        _viewModelResolver = viewModelResolver;
        ParentNavigator = parentNavigator;
        var initialEntry = CreateEntry(initialScreen);
        BackStack = ImmutableList.Create(initialEntry);
    }

    public virtual bool CanGoBack() => BackStack.Count > 1;

    public void OverrideBackStack(IEnumerable<Screen> screens)
    {
        var newBackStack = screens
            .Select(CreateEntry)
            .Aggregate(ImmutableList<NavEntry>.Empty, (backstack, entry) => backstack.Add(entry));
        SetBackStack(newBackStack);
    }
    
    private void SetBackStack(IImmutableList<NavEntry> newBackStack)
    {
        var oldEntry = BackStack.CurrentEntry();
        var oldScreen = oldEntry.Screen;
        oldEntry.ViewModel.OnNavigatingFrom();
        var newEntry = newBackStack.CurrentEntry();
        CheckForClosingViewModels(BackStack, newBackStack);
        Navigating?.Invoke(this, new NavigatingEventArgs(oldScreen, oldEntry.ViewModel, newEntry.Screen));
        BackStack = newBackStack;
        oldEntry.ViewModel.OnNavigatedFrom();
        newEntry.ViewModel.OnNavigatedTo();
        Navigated?.Invoke(this, new NavigatedEventArgs(oldScreen, newEntry.Screen, newEntry.ViewModel));
    }
    
    private void CheckForClosingViewModels(IImmutableList<NavEntry> oldStack, IImmutableList<NavEntry> newStack)
    {
        var entriesToClose = oldStack.Except(newStack).ToList();
        foreach (var entry in entriesToClose)
        {
            entry.ViewModel.OnClosing();
        }
    }

    public void Navigate(Screen screen)
    {
        var newBackStack = BackStack.Add(CreateEntry(screen));
        SetBackStack(newBackStack);
    }

    public void GoBack()
    {
        if (!CanGoBack())
            return;
        var newBackStack = BackStack.RemoveAt(BackStack.Count - 1);
        SetBackStack(newBackStack);
    }

    public void GoBackTo(Screen screen)
    {
        if (!BackStack.Any(entry => entry.Screen.Equals(screen)))
            return;
        var newBackStack = BackStack.ToList();
        while (!newBackStack.Last().Screen.Equals(screen))
        {
            newBackStack.RemoveAt(newBackStack.Count - 1);
        }
        SetBackStack(newBackStack.ToImmutableList());
    }

    public void ClearAndSet(Screen screen) => OverrideBackStack([screen]);
    

    public void ReplaceCurrent(Screen screen)
    {
        var newBackStack = BackStack.RemoveAt(BackStack.Count - 1);
        newBackStack = newBackStack.Add(CreateEntry(screen));
        SetBackStack(newBackStack);
    }
    
    private NavEntry CreateEntry(Screen screen)
    {
        var vm = _viewModelResolver.ResolveViewModel(screen);
        vm.Initialize(this, screen);
        return new NavEntry(screen, vm);
    }
}