using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Mvvm.NestedNav.Avalonia;

public class NavigationHost : ContentControl, INavigationHost
{
    private readonly CompositeDisposable _disposables = new();
    
    public static readonly StyledProperty<IViewModelFactory> ViewModelFactoryProperty = 
        AvaloniaProperty.Register<NavigationHost, IViewModelFactory>(nameof(ViewModelFactory));

    public static readonly StyledProperty<INavigator> NavigatorProperty = AvaloniaProperty.Register<NavigationHost, INavigator>(
        nameof(Navigator));

    public static readonly StyledProperty<IViewModel?> CurrentViewModelProperty = AvaloniaProperty.Register<NavigationHost, IViewModel?>(
        nameof(CurrentViewModel));

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Navigator.CurrentViewModel
            .Subscribe(OnNewViewModel)
            .DisposeWith(_disposables);
    }

    private void OnNewViewModel(IViewModel? vm)
    {
        Dispatcher.UIThread.Post(() => CurrentViewModel = vm);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _disposables.Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    public IViewModelFactory ViewModelFactory
    {
        get => GetValue(ViewModelFactoryProperty);
        set => SetValue(ViewModelFactoryProperty, value);
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
}