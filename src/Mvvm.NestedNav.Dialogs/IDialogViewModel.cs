using System.Windows.Input;

namespace Mvvm.NestedNav.Dialogs;

public interface IDialogViewModel : IScreenViewModel
{
    string Title { get; }
    
    string PrimaryButtonText { get; }
    ICommand? PrimaryCommand { get; }
    
    string SecondaryButtonText { get; }
    bool IsSecondaryButtonVisible { get; }
    ICommand? SecondaryCommand { get; }
    
    bool IsCloseButtonVisible { get; }
    ICommand? RequestCloseCommand { get; }
    void RequestClose();
}

public interface IDialogViewModel<TResult> : IDialogViewModel
    where TResult : class
{
    TResult? Result { get; }
    ICommand CancelCommand { get; }
}