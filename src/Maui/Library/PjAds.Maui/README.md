# PjAds.Maui - Cross-Platform Ads Library for .NET MAUI

A comprehensive, cross-platform advertising library for .NET MAUI applications that provides seamless integration with Google AdMob for both Android and iOS platforms.

## Features

- 🎯 **Cross-Platform Support**: Works on Android and iOS with platform-specific implementations
- 📱 **Banner Ads**: Multiple banner sizes with customizable placement
- 🎬 **Interstitial Ads**: Full-screen ads with intelligent frequency control
- 🔧 **Easy Integration**: Simple MAUI control and service-based architecture
- 📊 **Event-Driven**: Comprehensive event system for ad lifecycle management
- 🎛️ **Configurable**: Flexible configuration system supporting multiple apps
- 🔄 **User Interaction Tracking**: Automatic interstitial ad frequency management
- 📝 **Comprehensive Logging**: Built-in logging support for debugging and monitoring

## Installation

### NuGet Package (Coming Soon)
```xml
<PackageReference Include="PjAds.Maui" Version="1.0.0" />
```

### Manual Installation
1. Add the PjAds.Maui project to your solution
2. Reference it from your MAUI app project

## Quick Start

### 1. Configure Your MAUI App

In your `MauiProgram.cs`:

```csharp
using PjAds.Maui.Extensions;
using PjAds.Maui.Models;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            // Configure PjAds
            .UsePjAds(new AdConfiguration
            {
                ApplicationId = "ca-app-pub-3940256099942544~3347511713", // Test App ID
                BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111", // Test Banner ID
                InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712", // Test Interstitial ID
                InterstitialAdThreshold = 3 // Show interstitial every 3 user interactions
            })
            .ConfigurePjAdsHandlers();

        return builder.Build();
    }
}
```

### 2. Add Banner Ads in XAML

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:ads="clr-namespace:PjAds.Maui.Controls;assembly=PjAds.Maui"
             x:Class="YourApp.MainPage">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <!-- Your main content -->
        <ScrollView Grid.Row="0">
            <!-- Your content here -->
        </ScrollView>
        
        <!-- Banner Ad -->
        <ads:BannerAdView Grid.Row="1"
                          AdUnitId="{Binding BannerAdUnitId}"
                          AdSize="Banner"
                          BannerType="Regular"
                          AdLoaded="OnBannerAdLoaded"
                          AdFailedToLoad="OnBannerAdFailedToLoad" />
    </Grid>
</ContentPage>
```

### 3. Use Interstitial Ads in Code-Behind

```csharp
public partial class MainPage : ContentPage
{
    private readonly IAdManager _adManager;

    public MainPage(IAdManager adManager)
    {
        InitializeComponent();
        _adManager = adManager;
    }

    private async void OnButtonClicked(object sender, EventArgs e)
    {
        // Track user interaction (will automatically show interstitial when threshold is reached)
        _adManager.TrackUserInteraction();
        
        // Or manually show interstitial ad
        var shown = await _adManager.ShowInterstitialAdAsync();
        if (shown)
        {
            // Ad was shown successfully
        }
    }

    private void OnBannerAdLoaded(object sender, AdLoadedEventArgs e)
    {
        // Banner ad loaded successfully
    }

    private void OnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        // Handle banner ad load failure
        Console.WriteLine($"Banner ad failed to load: {e.ErrorMessage}");
    }
}
```

## Configuration

### AdConfiguration Properties

```csharp
public class AdConfiguration
{
    public string ApplicationId { get; set; } = string.Empty;
    public string BannerAdUnitId { get; set; } = string.Empty;
    public string InterstitialAdUnitId { get; set; } = string.Empty;
    public int InterstitialAdThreshold { get; set; } = 5;
    public bool IsTestMode { get; set; } = false;
}
```

### Platform-Specific Configuration

#### Android (AndroidManifest.xml)
```xml
<application>
    <!-- Google AdMob App ID -->
    <meta-data
        android:name="com.google.android.gms.ads.APPLICATION_ID"
        android:value="ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy"/>
</application>

<!-- Required permissions -->
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

#### iOS (Info.plist)
```xml
<key>GADApplicationIdentifier</key>
<string>ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy</string>

<!-- App Transport Security Settings -->
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

## Banner Ad Sizes

The library supports all standard Google AdMob banner sizes:

- `Banner` (320x50)
- `LargeBanner` (320x100)
- `MediumRectangle` (300x250)
- `FullBanner` (468x60)
- `Leaderboard` (728x90)
- `SmartBanner` (Screen width x 32|50|90)

## Banner Types

Use different banner types to track and manage multiple banner placements:

- `Regular` - Standard banner ads
- `Quiz` - Banner ads in quiz/game contexts

## Events

### Banner Ad Events
- `AdLoaded` - Banner ad loaded successfully
- `AdFailedToLoad` - Banner ad failed to load
- `AdClicked` - User clicked on banner ad
- `AdImpression` - Banner ad impression recorded

### Interstitial Ad Events
- `AdLoaded` - Interstitial ad loaded successfully
- `AdFailedToLoad` - Interstitial ad failed to load
- `AdShowed` - Interstitial ad was displayed
- `AdClosed` - Interstitial ad was closed
- `AdClicked` - User clicked on interstitial ad
- `AdImpression` - Interstitial ad impression recorded

## Advanced Usage

### Custom Configuration Factory

```csharp
builder.UsePjAds(serviceProvider =>
{
    // Create configuration based on environment, user settings, etc.
    var isDevelopment = serviceProvider.GetService<IHostEnvironment>()?.IsDevelopment() ?? false;
    
    return new AdConfiguration
    {
        ApplicationId = isDevelopment ? TestAppId : ProductionAppId,
        BannerAdUnitId = isDevelopment ? TestBannerAdUnitId : ProductionBannerAdUnitId,
        InterstitialAdUnitId = isDevelopment ? TestInterstitialAdUnitId : ProductionInterstitialAdUnitId,
        InterstitialAdThreshold = 3,
        IsTestMode = isDevelopment
    };
});
```

### Manual Ad Management

```csharp
public class MyViewModel
{
    private readonly IAdManager _adManager;

    public MyViewModel(IAdManager adManager)
    {
        _adManager = adManager;
        
        // Subscribe to events
        _adManager.InterstitialAdLoaded += OnInterstitialAdLoaded;
        _adManager.InterstitialAdClosed += OnInterstitialAdClosed;
    }

    public async Task LoadInterstitialAd()
    {
        await _adManager.LoadInterstitialAdAsync();
    }

    public async Task ShowInterstitialAd()
    {
        if (_adManager.IsInterstitialAdLoaded)
        {
            await _adManager.ShowInterstitialAdAsync();
        }
    }

    private void OnInterstitialAdLoaded(object sender, AdLoadedEventArgs e)
    {
        // Ad is ready to show
    }

    private void OnInterstitialAdClosed(object sender, InterstitialAdClosedEventArgs e)
    {
        // Load next ad
        _ = Task.Run(LoadInterstitialAd);
    }
}
```

## Testing

### Test Ad Unit IDs

Use these Google-provided test ad unit IDs during development:

```csharp
public static class TestAdUnits
{
    // Android
    public const string AndroidAppId = "ca-app-pub-3940256099942544~3347511713";
    public const string AndroidBanner = "ca-app-pub-3940256099942544/6300978111";
    public const string AndroidInterstitial = "ca-app-pub-3940256099942544/1033173712";
    
    // iOS
    public const string iOSAppId = "ca-app-pub-3940256099942544~1458002511";
    public const string iOSBanner = "ca-app-pub-3940256099942544/2934735716";
    public const string iOSInterstitial = "ca-app-pub-3940256099942544/4411468910";
}
```

## Troubleshooting

### Common Issues

1. **Ads not loading**
   - Verify internet connection
   - Check ad unit IDs are correct
   - Ensure proper platform configuration (AndroidManifest.xml / Info.plist)
   - Check device logs for detailed error messages

2. **iOS ads not showing**
   - Verify Info.plist contains GADApplicationIdentifier
   - Check App Transport Security settings
   - Ensure proper view controller hierarchy

3. **Android ads not showing**
   - Verify AndroidManifest.xml contains APPLICATION_ID meta-data
   - Check required permissions are granted
   - Ensure Google Play Services are available

### Logging

Enable detailed logging to troubleshoot issues:

```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});
```

## Requirements

- .NET 8.0 or later
- MAUI 8.0 or later
- Android API 21+ (Android 5.0)
- iOS 12.0+

## Dependencies

### Android
- Xamarin.GooglePlayServices.Ads (121.2.0+)

### iOS  
- Xamarin.Google.iOS.MobileAds (10.14.0+)

## License

This library is provided as-is for educational and development purposes. Please ensure compliance with Google AdMob policies and terms of service when using in production applications.

## Support

For issues and questions, please refer to the project documentation or create an issue in the project repository.