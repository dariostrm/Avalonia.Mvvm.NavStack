using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using BookTracker.Domain;
using BookTracker.ViewModels;
using BookTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.NavStack;

namespace BookTracker;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IBookRepository, DummyBookRepository>();
        
        serviceCollection.AddSingleton<Func<Route, IViewModel>>(serviceProvider => route => route switch
        {
            AboutTab => new AboutViewModel(),
            BooksTab => new BooksViewModel(
                bookRepository: serviceProvider.GetRequiredService<IBookRepository>()
            ),
            SettingsTab => new SettingsViewModel(),
            BookRoute r => new BookViewModel(
                bookId: r.BookId, 
                bookRepository: serviceProvider.GetRequiredService<IBookRepository>()
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(route))
        });
        serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();
        Resources.Add("ViewModelFactory", viewModelFactory);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView();
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