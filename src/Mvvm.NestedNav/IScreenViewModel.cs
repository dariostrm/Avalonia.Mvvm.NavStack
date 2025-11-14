using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IScreenViewModel : IViewModel, IAsyncDisposable
{
    Route Route { get; }
    IScreenNavigator ScreenNavigator { get; }
    Task LoadAsync(CancellationToken cancellationToken = default);
}