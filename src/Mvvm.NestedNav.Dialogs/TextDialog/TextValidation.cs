namespace Mvvm.NestedNav.Dialogs.TextDialog;

public record TextValidation(
    bool Required,
    int? MinLength = null,
    int? MaxLength = null,
    string? Pattern = null,
    string? CustomErrorMessage = null
)
{
    public static TextValidation None => new TextValidation(false);
    
    public static TextValidation RequiredField => new TextValidation(true);
    
    public static TextValidation LengthBetween(int minLength, int maxLength) =>
        new TextValidation(minLength > 0, minLength, maxLength);
}