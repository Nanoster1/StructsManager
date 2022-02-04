using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using StructsManager.Models;
using StructsManager.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Interactivity;
using Material.Dialog;

namespace StructsManager.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private TextEditor _textEditor;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _textEditor = this.FindControl<TextEditor>("Editor");

            this.WhenActivated(disposables =>
            {
                LoadHighlighting(Environment.CurrentDirectory + "\\init.xml");

                ViewModel.ConsoleTextChanged
                    .RegisterHandler(HandleConsoleTextChanged)
                    .DisposeWith(disposables);

                ViewModel.LoadAsInteraction
                    .RegisterHandler(ShowFileDialog)
                    .DisposeWith(disposables);

                ViewModel.GetException
                    .RegisterHandler(HandleVariableError)
                    .DisposeWith(disposables);

                ViewModel.SetVariableName
                    .RegisterHandler(ShowNewVariableWindow)
                    .DisposeWith(disposables);
            });
        }

        private void HandleConsoleTextChanged(InteractionContext<string, Unit> context)
        {
            _textEditor.Text = context.Input;
            context.SetOutput(Unit.Default);
        }
        
        private async Task ShowFileDialog(InteractionContext<Unit, string> context)
        {
            var window = new OpenFileDialog();
            var result = await window.ShowAsync(this) ?? Array.Empty<string>();
            var path = result.FirstOrDefault();
            context.SetOutput(path ?? string.Empty);
        }
        
        private async Task HandleVariableError(InteractionContext<Exception, Unit> context)
        {
            var window = MessageBoxManager.GetMessageBoxStandardWindow("Error",
                $"Unable to create a variable:\n{context.Input}",
                ButtonEnum.Ok,
                MessageBox.Avalonia.Enums.Icon.Error);
            await window.ShowDialog(this);
            context.SetOutput(Unit.Default);
        }
        
        private async Task ShowNewVariableWindow(InteractionContext<string, string> context)
        {
            var window = MessageBoxManager.GetMessageBoxInputWindow(new MessageBoxInputParams()
            {
                ContentHeader = "Adding a variable",
                ContentMessage = "Name:",
                ShowInCenter = true,
                Icon = MessageBox.Avalonia.Enums.Icon.Plus,
                InputDefaultValue = context.Input,
                CanResize = false
            });
            var result = await window.ShowDialog(this);
            context.SetOutput(result.Button == "Confirm" ? result.Message : string.Empty);
        }
        
        private void ViewModel_ConsoleTextChanged(string text)
        {
            _textEditor.Text = text;
        }

        private void TextChanged(object? sender, EventArgs e)
        {
            ViewModel!.ConsoleText = _textEditor.Text;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void LoadHighlighting(string path)
        {
            if (File.Exists(path)) 
            {
                await using var fileStream = File.OpenRead(path);
                using var xmlReader = new XmlTextReader(fileStream);
                try { _textEditor.SyntaxHighlighting = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance); } catch { }
                return;
            }
            var window = MessageBoxManager.GetMessageBoxStandardWindow(
                "Error", 
                $"Highlight file not found\nDo you want to select a new file?",
                ButtonEnum.YesNo,
                MessageBox.Avalonia.Enums.Icon.Error);
            
            var answer = await window.ShowDialog(this);
            if (answer != ButtonResult.Yes) return;
            var fileDialog = new OpenFileDialog();
            var result = await fileDialog.ShowAsync(this) ?? Array.Empty<string>();
            path = result.FirstOrDefault() ?? string.Empty;
            LoadHighlighting(path);
        }

        private void DeleteContext_Click(object? sender, RoutedEventArgs e)
        {
            ViewModel?.DeleteElement();
        }
    }
}
