using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface INavigator
{
    IImmutableStack<NavEntry> BackStack { get; }
    
    INavigator? ParentNavigator { get; }
    
    // Navigation Methods
    bool CanGoBack();
    NavEntry OverrideBackStack(IEnumerable<Route> routes);
    NavEntry Navigate(Route route);
    NavEntry GoBack();
    NavEntry GoBackTo(Route route);
    NavEntry ClearAndSet(Route route);
    NavEntry ReplaceCurrent(Route route);
    
    event EventHandler<NavigatedEventArgs>? Navigated;
}

public record NavigatedEventArgs(Route OldRoute, NavEntry NewEntry);