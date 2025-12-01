
namespace Mvvm.NestedNav.Exceptions;

public class InvalidScreenException(string viewModelName)
    : Exception($"The screen is invalid for the view model '{viewModelName}'.");