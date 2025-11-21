namespace Mvvm.NestedNav.Dialogs;

public interface IDialogHost
{
    bool IsDialogOpen { get; }
    
    void ShowDialog<TDialogResult>(
        DialogScreen<TDialogResult> screenToDialog,
        Action<TDialogResult?> onClosed
    ) where TDialogResult : class;
    
    Task<TDialogResult?> ShowDialogAsync<TDialogResult>(DialogScreen<TDialogResult> screenToDialog) 
        where TDialogResult : class;
}