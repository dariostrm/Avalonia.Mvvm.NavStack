using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.NestedNav.Avalonia.Samples.Routes;
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
        serviceCollection.AddSingleton<Func<Route, IViewModel>>(serviceProvider => route => route switch
        {
            HomeRoute => new HomeViewModel(),
            DetailsRoute r => new DetailsViewModel(r.Message),
            ProfileRoute => new ProfileViewModel(),
            SettingsRoute => new SettingsViewModel(),
            _ => throw new ArgumentException($"No ViewModel registered for route type {route.GetType().FullName}")
        });
        serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
        serviceCollection.AddSingleton<MainViewModel>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var vmFactory = serviceProvider.GetRequiredService<IViewModelFactory>();
        Resources["ViewModelFactory"] = vmFactory;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var mainVm = serviceProvider.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow()
            {
                DataContext = mainVm,
            };
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