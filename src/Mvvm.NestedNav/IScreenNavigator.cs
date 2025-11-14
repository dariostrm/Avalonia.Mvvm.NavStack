using System.Collections.Immutable;
using NestedNav;

namespace Mvvm.NestedNav;

public interface IScreenNavigator : INavigator
{
    IImmutableStack<Screen> Stack { get; }
    void Navigate(Route route);
}