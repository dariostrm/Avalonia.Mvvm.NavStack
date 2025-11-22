using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface INavigator : IDisposable
{
    // Navigation Stack
    IObservable<IImmutableStack<Screen>> Stack { get; }
    IImmutableStack<Screen> StackValue { get; }
    IObservable<Screen> CurrentScreen { get; }
    Screen CurrentScreenValue { get; }
    IObservable<IScreenViewModel> CurrentViewModel { get; }
    IScreenViewModel CurrentViewModelValue { get; }
    
    INavigator? ParentNavigator { get; }
    bool CanGoBack { get; }
    
    // Navigation Methods
    void SetStack(IImmutableStack<Screen> newStack);
    Task SetStackAsync(IImmutableStack<Screen> newStack);
    void Navigate(Screen screen);
    Task NavigateAsync(Screen screen);
    void GoBack();
    Task GoBackAsync();
    
    //Hooks
    IObservable<NavigatingEventArgs> Navigating { get; }
    IObservable<NavigatedEventArgs> Navigated { get; }
    IObservable<NavigatingBackEventArgs> NavigatingBack { get; }
    IObservable<Screen> NavigatedBack { get; }
}

public record NavigatingEventArgs(Screen OldScreen, IScreenViewModel OldViewModel, Screen NewScreen);
public record NavigatedEventArgs(Screen OldScreen, Screen NewScreen, IScreenViewModel NewViewModel);
public record NavigatingBackEventArgs(Screen RemovingScreen, IScreenViewModel RemovingViewModel);