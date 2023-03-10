using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Text.RegularExpressions;
using Windows.System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Reflection;
using System.Drawing;
using System.IO;
using Windows.ApplicationModel.Search;
using Windows.Storage.AccessCache;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();

            UpdateColors();
            settingsCreditsTextBlock.Text = "Credits\nAlp Deniz Senyurt - ACCTR5\nVersion: " + localSettings.Values["AppVersion"];

            // register events
            settingsIDEditBox.TextChanging += SettingsIDEditBox_TextChanging;
            settingsIDEditBox.Paste += SettingsIDEditBox_Paste;
            getHoppieLOGONCodeButton.Click += GetHoppieLOGONCodeButton_Click;
            settingsPageSaveButton.Click += SettingsPageSaveButton_Click;
            settingsESPathSelectButton.Click += SettingsESPathSelectButton_Click;
            settingsAFVPathSelectButton.Click += SettingsAFVPathSelectButton_Click;
            settingsVATISPathSelectButton.Click += SettingsVATISPathSelectButton_Click;
        }

        private async void SettingsVATISPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                settingsVATISPathTextBox.Text = folder.Name;
                localSettings.Values["vATISRequiredVersion"] = folder.Name.ToString();
            }
            else
            {
                settingsVATISPathTextBox.Text = "vATIS not found. Please select the installation directory from the right button.";
            }
        }

        private async void SettingsAFVPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };

            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                settingsAFVPathTextBox.Text = folder.Name;
                localSettings.Values["AFVRequiredVersion"] = folder.Name.ToString();
            }
            else
            {
                settingsAFVPathTextBox.Text = "Audio For VATSIM not found. Please select the installation directory from the right button.";
            }
        }

        private async void SettingsESPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                settingsESPathTextBox.Text = folder.Name;
                localSettings.Values["EuroScopeInstalledVersion"] = folder.Name.ToString();


                // add here check for sector files
            }
            else
            {
                settingsESPathTextBox.Text = "EuroScope not found. Please select the installation directory from the right button.";
            }
            */

            var folderPicker = new FolderPicker();

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, MainWindow._hWnd);

            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder == null)
            {
                settingsESPathTextBox.Text = "Please select a path";
            }
            else
            {
                if (folder.Path.Contains("Euroscope"))
                {
                    settingsESPathTextBox.Text = $"{folder?.Path + "\\EuroScope.exe"}";
                    settingsESPathTextBox.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    settingsESPathTextBox.Text = "Please select the correct path";
                }
            }

        }

        // Update colors to reflect that the path is not found.
        // When App wide settings or properties are implemented, fix this to check if PATH for the applications exists instead of string contains...
        private void UpdateColors()
        {
            if (settingsESPathTextBox.Text.Contains("not found"))
            {
                settingsESPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (settingsAFVPathTextBox.Text.Contains("not found"))
            {
                settingsAFVPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            if (settingsVATISPathTextBox.Text.Contains("not found"))
            {
                settingsVATISPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
        }


        /*
         *  Event Handling Section
         */
        // event to handle inputs and not allow any digits
        private void SettingsIDEditBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var currentPosition = settingsIDEditBox.SelectionStart - 1;
            var text = ((TextBox)sender).Text;

            // Using good old regex to filter out the text entered
            var regex = new Regex("^[0-9]*$");

            if (!regex.IsMatch(text))
            {
                // find the index of the char ([^0-9])
                var foundChar = Regex.Match(settingsIDEditBox.Text, @"[^0-9]");
                if (foundChar.Success)
                {
                    settingsIDEditBox.Text = settingsIDEditBox.Text.Remove(foundChar.Index, 1);
                }

                settingsIDEditBox.Select(currentPosition, 0);
            }
        }

        // save button click event handler
        private async void SettingsPageSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                getHoppieLOGONCodeButton.IsEnabled = false;

                List<Task> tasks = new();

                tasks.Add(Task.Delay(2000));
                await Task.WhenAll(tasks);
                // The main thread will be back here.
            }
            finally
            {
                // Enable the button even if an exception is thrown.
                getHoppieLOGONCodeButton.IsEnabled = true;
            }
        }

        // Disable button for 3 seconds to avoid over pressing
        private async void GetHoppieLOGONCodeButton_Click(object sender, RoutedEventArgs e)
        {
            App.log.Info("getHoppieLogonCode button pressed");
            try
            {
                // You can update the UI because
                // the Click event will use the main thread.
                getHoppieLOGONCodeButton.IsEnabled = false;

                List<Task> tasks = new()
                {
                    // The main thread will be released here until
                    // LaunchUriAsync returns.
                    WindowsRuntimeSystemExtensions.AsTask(Launcher.LaunchUriAsync(new Uri("https://www.hoppie.nl/acars/system/register.html"))),
                    Task.Delay(3000)
                };
                await Task.WhenAll(tasks);
                // The main thread will be back here.
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when trying to open Hoppie ACARS system register page.",
                    CloseButtonText = "OK",
                };

                _ = await dialog.ShowAsync();

                App.log.Error("Error when trying to open Hoppie register page.", ex);
            }
            finally
            {
                // Enable the button even if an exception is thrown.
                getHoppieLOGONCodeButton.IsEnabled = true;
            }
        }

        // No need to allow paste as it could break input mask and formatting
        private void SettingsIDEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            ((TextBox)sender).Undo();
        }

        internal ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    }
}
