using Microsoft.Extensions.DependencyInjection;

namespace Mvvm.NestedNav;

public class ServiceProviderViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    private readonly Dictionary<Type, Func<IServiceProvider, IScreenViewModel>> _factories = new();

    public void Register<TScreen, TViewModel>()
        where TScreen : Screen
        where TViewModel : IScreenViewModel
    {
        _factories[typeof(TScreen)] = sp => ActivatorUtilities.CreateInstance<TViewModel>(sp);
    }

    public async Task<IScreenViewModel> CreateAndLoadAsync(Screen screen, INavigator nav, CancellationToken ct)
    {
        if (!_factories.TryGetValue(screen.GetType(), out var factory))
            throw new InvalidOperationException($"No ViewModel registered for {screen.GetType().Name}");

        var vm = factory(serviceProvider);
        vm.Initialize(nav, screen);
        await vm.LoadAsync(ct);
        return vm;
    }
}