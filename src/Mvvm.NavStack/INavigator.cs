using System.Collections.Immutable;

namespace Mvvm.NavStack;

public interface INavigator
{
    IImmutableStack<NavEntry> BackStack { get; }
    NavEntry CurrentEntry { get; }
    
    bool CanGoBack();
    void OverrideBackStack(IEnumerable<Route> routes);
    void Navigate(Route route);
    bool GoBack();
    bool GoBackTo(Route route);
    void ClearAndSet(Route route);
    void ReplaceCurrent(Route route);
    
    event Action<IImmutableStack<NavEntry>>? BackStackChanged;
    event Action<NavEntry>? CurrentEntryChanged; 
}