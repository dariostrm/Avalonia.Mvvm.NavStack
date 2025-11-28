namespace Mvvm.NestedNav.Dialogs.TextDialog;

public record TextDialogRoute(
    string Title,
    string InputLabel,
    TextValidation Validation,
    string InitialText = "",
    string? Placeholder = null,
    bool IsPassword = false
) : DialogRoute(Title);