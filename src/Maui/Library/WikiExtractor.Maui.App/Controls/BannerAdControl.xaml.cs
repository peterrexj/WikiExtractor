namespace WikiExtractor.Maui.App.Controls;

public partial class BannerAdControl : ContentView
{
    public static readonly BindableProperty AdUnitIdProperty =
        BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(BannerAdControl), string.Empty);

    public string AdUnitId
    {
        get => (string)GetValue(AdUnitIdProperty);
        set => SetValue(AdUnitIdProperty, value);
    }

    public BannerAdControl()
	{
		InitializeComponent();
    }
}