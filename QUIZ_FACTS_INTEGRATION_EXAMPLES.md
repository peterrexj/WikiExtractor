## Integration Example for PersonaDetailPage

### Step 1: Add control to XAML

Add the following namespace to your XAML file:
```xaml
xmlns:controls="clr-namespace:WikiExtractor.Maui.App.Controls"
```

Add the control as an overlay in your main Grid (place it AFTER all other content so it overlays):
```xaml
<ContentPage>
    <Grid>
        <!-- Your existing content here -->
        <ScrollView ...>
            <!-- Detail content -->
        </ScrollView>
        
        <!-- Add Loading Facts Control as the LAST child for proper overlay -->
        <controls:LoadingFactsControl x:Name="loadingFactsControl" 
                                      IsVisible="False"
                                      ZIndex="1000" />
    </Grid>
</ContentPage>
```

### Step 2: Update Code-Behind (PersonaDetailPage.xaml.cs)

#### Add Using Statement:
```csharp
using WikiExtractor.Maui.App.Models;
using WikiExtractor.Maui.App.Controls;
```

#### Modify OnAppearing Method:
```csharp
protected override async void OnAppearing()
{
    try
    {
        base.OnAppearing();

        // Show loading control with master-specific facts
        ShowLoadingWithFacts();

        await Task.Yield();
        await Task.Delay(100);

        await LoadWithPageBinding();
    }
    catch (Exception ex)
    {
        CaptureErrorOnPage(ex);
    }
    finally
    {
        Stopwatch.Stop();
        
        // IMPORTANT: Hide loading control when done
        loadingFactsControl?.NotifyLoadComplete();
        
        ViewHelper.RunOnAppDispatcher(() =>
        {
            DisplayAlert("Info", $"Page loaded in {Stopwatch.ElapsedMilliseconds} ms", "OK");
        });
    }
}
```

#### Add Helper Method:
```csharp
private void ShowLoadingWithFacts()
{
    try
    {
        int.TryParse(MasterId, out var masterIdInt);
        
        var model = new LoadingFactsModel
        {
            FactCount = 3, // Show 3 facts for detail page
            FactDisplayDurationMs = 4000, // 4 seconds per fact
            ShowMasterImage = true, // Show the master's image
            AutoMarkFactsAsShown = true, // Automatically track shown facts
            MasterId = masterIdInt > 0 ? masterIdInt : (int?)null, // Facts for this master only
            OnLoadComplete = () =>
            {
                // Optional callback when loading completes
                Console.WriteLine("Detail page loading completed");
            }
        };
        
        loadingFactsControl.ViewModel.Model = model;
        loadingFactsControl.IsVisible = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error showing loading facts: {ex.Message}");
        // Gracefully degrade - just don't show facts
    }
}
```

#### Update LoadSubPageItemDataDetails to work with loading indicator:
```csharp
private async Task LoadSubPageItemDataDetails()
{
    try
    {
        personaDetailViewModel.IsPageBusy = true;
        personaDetailViewModel.IsDataLoading = true;
        personaDetailViewModel.LoadingMessage = "Fetching data...";
        await Task.Yield();
        await Task.Delay(100);

        // ... existing loading code ...

        RunOnAppDispatcher(() =>
        {
            personaDetailViewModel.IsDataLoading = false;
            personaDetailViewModel.IsPageBusy = false;
        });
    }
    catch (Exception ex)
    {
        CaptureErrorOnPage(ex);
        RunOnAppDispatcher(() => personaDetailViewModel.IsDataLoading = false);
    }
}
```

---

## Integration Example for WikiListOfItemsPage

### Step 1: Add control to XAML

```xaml
<ContentPage>
    <Grid>
        <!-- Your existing ListView and other controls -->
        <ListView ... />
        <autoComplete ... />
        
        <!-- Add Loading Facts Control as overlay -->
        <controls:LoadingFactsControl x:Name="loadingFactsControl" 
                                      IsVisible="False"
                                      ZIndex="1000" />
    </Grid>
</ContentPage>
```

### Step 2: Update Code-Behind (WikiListOfItemsPage.xaml.cs)

#### Modify OnAppearing Method:
```csharp
protected override async void OnAppearing()
{
    try
    {
        if (BindingContext == null || personaListViewModel == null)
        {
            personaListViewModel = new PersonaListViewModel();
            BindingContext = personaListViewModel;

            // Show loading with general facts
            ShowLoadingWithFacts();

            await LoadInitialData();
        }
        else
        {
            // Show loading for refresh
            ShowLoadingWithFacts();
            
            await LoadRefreshData();
        }
    }
    catch (Exception ex)
    {
        ExceptionHandler.CaptureException(ex);
        if (personaListViewModel != null)
        {
            personaListViewModel.IsDataLoading = false;
        }
    }
    finally
    {
        // Hide loading control
        loadingFactsControl?.NotifyLoadComplete();
        
        if (personaListViewModel != null)
        {
            personaListViewModel.IsPageBusy = false;
        }
        autoComplete.Unfocus();
    }
    
    base.OnAppearing();
}
```

#### Add Helper Method:
```csharp
private void ShowLoadingWithFacts()
{
    try
    {
        var model = new LoadingFactsModel
        {
            FactCount = 5, // Show 5 facts for list page
            FactDisplayDurationMs = 3000, // 3 seconds per fact
            ShowMasterImage = true, // Show images
            AutoMarkFactsAsShown = true, // Track shown facts
            MasterId = null, // Show facts from any master (general facts)
            OnLoadComplete = () =>
            {
                Console.WriteLine("List page loading completed");
            }
        };
        
        loadingFactsControl.ViewModel.Model = model;
        loadingFactsControl.IsVisible = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error showing loading facts: {ex.Message}");
        // Gracefully degrade
    }
}
```

---

## Advanced Usage Examples

### Example 1: Only show facts if loading takes more than 1 second

```csharp
private CancellationTokenSource _loadingCts;

private async void ShowLoadingWithFactsDelayed()
{
    _loadingCts = new CancellationTokenSource();
    
    // Wait 1 second before showing facts
    try
    {
        await Task.Delay(1000, _loadingCts.Token);
        
        if (!_loadingCts.Token.IsCancellationRequested)
        {
            ShowLoadingWithFacts();
        }
    }
    catch (TaskCanceledException)
    {
        // Loading finished before 1 second - don't show facts
    }
}

// In your finally block:
finally
{
    _loadingCts?.Cancel();
    loadingFactsControl?.NotifyLoadComplete();
}
```

### Example 2: Different facts based on user settings

```csharp
private void ShowLoadingWithFacts()
{
    // Get user preference for fact display
    var showFacts = SettingsHelper.GetSetting("ShowLoadingFacts", true);
    
    if (!showFacts)
    {
        return; // User disabled facts
    }
    
    var factCount = SettingsHelper.GetSetting("FactCount", 5);
    var factDuration = SettingsHelper.GetSetting("FactDuration", 3000);
    
    var model = new LoadingFactsModel
    {
        FactCount = factCount,
        FactDisplayDurationMs = factDuration,
        ShowMasterImage = true,
        AutoMarkFactsAsShown = true,
        MasterId = null
    };
    
    loadingFactsControl.ViewModel.Model = model;
    loadingFactsControl.IsVisible = true;
}
```

### Example 3: Manual fact tracking (without auto-mark)

```csharp
private void ShowLoadingWithManualTracking()
{
    var model = new LoadingFactsModel
    {
        FactCount = 3,
        FactDisplayDurationMs = 5000,
        ShowMasterImage = true,
        AutoMarkFactsAsShown = false, // Disable auto-tracking
        MasterId = null,
        OnLoadComplete = () =>
        {
            // Manually mark facts as shown after user interaction
            if (UserClickedLikeButton())
            {
                MarkCurrentFactAsShown();
            }
        }
    };
    
    loadingFactsControl.ViewModel.Model = model;
    loadingFactsControl.IsVisible = true;
}

private void MarkCurrentFactAsShown()
{
    var currentFact = loadingFactsControl.ViewModel.CurrentFact;
    if (currentFact != null)
    {
        SharedServices.QuizController.MarkFactAsShown(
            currentFact.MasterId, 
            currentFact.MetadataKey);
    }
}
```

---

## Testing the Integration

### 1. Visual Testing Checklist:
- [ ] Control appears as overlay on page load
- [ ] Spinner is visible and animating
- [ ] Facts are displayed and readable
- [ ] Facts rotate every N seconds (configurable)
- [ ] Master image appears in circle (if enabled)
- [ ] Control disappears when NotifyLoadComplete() is called
- [ ] Theming works correctly (Dark/Forest themes)

### 2. Functional Testing:
- [ ] Facts are different each time (not repeating)
- [ ] After seeing all facts, they cycle again
- [ ] Master-specific facts only show for that master
- [ ] No crashes if no facts available
- [ ] Performance is acceptable (no lag)

### 3. Edge Cases:
- [ ] No internet connection (facts from local DB)
- [ ] Empty QuizDefinition table (shows default message)
- [ ] All facts already shown (cycles through all)
- [ ] Very long fact text (wraps correctly)
- [ ] Missing master images (shows placeholder)

---

## Performance Tips

1. **Pre-fetch facts**: Load facts before showing control
```csharp
// Pre-fetch to reduce delay
var facts = await Task.Run(() => 
    SharedServices.QuizController.GetQuizFacts(5, masterId));
```

2. **Dispose properly**: Always call NotifyLoadComplete() in finally block
```csharp
try { /* loading */ }
finally { loadingFactsControl?.NotifyLoadComplete(); }
```

3. **Limit fact count**: Don't fetch more facts than necessary
```csharp
// Good for quick loads
FactCount = 3

// Good for longer loads
FactCount = 8
```

4. **Adjust rotation speed**: Match to expected load time
```csharp
// Faster rotation for quick loads
FactDisplayDurationMs = 2000

// Slower rotation for detailed reading
FactDisplayDurationMs = 5000
```

---

## Troubleshooting

### Facts not showing:
1. Check if control is visible: `loadingFactsControl.IsVisible = true`
2. Verify facts are being fetched: Debug `GetQuizFacts()` method
3. Ensure ZIndex is high enough to overlay other content

### Facts repeating:
1. Verify `AutoMarkFactsAsShown = true`
2. Check QuizFactStatus table for entries
3. Clear fact status: `QuizController.ResetShownFacts()`

### Performance issues:
1. Reduce `FactCount` to 3-5 facts
2. Increase `FactDisplayDurationMs` to reduce rotation frequency
3. Use `masterId` filter for better performance

### Styling issues:
1. Verify theme colors are defined in Theme.xaml files
2. Check DynamicResource keys match
3. Test in both Dark and Forest themes
