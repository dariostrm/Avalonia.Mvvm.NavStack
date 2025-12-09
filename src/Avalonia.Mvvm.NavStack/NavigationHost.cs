using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Mvvm.NavStack;

namespace Avalonia.Mvvm.NavStack;



public class NavigationHost : ContentControl
{
    private TopLevel? _topLevel;

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
        get => _currentViewModel;
        set => SetAndRaise(CurrentViewModelProperty, ref _currentViewModel!, value);
    }

    public static readonly StyledProperty<IPageTransition> PageTransitionProperty = AvaloniaProperty.Register<NavigationHost, IPageTransition>(
        nameof(PageTransition), defaultValue: new ScaleTransition());

    public IPageTransition PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    public NavigationHost()
    {
        var transitioningContentControl = new TransitioningContentControl
        {
            [!ContentProperty] = this[!CurrentViewModelProperty],
            [!PageTransitionProperty] = this[!PageTransitionProperty],
            PageTransition = PageTransition
        };
        Content = transitioningContentControl;
    }

    public NavigationHost(Route initialRoute, IViewModelFactory viewModelFactory)
        : this()
    {
        InitialRoute = initialRoute;
        ViewModelFactory = viewModelFactory;
    }

    private void SetCurrentViewModel(IViewModel? viewModel)
    {
        Dispatcher.UIThread.Post(() => CurrentViewModel = viewModel);
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
        _topLevel = TopLevel.GetTopLevel(this);
        if (_topLevel != null)
            _topLevel.BackRequested += OnBackRequested;
        Navigator = new Navigator(ViewModelFactory, InitialRoute);
        SetCurrentViewModel(Navigator.CurrentEntry.ViewModel);
        Navigator.CurrentEntryChanged += OnCurrentEntryChanged;
    }

    private void OnBackRequested(object? sender, RoutedEventArgs e)
    {
        if (Navigator.CanGoBack())
        {
            Navigator.GoBack();
            e.Handled = true;
        }
    }

    private void OnCurrentEntryChanged(NavEntry newEntry)
    {
        var newViewModel = newEntry.ViewModel;
        SetCurrentViewModel(newViewModel);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        Navigator.CurrentEntryChanged -= OnCurrentEntryChanged;
        if (_topLevel != null)
            _topLevel.BackRequested -= OnBackRequested;
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