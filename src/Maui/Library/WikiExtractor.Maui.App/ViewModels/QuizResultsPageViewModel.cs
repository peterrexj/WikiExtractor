using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Charts;
using WikiExtractor.Maui.App.ViewModels.Charts;
using WikiExtractor.ViewModels;
using Microsoft.Maui.Controls;

namespace WikiExtractor.Maui.App.ViewModels;

public class QuizResultsPageViewModel : BaseViewModel, IQueryAttributable
{
    private ObservableCollection<DataModel> _chartPassFailData;
    public ObservableCollection<DataModel> ChartPassFailData
    {
        get => _chartPassFailData;
        set
        {
            _chartPassFailData = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Brush> _customChartColors;
    public ObservableCollection<Brush> CustomChartColors
    {
        get => _customChartColors;
        set
        {
            _customChartColors = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<QuizPageQuestionViewModel> _questions;
    public ObservableCollection<QuizPageQuestionViewModel> Questions
    {
        get => _questions;
        set
        {
            _questions = value;
            OnPropertyChanged();
        }
    }

    public ICommand CloseQuizCommand { get; set; }

    public QuizResultsPageViewModel()
    {
        CloseQuizCommand = new Command(async () => await CloseQuizAsync());

        // Initialize default colors (will be overridden when data is passed)
        CustomChartColors = new ObservableCollection<Brush>
        {
            new SolidColorBrush(Color.FromArgb("#4CAF50")), // Green for correct
            new SolidColorBrush(Color.FromArgb("#F44336")), // Red for wrong
            new SolidColorBrush(Color.FromArgb("#FF9800"))  // Orange for not answered
        };
    }

    public void Initialize(ObservableCollection<QuizPageQuestionViewModel> questions,
                          ObservableCollection<DataModel> chartData,
                          ObservableCollection<Brush> chartColors)
    {
        Questions = questions;
        ChartPassFailData = chartData;
        CustomChartColors = chartColors;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            if (query.ContainsKey("Questions") && query["Questions"] is ObservableCollection<QuizPageQuestionViewModel> questions)
            {
                Questions = questions;
            }

            if (query.ContainsKey("ChartData") && query["ChartData"] is ObservableCollection<DataModel> chartData)
            {
                ChartPassFailData = chartData;
            }

            if (query.ContainsKey("ChartColors") && query["ChartColors"] is ObservableCollection<Brush> chartColors)
            {
                CustomChartColors = chartColors;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying query attributes: {ex.Message}");
        }
    }

    private async Task CloseQuizAsync()
    {
        try
        {
            // Navigate back to the list page (go back twice - from results to quiz to list)
            await Shell.Current.GoToAsync("../..");
        }
        catch (Exception ex)
        {
            // Handle navigation error
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            // Fallback - just go back once
            await Shell.Current.GoToAsync("..");
        }
    }
}