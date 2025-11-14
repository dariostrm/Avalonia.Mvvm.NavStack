using Mvvm.NestedNav;

namespace Mvvm.NestedNav;

public interface IViewModelFactory
{
    Task<IScreenViewModel> CreateAndLoadAsync(Screen screen, CancellationToken cancellationToken = default);
}