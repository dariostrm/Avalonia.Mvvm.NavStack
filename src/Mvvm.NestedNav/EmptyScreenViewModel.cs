namespace Mvvm.NestedNav;

public class EmptyScreenViewModel(Screen screen, IScreenNavigator navigator) : IScreenViewModel
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Screen Screen { get; } = screen;
    public IScreenNavigator ScreenNavigator { get; } = navigator;
    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}