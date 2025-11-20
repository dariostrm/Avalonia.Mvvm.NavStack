namespace Mvvm.NestedNav;

public class ViewModelFactory
{
    private static IViewModelFactory? _instance;
    
    public static IViewModelFactory Instance
    {
        get => _instance ?? throw new InvalidOperationException("A ViewModel Factory has not been registered.");
        private set => _instance = value;
    }
    
    public static void RegisterSingleton(IViewModelFactory factory)
    {
        _instance = factory;
    }
}