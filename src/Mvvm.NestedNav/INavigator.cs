using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface INavigator : IDisposable
{
    IObservable<IImmutableStack<Screen>> Stack { get; }
    IImmutableStack<Screen> StackValue { get; }
    IObservable<Screen> CurrentScreen { get; }
    Screen CurrentScreenValue { get; }
    IObservable<IScreenViewModel> CurrentViewModel { get; }
    IScreenViewModel CurrentViewModelValue { get; }
    INavigator? ParentNavigator { get; }
    void Navigate(Screen screen);
    Task NavigateAsync(Screen screen);
    bool CanGoBack { get; }
    void GoBack();
    Task GoBackAsync();
}