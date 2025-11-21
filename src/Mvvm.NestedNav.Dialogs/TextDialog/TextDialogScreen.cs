namespace Mvvm.NestedNav.Dialogs.TextDialog;

public record TextDialogScreen(
    string Title,
    string InputLabel,
    TextValidation Validation,
    string InitialText = "",
    string? Placeholder = null,
    bool IsPassword = false
) : DialogScreen<string>(Title);