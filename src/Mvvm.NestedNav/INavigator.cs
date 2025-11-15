using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface INavigator
{
    IObservable<IImmutableStack<Screen>> Stack { get; }
    IImmutableStack<Screen> StackValue { get; }
    IObservable<Screen> CurrentScreen { get; }
    Screen CurrentScreenValue { get; }
    INavigator? ParentNavigator { get; }
    void Navigate(Screen screen);
    bool GoBack();
}