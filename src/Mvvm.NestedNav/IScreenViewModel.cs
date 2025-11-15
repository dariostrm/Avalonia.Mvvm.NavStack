using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IScreenViewModel : IViewModel, IAsyncDisposable
{
    Screen Screen { get; }
    IScreenNavigator ScreenNavigator { get; }
    Task LoadAsync(CancellationToken cancellationToken = default);
}