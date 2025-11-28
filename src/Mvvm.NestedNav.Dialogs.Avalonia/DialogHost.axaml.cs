using Avalonia;
using Avalonia.Controls.Primitives;

namespace Mvvm.NestedNav.Dialogs.Avalonia;

public class DialogHost : TemplatedControl, IDialogHost
{
    // Null if no dialog is shown
    private INavigator? _navigator;

    public static readonly StyledProperty<IViewModelFactory> ViewModelFactoryProperty = AvaloniaProperty.Register<DialogHost, IViewModelFactory>(
        nameof(ViewModelFactory));

    public IViewModelFactory ViewModelFactory
    {
        get => GetValue(ViewModelFactoryProperty);
        set => SetValue(ViewModelFactoryProperty, value);
    }
    
    private bool _isDialogOpen;

    public static readonly DirectProperty<DialogHost, bool> IsDialogOpenProperty = AvaloniaProperty.RegisterDirect<DialogHost, bool>(
        nameof(IsDialogOpen), o => o.IsDialogOpen, (o, v) => o.IsDialogOpen = v);

    public bool IsDialogOpen
    {
        get => _isDialogOpen;
        set => SetAndRaise(IsDialogOpenProperty, ref _isDialogOpen, value);
    }

    private IDialogViewModel? _currentDialog;

    public static readonly DirectProperty<DialogHost, IDialogViewModel?> CurrentDialogProperty 
        = AvaloniaProperty.RegisterDirect<DialogHost, IDialogViewModel?>
        (
            nameof(CurrentDialog), 
            o => o.CurrentDialog, 
            (o, v) => o.CurrentDialog = v
        );

    public IDialogViewModel? CurrentDialog
    {
        get => _currentDialog;
        set => SetAndRaise(CurrentDialogProperty, ref _currentDialog, value);
    }
    
    public void ShowDialog(DialogRoute dialogRoute, Action onClosed)
    {
        NavEntry currentEntry;
        if (!IsDialogOpen)
        {
            _navigator = new Navigator(ViewModelFactory, dialogRoute);
            _navigator.Navigated += OnNavigated;
            currentEntry = _navigator.BackStack.CurrentEntry();
            CurrentDialog = (IDialogViewModel)currentEntry.ViewModel;
            CurrentDialog.Closed += OnLastDialogClosed;
            CurrentDialog.Closed += (_, _) => onClosed();
            IsDialogOpen = true;
        }
        else
        {
            currentEntry = _navigator!.Navigate(dialogRoute);
            currentEntry.Destroyed += (_, _) => onClosed();
        }
    }

    private void OnNavigated(object? sender, NavigatedEventArgs e)
    {
        var newEntry = e.NewEntry;
        CurrentDialog = (IDialogViewModel)newEntry.ViewModel;
    }

    private void OnLastDialogClosed(object? sender, EventArgs e)
    {
        IsDialogOpen = false;
        _navigator = null;
        CurrentDialog = null;
    }

    public async Task ShowDialogAsync(DialogRoute dialogRoute)
    {
        var tcs = new TaskCompletionSource();
        
        ShowDialog(dialogRoute, () => tcs.SetResult());
        
        await tcs.Task;
    }
    
    public void ShowDialog<TDialogResult>(DialogRoute dialogRoute, Action<TDialogResult?> onClosed) 
        where TDialogResult : class
    {
        throw new NotImplementedException();
    }

    public async Task<TDialogResult?> ShowDialogAsync<TDialogResult>(DialogRoute dialogRoute) 
        where TDialogResult : class
    {
        var tcs = new TaskCompletionSource<TDialogResult?>();
        
        ShowDialog<TDialogResult>(dialogRoute, result => tcs.SetResult(result));
        
        return await tcs.Task;
    }
    
}