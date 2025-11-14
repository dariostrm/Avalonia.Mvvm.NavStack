
namespace Mvvm.NestedNav.Exceptions;

public class InvalidRouteException(string viewModelName)
    : Exception($"The route is invalid for the view model '{viewModelName}'.");