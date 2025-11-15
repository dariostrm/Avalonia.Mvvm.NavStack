using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface IScreenNavigator : INavigator
{
    IImmutableStack<Screen> Stack { get; }
    void Navigate(Screen screen);
}