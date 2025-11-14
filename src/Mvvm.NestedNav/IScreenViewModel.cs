using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IScreenViewModel : IViewModel, IAsyncDisposable
{
    Screen Screen { get; }
    Task LoadAsync(Screen screen, CancellationToken cancellationToken = default);
}