using System.Collections.ObjectModel;
using System.Windows.Input;
using WikiExtractor.Maui.App.ViewModels.Charts;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.Services;

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
            OnPropertyChanged(nameof(CorrectCountText));
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

    private ObservableCollection<QuizPageQuestionViewModel> _tempQuestions;
    private ObservableCollection<DataModel> _tempChartData;

    public ICommand CloseQuizCommand { get; set; }

    public QuizResultsPageViewModel()
    {
        CloseQuizCommand = new Command(async () => await CloseQuizAsync());
    }

    public string CorrectCountText
    {
        get
        {
            if (ChartPassFailData == null || ChartPassFailData.Count == 0)
                return "0/0";

            // Find the "Correct" category and the total sum of all values
            var correct = ChartPassFailData.FirstOrDefault(d => d.Category.StartsWith("correct", StringComparison.InvariantCultureIgnoreCase))?.Value ?? 0;
            var total = ChartPassFailData.Sum(d => d.Value);

            return $"{correct}/{total}";
        }
    }

    public async Task LoadChartDataAsync()
    {
        // 1. Wait for the transition
        await Task.Delay(1000);

        // 2. Await the actual results from your Service TCS
        var colors = await SharedServiceCore.ThemeHandler.GetChartColorsAsync();

        // 3. Update on Main Thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Assign colors first
            CustomChartColors = colors;

            // Assign data
            if (_tempQuestions != null) Questions = _tempQuestions;
            if (_tempChartData != null) ChartPassFailData = _tempChartData;

            // Force the Score text to update
            OnPropertyChanged(nameof(CorrectCountText));

            IsPageBusy = false;
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        try
        {
            // 1. Capture the data from the navigation dictionary
            if (query.TryGetValue("Questions", out var q) && q is ObservableCollection<QuizPageQuestionViewModel> questions)
            {
                // Don't assign to the Public Property yet to prevent the ListView from 
                // trying to render while the page animation is running
                _tempQuestions = questions;
            }

            if (query.TryGetValue("ChartData", out var c) && c is ObservableCollection<DataModel> chartData)
            {
                _tempChartData = chartData;
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