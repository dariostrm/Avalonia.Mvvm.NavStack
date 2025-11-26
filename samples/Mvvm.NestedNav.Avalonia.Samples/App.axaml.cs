using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.NestedNav.Avalonia.Samples.Screens;
using Mvvm.NestedNav.Avalonia.Samples.ViewModels;
using Mvvm.NestedNav.Avalonia.Samples.Views;

namespace Mvvm.NestedNav.Avalonia.Samples;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<HomeViewModel>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var factory = new ServiceProviderViewModelResolver(serviceProvider);
        factory.Register<HomeScreen, HomeViewModel>();
        factory.Register<DetailsScreen, DetailsViewModel>();
        factory.Register<ProfileScreen, ProfileViewModel>();
        factory.Register<SettingsScreen, SettingsViewModel>();
        ViewModelResolver.RegisterSingleton(factory);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}