using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mvvm.NestedNav.Dialogs;

public partial class DialogViewModelBase<TRoute> : ViewModelBase<TRoute>, IDialogViewModel
    where TRoute : DialogRoute
{
    [ObservableProperty] private string _title = "Dialog";

    [ObservableProperty] private string _primaryButtonText = "OK";
    [ObservableProperty] private ICommand _primaryCommand;

    [ObservableProperty] private string _secondaryButtonText = string.Empty;
    [ObservableProperty] private bool _isSecondaryButtonVisible;
    [ObservableProperty] private ICommand _secondaryCommand;

    [ObservableProperty] private string _closeButtonText = "Cancel";
    [ObservableProperty] private bool _isCloseButtonVisible = true;
    
    public event EventHandler? Closed;
    
    protected Action PrimaryAction { get; private set; }
    protected Func<bool> CanExecutePrimary { get; private set; } = () => true;

    protected Action SecondaryAction { get; private set; } = () => { };
    protected Func<bool> CanExecuteSecondary { get; private set; } = () => true;

    public DialogViewModelBase()
    {
        PrimaryAction = Close;
        PrimaryCommand = new RelayCommand(PrimaryAction, CanExecutePrimary);
        SecondaryCommand = new RelayCommand(SecondaryAction, CanExecuteSecondary);
    }
    
    public override void Initialize(INavigator navigator, TRoute route)
    {
        base.Initialize(navigator, route);
        Title = route.Title;
    }

    protected void Close()
    {
        if (Navigator.CanGoBack())
            Navigator.GoBack();
        else
            Closed?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public virtual void RequestClose() => Close();
    
    protected virtual void StateChanged()
    {
        ((IRelayCommand)PrimaryCommand).NotifyCanExecuteChanged();
        ((IRelayCommand)SecondaryCommand).NotifyCanExecuteChanged();
    }
    
    protected void SetPrimaryAction(Action action, Func<bool>? canExecute = null)
    {
        PrimaryAction = action;
        CanExecutePrimary = canExecute ?? (() => true);
        PrimaryCommand = new RelayCommand(PrimaryAction, CanExecutePrimary);
    }
    protected void SetSecondaryAction(Action action, Func<bool>? canExecute = null)
    {
        SecondaryAction = action;
        CanExecuteSecondary = canExecute ?? (() => true);
        SecondaryCommand = new RelayCommand(SecondaryAction, CanExecuteSecondary);
    }
    protected void SetPrimaryCanExecute(Func<bool> canExecute)
    {
        CanExecutePrimary = canExecute;
        PrimaryCommand = new RelayCommand(PrimaryAction, CanExecutePrimary);
    }
    protected void SetSecondaryCanExecute(Func<bool> canExecute) 
    {
        CanExecuteSecondary = canExecute;
        SecondaryCommand = new RelayCommand(SecondaryAction, CanExecuteSecondary);
    }
}

public abstract partial class DialogViewModel<TRoute, TResult> 
    : DialogViewModelBase<TRoute>, IDialogViewModel<TResult> 
    where TResult : class
    where TRoute : DialogRoute
{
    [ObservableProperty] private TResult? _result;

    partial void OnResultChanged(TResult? value)
    {
        StateChanged();
    }
}