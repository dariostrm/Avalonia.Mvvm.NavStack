using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public interface IScreenNavigator : INavigator
{
    IImmutableStack<Route> Stack { get; }
    void Navigate(Route route);
}