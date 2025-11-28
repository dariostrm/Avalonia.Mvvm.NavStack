using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Mvvm.NestedNav.Exceptions;

namespace Mvvm.NestedNav.Dialogs.TextDialog;

public partial class TextDialogViewModel : DialogViewModel<TextDialogRoute, string>
{
    [CustomValidation(typeof(TextDialogViewModel), nameof(ValidateText))]
    [ObservableProperty] private string _text = string.Empty;
    
    public string Placeholder { get; set; } = string.Empty;
    public char? PasswordChar { get; set; } = null;
    public string InputLabel { get; set; } = string.Empty;
    
    public TextValidation Validation { get; set; } = TextValidation.None;

    public TextDialogViewModel()
    {
        SetPrimaryCanExecute(() => !HasErrors);
    }

    public override void Initialize(INavigator navigator, TextDialogRoute route)
    {
        base.Initialize(navigator, route);
        Text = route.InitialText;
        Placeholder = route.Placeholder ?? string.Empty;
        InputLabel = route.InputLabel;
        PasswordChar = route.IsPassword ? '●' : null;
        Validation = route.Validation;
        StateChanged();
    }

    public static ValidationResult? ValidateText(string value, ValidationContext context)
    {
        var instance = (TextDialogViewModel)context.ObjectInstance;
        var validation = instance.Validation;
        if (validation.Required && string.IsNullOrWhiteSpace(value))
            return new ValidationResult("The field cannot be empty.");
        if (validation.MinLength.HasValue && value.Length < validation.MinLength.Value)
            return new ValidationResult($"The minimum length is {validation.MinLength.Value} characters.");
        if (validation.MaxLength.HasValue && value.Length > validation.MaxLength.Value)
            return new ValidationResult($"The maximum length is {validation.MaxLength.Value} characters.");
        if (validation.Pattern != null && !Regex.IsMatch(value, validation.Pattern))
            return new ValidationResult(validation.CustomErrorMessage ?? "The format is invalid.");
        return ValidationResult.Success;
    }

    partial void OnTextChanged(string value)
    {
        Result = value;
    }
}