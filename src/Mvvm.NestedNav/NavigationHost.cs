using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
namespace Mvvm.NestedNav;

public class NavigationHost : INavigationHost
{
    protected IViewModelFactory VmFactory { get; }
    protected INavigator Navigator { get; }
    
    private IScreenViewModel? _currentViewModel = null!;
    public virtual IScreenViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            SetField(ref _currentViewModel, value);
            OnPropertyChanged(nameof(Content));
        }
    }

    public virtual object? Content => CurrentViewModel;

    protected CompositeDisposable Subscriptions { get; } = new();
    private CancellationTokenSource _screenCts = new();
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public NavigationHost(INavigator navigator, IViewModelFactory vmFactory)
    {
        VmFactory = vmFactory;
        Navigator = navigator;
        Navigator.CurrentScreen
            .Subscribe(OnNewScreen)
            .DisposeWith(Subscriptions);
    }

    protected virtual async void OnNewScreen(Screen newScreen)
    {
        await _screenCts.CancelAsync();
        _screenCts.Dispose();
        _screenCts = new CancellationTokenSource();
        var ct = _screenCts.Token;

        try
        {
            if (CurrentViewModel != null)
            {
                await CurrentViewModel.DisposeAsync();
                if (ct.IsCancellationRequested) return;
            }
            var vm = await VmFactory.CreateAndLoadAsync(newScreen, ct);
            if (ct.IsCancellationRequested) return;

            CurrentViewModel = vm;
        }
        catch (OperationCanceledException) { /* ignore */ }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        await _screenCts.CancelAsync();
        if (CurrentViewModel != null)
        {
            await CurrentViewModel.DisposeAsync();
            CurrentViewModel = null;
        }
        await CastAndDispose(_screenCts);
        await CastAndDispose(Subscriptions);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}