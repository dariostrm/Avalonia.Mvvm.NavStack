using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface INavigator
{
    IImmutableList<NavEntry> BackStack { get; }
    
    INavigator? ParentNavigator { get; }
    
    // Navigation Methods
    bool CanGoBack();
    void OverrideBackStack(IEnumerable<Screen> screens);
    void Navigate(Screen screen);
    void GoBack();
    void GoBackTo(Screen screen);
    void ClearAndSet(Screen screen);
    void ReplaceCurrent(Screen screen);
    
    event EventHandler<NavigatingEventArgs>? Navigating;
    event EventHandler<NavigatedEventArgs>? Navigated;
}

public record NavigatingEventArgs(Screen OldScreen, IScreenViewModel OldViewModel, Screen NewScreen);
public record NavigatedEventArgs(Screen OldScreen, Screen NewScreen, IScreenViewModel NewViewModel);