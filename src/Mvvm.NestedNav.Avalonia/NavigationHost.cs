using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Mvvm.NestedNav.Avalonia;

public class NavigationHost : ContentControl
{
    private IDisposable _navigatorSubscription = Disposable.Empty;
    private CompositeDisposable _disposables = new();

    public static readonly StyledProperty<INavigator> NavigatorProperty = AvaloniaProperty.Register<NavigationHost, INavigator>(
        nameof(Navigator));

    public static readonly StyledProperty<Screen?> InitialScreenProperty = AvaloniaProperty.Register<NavigationHost, Screen?>(
        nameof(InitialScreen));

    private IScreenViewModel? _currentViewModel;

    public static readonly DirectProperty<NavigationHost, IScreenViewModel> CurrentViewModelProperty = AvaloniaProperty.RegisterDirect<NavigationHost, IScreenViewModel>(
        nameof(CurrentViewModel), o => o.CurrentViewModel, (o, v) => o.CurrentViewModel = v);

    public IScreenViewModel CurrentViewModel
    {
        get => _currentViewModel ?? throw new InvalidOperationException("Current ViewModel has not been loaded yet.");
        set => SetAndRaise(CurrentViewModelProperty, ref _currentViewModel!, value);
    }

    public NavigationHost()
    {
        
    }

    public NavigationHost(Screen initialScreen)
        : this()
    {
        InitialScreen = initialScreen;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _disposables = new CompositeDisposable();
        this.GetObservable(CurrentViewModelProperty)
            .Subscribe(OnCurrentViewModelChanged)
            .DisposeWith(_disposables);
        Navigator = new Navigator(ViewModelResolver.Instance, InitialScreen, parentNavigator: null);
        _navigatorSubscription = Navigator.BackStack.Subscribe(OnBackStackChanged);
    }

    private void OnCurrentViewModelChanged(IScreenViewModel vm)
    {
        Dispatcher.UIThread.Post(() => Content = vm);
    }

    private void OnBackStackChanged(NavBackStack stack)
    {
        if (stack.CurrentViewModel is null)
        {
            Console.WriteLine("NavigationHost: Current ViewModel is null.");
            return;
        }

        _disposables.Clear();
        stack.CurrentViewModel.LifecycleState
            .Where(state => state == ViewModelLifecycleState.Active)
            .Subscribe(OnViewModelLoaded)
            .DisposeWith(_disposables);

        void OnViewModelLoaded(ViewModelLifecycleState state)
        {
            CurrentViewModel = stack.CurrentViewModel;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _navigatorSubscription.Dispose();
        _disposables.Dispose();
        base.OnDetachedFromLogicalTree(e);
    }
    
    public INavigator Navigator
    {
        get => GetValue(NavigatorProperty);
        set => SetValue(NavigatorProperty, value);
    }
    public Screen? InitialScreen
    {
        get => GetValue(InitialScreenProperty);
        set => SetValue(InitialScreenProperty, value);
    }
}