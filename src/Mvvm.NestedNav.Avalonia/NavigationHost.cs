using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Mvvm.NestedNav.Avalonia;

public class NavigationHost : ContentControl, INavigationHost
{
    private readonly CompositeDisposable _disposables = new();

    public static readonly StyledProperty<INavigator> NavigatorProperty = AvaloniaProperty.Register<NavigationHost, INavigator>(
        nameof(Navigator));

    public static readonly StyledProperty<Screen> InitialScreenProperty = AvaloniaProperty.Register<NavigationHost, Screen>(
        nameof(InitialScreen));

    public static readonly StyledProperty<IViewModel?> CurrentViewModelProperty = AvaloniaProperty.Register<NavigationHost, IViewModel?>(
        nameof(CurrentViewModel));

    public NavigationHost()
    {
        
    }

    public NavigationHost(Screen initialScreen)
        : this()
    {
        InitialScreen = initialScreen;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.GetObservable(CurrentViewModelProperty)
            .Subscribe(OnCurrentViewModelChanged)
            .DisposeWith(_disposables);
        Navigator = new Navigator(InitialScreen, ViewModelFactory.Instance, parentNavigator: null);
        Navigator.CurrentViewModel
            .Subscribe(OnNewViewModel)
            .DisposeWith(_disposables);
    }

    private void OnCurrentViewModelChanged(IViewModel? vm)
    {
        Dispatcher.UIThread.Post(() => Content = vm);
    }

    private void OnNewViewModel(IViewModel? vm)
    {
        CurrentViewModel = vm;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _disposables.Dispose();
        base.OnDetachedFromVisualTree(e);
    }
    
    public INavigator Navigator
    {
        get => GetValue(NavigatorProperty);
        set => SetValue(NavigatorProperty, value);
    }
    public IViewModel? CurrentViewModel
    {
        get => GetValue(CurrentViewModelProperty);
        private set => SetValue(CurrentViewModelProperty, value);
    }
    public Screen InitialScreen
    {
        get => GetValue(InitialScreenProperty);
        set => SetValue(InitialScreenProperty, value);
    }
}