using System.Runtime.CompilerServices;

namespace Mvvm.NestedNav.Exceptions;

public class ViewModelNotLoadedException(string viewModelName, [CallerMemberName] string? callerName = null)
    : Exception($"The ViewModel '{viewModelName}' has not been loaded yet, cannot access the property '{callerName}'.");