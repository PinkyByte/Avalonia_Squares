using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Squares.Model;
using Squares.Persistence;
using Squares_Avalonia.ViewModels;
using Squares_Avalonia.Views;
using System;
using System.IO;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia.Platform;

namespace Squares_Avalonia;

public partial class App : Application
{
    #region Fields

    private SquaresModel _squaresModel = null!;
    private SquaresViewModel _squaresViewModel = null!;

    #endregion

    #region Properties

    private TopLevel? TopLevel
    {
        get
        {
            return ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime desktop => TopLevel.GetTopLevel(desktop.MainWindow),
                ISingleViewApplicationLifetime singleViewPlatform => TopLevel.GetTopLevel(singleViewPlatform.MainView),
                _ => null
            };
        }
    }

    #endregion

    #region Application methods

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        _squaresModel = new SquaresModel(new SquaresDataAccess());
        _squaresModel.GameOver += new EventHandler<GameEventArgs>(Model_GameOver);
        _squaresModel.NewGame();
        
        _squaresViewModel = new SquaresViewModel(_squaresModel);
        _squaresViewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
        _squaresViewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _squaresViewModel
            };
            desktop.Startup += async (s, e) =>
            {
                try
                {
                    await _squaresModel.LoadGame(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SquaresSuspendedGame"));
                }
                catch { }
            };

            desktop.Exit += async (s, e) =>
            {
                try
                {
                    await _squaresModel.SaveGame(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SquaresSuspendedGame"));
                }
                catch { }
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = _squaresViewModel
            };

            if (Application.Current?.TryGetFeature<IActivatableLifetime>() is { } activatableLifetime)
            {
                activatableLifetime.Activated += async (sender, args) =>
                {
                    if (args.Kind == ActivationKind.Background)
                    {
                        try
                        {
                            await _squaresModel.LoadGame(
                                Path.Combine(AppContext.BaseDirectory, "SuspendedGame"));
                        }
                        catch
                        {
                        }
                    }
                };
                activatableLifetime.Deactivated += async (sender, args) =>
                {
                    if (args.Kind == ActivationKind.Background)
                    {
                        try
                        {
                            await _squaresModel.SaveGame(
                                Path.Combine(AppContext.BaseDirectory, "SuspendedGame"));
                        }
                        catch
                        {
                        }
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion

    #region ViewModel event handlers

    private async void ViewModel_LoadGame(object? sender, EventArgs e)
    {
        if (TopLevel == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Négyzetek", "A fájlkezelés nem támogatott!", ButtonEnum.Ok, Icon.Error).ShowAsync();
            return;
        }

        try
        {
            var files = await TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Tábla betöltése",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Tábla")
                    {
                        Patterns = new[] { "*.txt" }
                    }
                }
            });

            if (files.Count > 0)
            {
                using (var stream = await files[0].OpenReadAsync())
                {
                    await _squaresModel.LoadGame(stream);
                }
            }
        }
        catch (SquaresDataException)
        {
            await MessageBoxManager.GetMessageBoxStandard("Négyzetek", "A fájl betöltése sikertelen!", ButtonEnum.Ok, Icon.Error).ShowAsync();
        }
        _squaresViewModel.ClearTable();
        _squaresViewModel.CreateTable();
    }
    private async void ViewModel_SaveGame(object? sender, EventArgs e)
    {
        if (TopLevel == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Négyzetek", "A fájlkezelés nem támogatott!", ButtonEnum.Ok, Icon.Error).ShowAsync();
            return;
        }

        try
        {
            var file = await TopLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Tábla mentése",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Tábla")
                    {
                        Patterns = new[] { "*.txt" }
                    }
                }
            });

            if (file != null)
            {
                using (var stream = await file.OpenWriteAsync())
                {
                    await _squaresModel.SaveGame(stream);
                }
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Négyzetek", "A fájl mentése sikertelen!" + ex.Message, ButtonEnum.Ok, Icon.Error).ShowAsync();
        }
    }

    #endregion

    #region View event handlers

    private async void Model_GameOver(object? sender, GameEventArgs e)
    {
        await MessageBoxManager.GetMessageBoxStandard("Négyzetek", $"A(z) {e.Winner.Color} játékos győzött {e.Winner.Score} ponttal.", ButtonEnum.Ok, Icon.Info).ShowAsync();
    }

    #endregion
}
