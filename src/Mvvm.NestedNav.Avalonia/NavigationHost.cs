using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Mvvm.NestedNav.Avalonia;

public class NavigationHost : ContentControl
{
    public static readonly StyledProperty<INavigator> NavigatorProperty = AvaloniaProperty.Register<NavigationHost, INavigator>(
        nameof(Navigator));

    public static readonly StyledProperty<Route> InitialRouteProperty = AvaloniaProperty.Register<NavigationHost, Route>(
        nameof(InitialRoute));

    public static readonly StyledProperty<IViewModelFactory> ViewModelFactoryProperty = AvaloniaProperty.Register<NavigationHost, IViewModelFactory>(
        nameof(ViewModelFactory));

    public IViewModelFactory ViewModelFactory
    {
        get => GetValue(ViewModelFactoryProperty);
        set => SetValue(ViewModelFactoryProperty, value);
    }

    private IViewModel? _currentViewModel;

    public static readonly DirectProperty<NavigationHost, IViewModel?> CurrentViewModelProperty = AvaloniaProperty.RegisterDirect<NavigationHost, IViewModel?>(
        nameof(CurrentViewModel), o => o.CurrentViewModel, (o, v) => o.CurrentViewModel = v);

    public IViewModel? CurrentViewModel
    {
        get => _currentViewModel ?? throw new InvalidOperationException("NavigationHost not initialized yet.");
        set => SetAndRaise(CurrentViewModelProperty, ref _currentViewModel!, value);
    }

    public NavigationHost()
    {
        
    }

    public NavigationHost(Route initialRoute, IViewModelFactory viewModelFactory)
        : this()
    {
        InitialRoute = initialRoute;
        ViewModelFactory = viewModelFactory;
    }

    private void SetCurrentViewModel(IViewModel? viewModel)
    {
        CurrentViewModel = viewModel;
        Dispatcher.UIThread.Post(() => Content = viewModel);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if(this.TryFindResource("ViewModelFactory", out object? factory) && factory is IViewModelFactory vmFactory)
        {
            ViewModelFactory = vmFactory;
        }
        
        if (InitialRoute is null)
            throw new InvalidOperationException("The " + nameof(InitialRoute) +" has not been set on the navigation host.");
        if (ViewModelFactory is null)
            throw new InvalidOperationException("The " + nameof(ViewModelFactory) + " has not been set on the navigation host.");
        Navigator = new Navigator(ViewModelFactory, InitialRoute);
        SetCurrentViewModel(Navigator.CurrentEntry.ViewModel);
        Navigator.BackStackChanged += OnBackStackChanged;
    }

    private void OnBackStackChanged(IImmutableStack<NavEntry> backStack)
    {
        var newViewModel = backStack.Peek().ViewModel;
        SetCurrentViewModel(newViewModel);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        Navigator.BackStackChanged -= OnBackStackChanged;
        base.OnDetachedFromLogicalTree(e);
    }

    public INavigator Navigator
    {
        get => GetValue(NavigatorProperty);
        set => SetValue(NavigatorProperty, value);
    }
    public Route InitialRoute
    {
        get => GetValue(InitialRouteProperty);
        set => SetValue(InitialRouteProperty, value);
    }
}