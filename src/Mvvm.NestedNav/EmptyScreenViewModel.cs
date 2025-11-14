namespace Mvvm.NestedNav;

public class EmptyScreenViewModel(IScreenNavigator navigator) : IScreenViewModel
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Screen Screen { get; } = Screen.Empty(navigator);
    
    public Task LoadAsync(Screen screen, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}