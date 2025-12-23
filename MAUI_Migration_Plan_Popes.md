This is a reference project which works and its MAUI which got transformed from Xamarin 
/Users/josephpe/Git/peterrexj/new/LoanCalculatorMaui/src

I have another xamarin project which needs to be transformed to MAUI. The above is only a reference on how things will look like in MAUI and that project works perfectly.
I have started to create a MAUI project /Users/josephpe/Git/peterrexj/new/WikiExtractor/src/Maui/Apps/Maui.Popes/Maui.Popes.csproj,
There are so many link files in this project as it a single app code for multiple applications only difference is the data or the db and few properties injected through DI
There is also a Library project
I want to start slowly shape the new project MAUI.Popes into a working condition
Do not add more code or new logic as majority works, its just that we need to move into MAUI using the reference project.

I can see the db can be loaded now successfully and for userstore we are going with SecureStorage, next step lets remove everything from the UI and only load the mainpage and also the flyout menus which reads from the DB


# Xamarin.Forms to MAUI Migration Plan - Popes Project

## Project Overview

**Migration Target:** Convert PopesOfChurch Xamarin.Forms project to MAUI (Android-only initially)

**Source Projects:**
- `src/WikiExtractor.XamarinForms.App/Popes/PopesOfChurch.Android/PopesOfChurch.Android.csproj` - Xamarin.Android project
- `src/WikiExtractor.XamarinForms/WikiExtractor.XamarinForms.csproj` - Shared Xamarin.Forms library

**Target Project:**
- `src/Maui/Apps/Maui.Popes/Maui.Popes.csproj` - MAUI project (currently basic template)

**Key Requirements:**
- Target Android platform only initially
- Remove MarcTron.Admob (Ads library)
- Remove MagicGradients package
- Maintain Syncfusion controls support
- Preserve core app functionality

---

## Phase 1: Project Structure Analysis & Setup

### 1.1 Analyze current Xamarin.Forms dependencies and identify MAUI equivalents

**Current Dependencies Analysis:**

**Xamarin.Forms Dependencies (to be replaced):**
- `Xamarin.Forms` v5.0.0.2612 → `Microsoft.Maui.Controls`
- `Xamarin.Essentials` v1.8.0 → `Microsoft.Maui.Essentials`

**Syncfusion Packages (to be updated):**
- `Syncfusion.Xamarin.Buttons` v23.1.39 → `Syncfusion.Maui.Buttons`
- `Syncfusion.Xamarin.Core` v23.1.39 → `Syncfusion.Maui.Core`
- `Syncfusion.Xamarin.SfAutoComplete` v23.1.39 → `Syncfusion.Maui.Inputs`
- `Syncfusion.Xamarin.SfBusyIndicator` v23.1.39 → `Syncfusion.Maui.Core`
- `Syncfusion.Xamarin.SfCarousel` v23.1.39 → `Syncfusion.Maui.Carousel`
- `Syncfusion.Xamarin.SfChart` v23.1.39 → `Syncfusion.Maui.Charts`
- `Syncfusion.Xamarin.SfListView` v23.1.39 → `Syncfusion.Maui.ListView`
- `Syncfusion.Xamarin.SfPopupLayout` v23.1.39 → `Syncfusion.Maui.Popup`
- `Syncfusion.Xamarin.SfProgressBar` v23.1.39 → `Syncfusion.Maui.ProgressBar`
- `Syncfusion.Xamarin.SfTabView` v23.1.39 → `Syncfusion.Maui.TabView`

**Packages to Remove:**
- `MarcTron.Admob` v1.9.0.6 - Ads library (as requested)
- `MagicGradients` v1.3.1 - Magic gradient library (as requested)

**Custom Libraries (need compatibility check):**
- These libraries are not directly added as project reference and has been updated with the code to support
- `Pj.Library` v1.0.4.31
- `Pj.Library.Datastore.Sqlite` v1.0.4.31
- `Pj.Library.Mobile.Sqlite` v1.0.4.31

**Other Dependencies:**
- `Microsoft.AppCenter.Analytics` v5.0.3 → Keep (MAUI compatible)
- `Microsoft.AppCenter.Crashes` v5.0.3 → Keep (MAUI compatible)

### 1.2 Create MAUI-compatible project structure for Popes app

**Target Structure:**
```
src/Maui/Apps/Maui.Popes/
├── Platforms/
│   └── Android/
│       ├── MainActivity.cs
│       ├── MainApplication.cs
│       ├── AndroidManifest.xml
│       └── Resources/
├── Resources/
│   ├── AppIcon/
│   ├── Splash/
│   ├── Images/
│   ├── Fonts/
│   └── Raw/
├── Views/
├── ViewModels/
├── Services/
├── Models/
└── Controls/
```

### 1.3 Set up Android-only target framework configuration

**Update Maui.Popes.csproj:**
```xml
<TargetFrameworks>net9.0-android</TargetFrameworks>
<SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
```

### 1.4 Configure project references and dependencies

**Add project references:**
- Reference to `WikiExtractor.Maui.App` library
- Reference to `WikiExtractor.Maui.Core` library

---

## Phase 2: Core Library Migration

### 2.1 Migrate WikiExtractor.XamarinForms shared library to WikiExtractor.Maui.App

**Files to migrate from `src/WikiExtractor.XamarinForms/`:**

**Core Application Files:**
- `App.xaml` and `App.xaml.cs`
- `AppShell.xaml` and `AppShell.xaml.cs`
- `AssemblyInfo.cs`

**Controls:**
- `Controls/Header2ListItemTemplate.xaml/.cs`
- `Controls/Header3ListItemTemplate.xaml/.cs`
- `Controls/ImageListItemTemplate.xaml/.cs`
- `Controls/ItemDetailListTemplateSelector.cs`
- `Controls/ParagraphContentListItemTemplate.xaml/.cs`

**Views:**
- `Views/PersonaDetailPage.xaml/.cs`
- `Views/QuizPage.xaml/.cs`
- `Views/WikiListOfItemsPage.xaml/.cs`

**ViewModels:**
- `ViewModels/ItemDetailListViewModel.cs`
- `ViewModels/PersonaDetailViewModel.cs`
- `ViewModels/PersonaListViewModel.cs`
- `ViewModels/QuizPageQuestionViewModel.cs`
- `ViewModels/QuizPageViewModel.cs`
- `ViewModels/Charts/DataModel.cs`

**Services:**
- `Services/IAppEnvironment.cs`
- `Services/IAppInformation.cs`
- `Services/IAppMenuItem.cs`
- `Services/IImageService.cs`
- `Services/ILocalStorage.cs`
- `Services/SharedServices.cs`

**Repository:**
- `Repository/AppDatabase.cs`
- `Repository/DatabaseService.cs`
- `Repository/UserStore/` (all files)

**Extensions:**
- `Exts/` (all files except AdsHelper.cs - to be removed)

**Models:**
- `Models/` (all subdirectories and files)

**Converters:**
- `Converters/StringToColorConverter.cs`

**Fonts:**
- `Fonts/` (all font files and classes)

### 2.2 Convert Xamarin.Forms controls to MAUI equivalents

**Namespace Updates Required:**
- `using Xamarin.Forms;` → `using Microsoft.Maui.Controls;`
- `using Xamarin.Forms.Xaml;` → `using Microsoft.Maui.Controls.Xaml;`
- `using Xamarin.Essentials;` → `using Microsoft.Maui.Essentials;`

**Control Mapping:**
- `ContentPage` → `ContentPage` (same)
- `Shell` → `Shell` (same)
- `ListView` → `CollectionView` (recommended) or `ListView`
- `Entry` → `Entry` (same)
- `Button` → `Button` (same)
- `Label` → `Label` (same)
- `Image` → `Image` (same)

### 2.3 Update namespace references from Xamarin.Forms to Microsoft.Maui

**Files requiring namespace updates:**
- All `.xaml` files: Update xmlns declarations
- All `.cs` files: Update using statements
- Custom controls and converters
- ViewModels and services

### 2.4 Migrate custom controls and templates

**Custom Controls to Update:**
- `ExtendedImage.cs` - Update for MAUI compatibility
- `ExtendedListView.cs` - Consider migration to CollectionView
- Template selectors and data templates

### 2.5 Update font and resource handling for MAUI

**Font Migration:**
- Move fonts to `Resources/Fonts/` in MAUI project
- Update font registration in `MauiProgram.cs`
- Update font references in XAML and code

**Resource Updates:**
- Embedded resources handling
- Asset management updates

---

## Phase 3: Package Dependencies Migration

### 3.1 Replace Xamarin.Forms package with Microsoft.Maui.Controls

**Remove:**
```xml
<PackageReference Include="Xamarin.Forms" Version="5.0.0.2612" />
```

**Add:**
```xml
<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="$(MauiVersion)" />
```

### 3.2 Update Syncfusion packages from Xamarin to MAUI versions

**Replace Syncfusion packages:**
```xml
<!-- Remove Xamarin versions -->
<PackageReference Include="Syncfusion.Xamarin.Buttons" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.Core" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfAutoComplete" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfBusyIndicator" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfCarousel" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfChart" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfListView" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfPopupLayout" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfProgressBar" Version="23.1.39" />
<PackageReference Include="Syncfusion.Xamarin.SfTabView" Version="23.1.39" />

<!-- Add MAUI versions -->
<PackageReference Include="Syncfusion.Maui.Buttons" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.Core" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.Inputs" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.Carousel" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.Charts" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.ListView" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.Popup" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.ProgressBar" Version="27.1.48" />
<PackageReference Include="Syncfusion.Maui.TabView" Version="27.1.48" />
```

### 3.3 Remove MarcTron.Admob (Ads library) as requested

**Remove package reference:**
```xml
<PackageReference Include="MarcTron.Admob" Version="1.9.0.6" />
```

**Remove related code:**
- `Exts/AdsHelper.cs`
- Any ad-related UI elements
- Ad initialization code

### 3.4 Remove MagicGradients package as requested

**Remove package reference:**
```xml
<PackageReference Include="MagicGradients" Version="1.3.1" />
```

**Replace gradient usage:**
- Convert to MAUI native gradients
- Update XAML gradient declarations

### 3.5 Update Pj.Library packages to MAUI-compatible versions

**Verify compatibility:**
- Check if Pj.Library packages support .NET 9.0
- Update to latest versions if available
- Consider alternatives if not compatible

### 3.6 Replace Xamarin.Essentials with Microsoft.Maui.Essentials

**Remove:**
```xml
<PackageReference Include="Xamarin.Essentials" Version="1.8.0" />
```

**Add:**
```xml
<PackageReference Include="Microsoft.Maui.Essentials" Version="$(MauiVersion)" />
```

---

## Phase 4: Platform-Specific Code Migration

### 4.1 Migrate Android MainActivity from Xamarin.Forms to MAUI

**Current MainActivity location:**
`Resources/LinkFiles/Android/MainActivity.cs`

**Target location:**
`Platforms/Android/MainActivity.cs`

**Migration steps:**
- Update base class from `FormsAppCompatActivity` to `MauiAppCompatActivity`
- Update initialization calls
- Migrate platform-specific configurations

### 4.2 Convert Android dependency injection implementations

**Files to migrate:**
- `DependencyInjection/AppInformation.cs`
- `Resources/LinkFiles/Android/AppEnvironment.cs`
- `Resources/LinkFiles/Android/ConfigHelper.cs`
- `Resources/LinkFiles/Android/ImageService.cs`
- `Resources/LinkFiles/Android/LocalStorage.cs`
- `Resources/LinkFiles/Android/SqliteFileHelper.cs`

**Migration to MAUI DI:**
- Register services in `MauiProgram.cs`
- Update service implementations for MAUI

### 4.3 Update Android manifest and resource configurations

**AndroidManifest.xml updates:**
- Update package references
- Remove ad-related permissions
- Update target SDK version

**Resource updates:**
- Migrate drawable resources
- Update color and style resources
- Convert layout files if needed

### 4.4 Migrate platform-specific services

**Service implementations to update:**
- AppInformation service
- LocalStorage service
- SqliteFileHelper service
- ImageService implementation

### 4.5 Update Android asset and resource handling

**Assets to migrate:**
- `Assets/CommonConfigs.json`
- `Assets/WikiStorePopes.db`
- Drawable resources
- Mipmap resources

---

## Phase 5: UI and Navigation Migration

### 5.1 Convert App.xaml and App.xaml.cs to MAUI structure

**App.xaml updates:**
- Update xmlns declarations
- Update resource dictionaries
- Remove Xamarin.Forms specific configurations

**App.xaml.cs updates:**
- Update initialization code
- Migrate to MAUI app lifecycle

### 5.2 Migrate AppShell navigation to MAUI Shell

**AppShell.xaml updates:**
- Update Shell structure for MAUI
- Update navigation routes
- Update tab and flyout configurations

### 5.3 Update page navigation and routing

**Navigation updates:**
- Update Shell routing
- Update page navigation calls
- Update parameter passing

### 5.4 Convert Views

**PersonaDetailPage.xaml/.cs:**
- Update namespace references
- Update control usage
- Test Syncfusion control compatibility

**QuizPage.xaml/.cs:**
- Remove ad-related UI elements
- Update control bindings
- Test functionality without ads

**WikiListOfItemsPage.xaml/.cs:**
- Update ListView to CollectionView if needed
- Update data templates
- Test list functionality

### 5.5 Update ViewModels for MAUI compatibility

**ViewModel updates:**
- Update base classes if needed
- Update navigation service calls
- Update platform-specific service calls

---

## Phase 6: Data and Services Migration

### 6.1 Migrate database services and repositories

**Repository files to update:**
- `Repository/AppDatabase.cs`
- `Repository/DatabaseService.cs`
- All repository classes in linked files

**Updates required:**
- Update SQLite connection handling
- Update file path resolution
- Test database connectivity

### 6.2 Update SQLite integration for MAUI

**SQLite updates:**
- Verify Pj.Library.Datastore.Sqlite compatibility
- Update connection string handling
- Update file access patterns

### 6.3 Convert dependency injection to MAUI DI container

**MauiProgram.cs configuration:**
```csharp
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
            });

        // Register services
        builder.Services.AddSingleton<IAppInformation, AppInformation>();
        builder.Services.AddSingleton<ILocalStorage, LocalStorage>();
        // ... other services

        return builder.Build();
    }
}
```

### 6.4 Migrate shared services and interfaces

**Service interfaces to update:**
- `IAppEnvironment`
- `IAppInformation`
- `IAppMenuItem`
- `IImageService`
- `ILocalStorage`

### 6.5 Update configuration and settings management

**Configuration updates:**
- Update settings storage
- Update configuration file handling
- Test settings persistence

---

## Phase 7: Assets and Resources Migration

### 7.1 Convert Android resources to MAUI resource structure

**Resource migration:**
- Move images to `Resources/Images/`
- Move fonts to `Resources/Fonts/`
- Update resource references

### 7.2 Migrate app icons and splash screens

**Icon migration:**
- Convert mipmap resources to MAUI format
- Update app icon configuration
- Create splash screen resources

### 7.3 Update font handling and embedded resources

**Font updates:**
- Register fonts in MauiProgram.cs
- Update font references in XAML
- Test font rendering

### 7.4 Convert drawable resources to MAUI format

**Drawable migration:**
- Convert splash screen drawables
- Update image resources
- Test resource loading

### 7.5 Update color and style resources

**Style migration:**
- Convert colors.xml to MAUI format
- Update style resources
- Test theme application

---

## Phase 8: Build Configuration & Testing

### 8.1 Configure Android-specific build settings

**Build configuration:**
- Update target framework
- Configure signing (remove hardcoded paths)
- Update package format settings

### 8.2 Update signing and packaging configuration

**Signing updates:**
- Remove hardcoded keystore paths
- Configure for development/release
- Update package format to AAB

### 8.3 Test basic app functionality

**Basic tests:**
- App launches successfully
- Navigation works
- Basic UI renders correctly

### 8.4 Verify database connectivity and data loading

**Database tests:**
- Database opens successfully
- Data loads correctly
- Repository operations work

### 8.5 Test navigation and UI rendering

**UI tests:**
- All pages load correctly
- Navigation between pages works
- Controls render properly

---

## Phase 9: Feature Validation & Cleanup

### 9.1 Validate Syncfusion controls functionality

**Syncfusion tests:**
- Charts render correctly
- ListView/CollectionView works
- Popup and progress controls function
- AutoComplete functionality

### 9.2 Test quiz functionality without ads

**Quiz tests:**
- Quiz pages load correctly
- Question navigation works
- Results display properly
- No ad-related errors

### 9.3 Verify all pages and navigation work correctly

**Navigation tests:**
- Shell navigation works
- Page parameters pass correctly
- Back navigation functions

### 9.4 Remove unused Xamarin.Forms references

**Cleanup:**
- Remove unused using statements
- Remove unused packages
- Clean up obsolete code

### 9.5 Clean up obsolete code and dependencies

**Code cleanup:**
- Remove ad-related code
- Remove gradient-related code
- Clean up unused resources

---

## Phase 10: Documentation & Final Validation

### 10.1 Document migration changes and new structure

**Documentation:**
- Update README with MAUI structure
- Document new build process
- Document dependency changes

### 10.2 Create build and deployment instructions

**Build instructions:**
- MAUI build commands
- Android deployment steps
- Development setup guide

### 10.3 Perform final testing on Android device/emulator

**Device testing:**
- Test on physical Android device
- Test on Android emulator
- Verify all functionality

### 10.4 Validate app performance and functionality

**Performance tests:**
- App startup time
- Navigation performance
- Memory usage
- Database performance

### 10.5 Prepare for future iOS/Windows platform additions

**Future preparation:**
- Document iOS migration steps
- Identify Windows-specific requirements
- Plan multi-platform testing

---

## Risk Assessment & Mitigation

### High-Risk Areas

1. **Syncfusion Package Compatibility**
   - Risk: MAUI versions may have breaking changes
   - Mitigation: Test each control individually, have fallback plans

2. **Custom Pj.Library Compatibility**
   - Risk: May not support .NET 9.0/MAUI
   - Mitigation: Contact vendor, consider alternatives

3. **Database Migration**
   - Risk: SQLite file access patterns may change
   - Mitigation: Thorough testing, backup strategies

4. **Platform Services**
   - Risk: Android-specific implementations may need updates
   - Mitigation: Test each service, update implementations

### Validation Checkpoints

- [ ] Project builds successfully after each phase
- [ ] App launches without crashes
- [ ] Database connectivity works
- [ ] Navigation functions correctly
- [ ] UI renders properly
- [ ] Syncfusion controls work
- [ ] No ad-related errors
- [ ] Performance is acceptable

---

## Success Criteria

1. **Functional Requirements:**
   - App launches successfully on Android
   - All pages and navigation work
   - Database operations function correctly
   - Quiz functionality works without ads
   - Syncfusion controls render and function properly

2. **Technical Requirements:**
   - Clean build with no errors
   - No Xamarin.Forms dependencies
   - MAUI-compatible package versions
   - Proper resource handling
   - Android-only targeting

3. **Performance Requirements:**
   - App startup time comparable to original
   - Smooth navigation and UI interactions
   - Efficient memory usage
   - Responsive database operations

---

## Timeline Estimate

- **Phase 1-2:** 2-3 days (Project setup and core migration)
- **Phase 3-4:** 2-3 days (Dependencies and platform code)
- **Phase 5-6:** 3-4 days (UI and data services)
- **Phase 7-8:** 2-3 days (Assets and build configuration)
- **Phase 9-10:** 2-3 days (Testing and documentation)

**Total Estimated Time:** 11-16 days

---

## Next Steps

1. Begin with Phase 1: Project Structure Analysis & Setup
2. Validate each phase before proceeding to the next
3. Document any issues or deviations from the plan
4. Update timeline estimates based on actual progress
5. Prepare for iOS/Windows platform additions after Android success

This migration plan provides a systematic approach to converting the Popes Xamarin.Forms project to MAUI while maintaining functionality and removing the requested components (ads and magic gradients).