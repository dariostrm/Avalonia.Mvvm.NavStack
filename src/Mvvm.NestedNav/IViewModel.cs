namespace Mvvm.NestedNav;

public interface IViewModel : IAsyncDisposable
{
    Screen Screen { get; }
    INavigator Navigator { get; }
    Task LoadAsync(CancellationToken cancellationToken = default);
}