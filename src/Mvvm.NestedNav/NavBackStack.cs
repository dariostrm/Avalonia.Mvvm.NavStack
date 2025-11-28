using System.Collections.Immutable;

namespace Mvvm.NestedNav;

public static class NavBackStack
{
    public static NavEntry CurrentEntry(this IImmutableList<NavEntry> entries)
    {
        if (entries.Count == 0)
        {
            throw new InvalidOperationException("The navigation back stack is empty.");
        }
        return entries[^1];
    }
    
    public static Screen CurrentScreen(this IImmutableList<NavEntry> entries)
    {
        return entries.CurrentEntry().Screen;
    }
    
    public static IScreenViewModel CurrentViewModel(this IImmutableList<NavEntry> entries) 
    {
        return entries.CurrentEntry().ViewModel;
    }
}