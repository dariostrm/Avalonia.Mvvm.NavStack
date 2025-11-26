namespace Mvvm.NestedNav;

public class ViewModelResolver
{
    private static IViewModelResolver? _instance;
    
    public static IViewModelResolver Instance
    {
        get => _instance ?? throw new InvalidOperationException("A ViewModel Factory has not been registered.");
        private set => _instance = value;
    }
    
    public static void RegisterSingleton(IViewModelResolver resolver)
    {
        _instance = resolver;
    }
}