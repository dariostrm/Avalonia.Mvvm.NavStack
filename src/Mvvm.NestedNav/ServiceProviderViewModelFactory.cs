using Microsoft.Extensions.DependencyInjection;

namespace Mvvm.NestedNav;

public class ServiceProviderViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    private readonly Dictionary<Type, Func<IServiceProvider, IViewModel>> _factories = new();

    public void Register<TScreen, TViewModel>()
        where TScreen : Screen
        where TViewModel : IViewModel
    {
        _factories[typeof(TScreen)] = sp => ActivatorUtilities.CreateInstance<TViewModel>(sp);
    }

    public async Task<IViewModel> CreateAndLoadAsync(Screen screen, INavigator nav, CancellationToken ct)
    {
        if (!_factories.TryGetValue(screen.GetType(), out var factory))
            throw new InvalidOperationException($"No ViewModel registered for {screen.GetType().Name}");

        var vm = factory(serviceProvider);
        await vm.LoadAsync(nav, screen, ct);
        return vm;
    }
}