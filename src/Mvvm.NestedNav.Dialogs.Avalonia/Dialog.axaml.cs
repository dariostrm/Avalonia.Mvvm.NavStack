using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;

namespace Mvvm.NestedNav.Dialogs.Avalonia;

public class Dialog : TemplatedControl
{
    private TopLevel? _topLevel;
    private IInputElement? _focusedElementBeforeOpen;
    private Thickness _baseMargin;
    public Dialog()
    {
        _baseMargin = Margin;
    }

    public Dialog(Classes classes) : this()
    {
        Classes.AddRange(classes);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not IDialogViewModel) return;

        this[!PrimaryButtonTextProperty] = new Binding(nameof(IDialogViewModel.PrimaryButtonText));
        this[!SecondaryButtonTextProperty] = new Binding(nameof(IDialogViewModel.SecondaryButtonText));
        this[!CloseButtonTextProperty] = new Binding(nameof(IDialogViewModel.CloseButtonText));
        this[!IsSecondaryButtonVisibleProperty] = new Binding(nameof(IDialogViewModel.IsSecondaryButtonVisible));
        this[!IsCloseButtonVisibleProperty] = new Binding(nameof(IDialogViewModel.IsCloseButtonVisible));
        this[!TitleProperty] = new Binding(nameof(IDialogViewModel.Title));
        this[!RequestCloseCommandProperty] = new Binding(nameof(IDialogViewModel.RequestCloseCommand));
        this[!PrimaryCommandProperty] = new Binding(nameof(IDialogViewModel.PrimaryCommand));
        this[!SecondaryCommandProperty] = new Binding(nameof(IDialogViewModel.SecondaryCommand));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var content = ContentTemplate?.Build(this);
        if (content != null)
        {
            content.Result.AttachedToVisualTree += (_, _) => OnContentLoaded();
            MainContent = content.Result;
        }
    }

    /// <summary>
    /// Override this method to set focus to a specific element when the dialog is opened.
    /// </summary>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        //get currently focused element to restore focus later
        _topLevel = TopLevel.GetTopLevel(this);
        _focusedElementBeforeOpen = _topLevel?.FocusManager?.GetFocusedElement();
        
        //Listen for keyboard showing/hiding events to adjust dialog position
        if (_topLevel?.InputPane != null)
            _topLevel.InputPane.StateChanged += OnInputPaneStateChanged;
        
        Focus();
    }

    private void OnInputPaneStateChanged(object? sender, InputPaneStateEventArgs e)
    {
        Margin = e.NewState == InputPaneState.Open 
            ? new Thickness(0, 0, 0, e.EndRect.Height) 
            : _baseMargin;
    }

    protected virtual void Focus()
    {
        if (!FocusOnLoad) return;
        //set focus to the element with the "DialogFocus" tag
        //if not found, set focus to the primary button
        //if not found, set focus to first focusable element
        var focusTagElement = this.GetVisualDescendants().OfType<Control>()
            .FirstOrDefault(x => x.Tag?.ToString()?.Contains("DialogFocus") == true);
        if (focusTagElement != null)
        {
            var focused = focusTagElement.Focus();
            if (focused) return;
        }
        
        var primaryButton = this.FindControl<Button>("PrimaryButton");
        if (primaryButton != null)
        {
            var focused = primaryButton.Focus();
            if (focused) return;
        }
        
        var firstFocusable = this.GetVisualDescendants().OfType<IInputElement>()
            .FirstOrDefault(x => x.Focusable);
        firstFocusable?.Focus();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        //restore focus to previously focused element
        if (_focusedElementBeforeOpen != null)
        {
            var focused = _focusedElementBeforeOpen.Focus();
            Console.WriteLine($"Restored focus to previous element: {_focusedElementBeforeOpen.GetType().Name}, success: {focused}");
        }
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var vm = DataContext as IDialogViewModel;
        switch (e.Key)
        {
            case Key.Escape:
                _topLevel?.RaiseEvent(new RoutedEventArgs(TopLevel.BackRequestedEvent));
                break;
            case Key.Enter:
                vm?.PrimaryCommand.Execute(null);
                break;
        }
    }

    public static readonly StyledProperty<Control> MainContentProperty = AvaloniaProperty.Register<Dialog, Control>(
        nameof(MainContent));

    public Control MainContent
    {
        get => GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }
    
    public static readonly RoutedEvent<RoutedEventArgs> ContentLoadedEvent =
        RoutedEvent.Register<Dialog, RoutedEventArgs>(nameof(ContentLoaded), RoutingStrategies.Direct);

    public event EventHandler<RoutedEventArgs> ContentLoaded
    {
        add => AddHandler(ContentLoadedEvent, value);
        remove => RemoveHandler(ContentLoadedEvent, value);
    }

    protected virtual void OnContentLoaded()
    {
        var args = new RoutedEventArgs(ContentLoadedEvent);
        RaiseEvent(args);
    }

    public static readonly StyledProperty<ControlTemplate?> ContentTemplateProperty = AvaloniaProperty.Register<Dialog, ControlTemplate?>(
        nameof(ContentTemplate));

    public ControlTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Dialog, string>(
        nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<ICommand> RequestCloseCommandProperty = AvaloniaProperty.Register<Dialog, ICommand>(
        nameof(RequestCloseCommand));

    public ICommand RequestCloseCommand
    {
        get => GetValue(RequestCloseCommandProperty);
        set => SetValue(RequestCloseCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> PrimaryCommandProperty = AvaloniaProperty.Register<Dialog, ICommand>(
        nameof(PrimaryCommand));

    public ICommand PrimaryCommand
    {
        get => GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> SecondaryCommandProperty = AvaloniaProperty.Register<Dialog, ICommand>(
        nameof(SecondaryCommand));

    public ICommand SecondaryCommand
    {
        get => GetValue(SecondaryCommandProperty);
        set => SetValue(SecondaryCommandProperty, value);
    }

    public static readonly StyledProperty<string> PrimaryButtonTextProperty = AvaloniaProperty.Register<Dialog, string>(
        nameof(PrimaryButtonText));

    public string PrimaryButtonText
    {
        get => GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly StyledProperty<string> SecondaryButtonTextProperty = AvaloniaProperty.Register<Dialog, string>(
        nameof(SecondaryButtonText));

    public string SecondaryButtonText
    {
        get => GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly StyledProperty<string> CloseButtonTextProperty = AvaloniaProperty.Register<Dialog, string>(
        nameof(CloseButtonText));

    public string CloseButtonText
    {
        get => GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    public static readonly StyledProperty<bool> IsSecondaryButtonVisibleProperty = AvaloniaProperty.Register<Dialog, bool>(
        nameof(IsSecondaryButtonVisible));

    public bool IsSecondaryButtonVisible
    {
        get => GetValue(IsSecondaryButtonVisibleProperty);
        set => SetValue(IsSecondaryButtonVisibleProperty, value);
    }

    public static readonly StyledProperty<string> IsCloseButtonVisibleProperty = AvaloniaProperty.Register<Dialog, string>(
        nameof(IsCloseButtonVisible));

    public string IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    public static readonly StyledProperty<bool> FocusOnLoadProperty = AvaloniaProperty.Register<Dialog, bool>(
        nameof(FocusOnLoad), true);

    public bool FocusOnLoad
    {
        get => GetValue(FocusOnLoadProperty);
        set => SetValue(FocusOnLoadProperty, value);
    }
}