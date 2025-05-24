using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml; // ← ważne!
using laboratorium10.Models;
using laboratorium10.Services;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Text.Json;
using System.IO;
using System.Text;

namespace laboratorium10.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this); 
        DataContext = this;
    }

    public ObservableCollection<FastaSequence> Sequences { get; set; } = new ObservableCollection<FastaSequence>();

    private FastaSequence _selectedSequence;
    public FastaSequence SelectedSequence
    {
        get => _selectedSequence;
        set
        {
            _selectedSequence = value;
            DataContext = this;
            UpdateChart();
        }
    }

    public ISeries[] ChartSeries { get; set; }

    private async void LoadFastaFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "FASTA files", Extensions = { "fasta", "fa" } });
        dialog.AllowMultiple = false;

        var result = await dialog.ShowAsync(this);
        if (result != null && result.Length > 0)
        {
            var sequences = FastaParser.ParseFastaFile(result[0]);
            Sequences.Clear();
            foreach (var seq in sequences)
                Sequences.Add(seq);

            SelectedSequence = Sequences.FirstOrDefault();
        }
    }
    private async void ExportToJson_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedSequence == null) return;

        var saveDialog = new SaveFileDialog
        {
            Filters = { new FileDialogFilter { Name = "JSON Files", Extensions = { "json" } } },
            DefaultExtension = "json"
        };

        var path = await saveDialog.ShowAsync(this);
        if (path == null) return;

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(SelectedSequence, jsonOptions);
        File.WriteAllText(path, json);
    }

    private async void ExportToCsv_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedSequence == null) return;

        var saveDialog = new SaveFileDialog
        {
            Filters = { new FileDialogFilter { Name = "CSV Files", Extensions = { "csv" } } },
            DefaultExtension = "csv"
        };

        var path = await saveDialog.ShowAsync(this);
        if (path == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("ID,Description,Length,GCContent,CodonCount");

        sb.AppendLine($"{SelectedSequence.Id}," +
                      $"{SelectedSequence.Description}," +
                      $"{SelectedSequence.Length}," +
                      $"{SelectedSequence.GCContent:F2}," +
                      $"{SelectedSequence.CodonCount}");

        File.WriteAllText(path, sb.ToString());
    }

    private void UpdateChart()
    {
        if (SelectedSequence != null)
        {
            var baseCounts = SelectedSequence.BaseCounts;
            ChartSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new int[]
                    {
                        baseCounts.ContainsKey('A') ? baseCounts['A'] : 0,
                        baseCounts.ContainsKey('T') ? baseCounts['T'] : 0,
                        baseCounts.ContainsKey('G') ? baseCounts['G'] : 0,
                        baseCounts.ContainsKey('C') ? baseCounts['C'] : 0
                    },
                    Name = "Liczba zasad"
                }
            };

            this.DataContext = null;
            this.DataContext = this;
        }
    }
}
