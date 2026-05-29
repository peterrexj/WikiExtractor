# WikiExtractor MAUI Apps — Feature Plan

Priority order: Acquisition → Retention → Polish

---

## Feature 1 — Share Item

**Status:** Not started

**Why:**
The only feature on this list that reaches people outside the app.
Every share is a free acquisition channel.
No ad budget needed — users do the work.

**What:**
A share button on the detail page.
Tapping it opens the native iOS/Android share sheet.
The shared text includes the item name, a one-line description, and the Wikipedia URL.

**How:**
- Add a share toolbar button to `PersonaDetailPage.xaml`.
- In `PersonaDetailPage.xaml.cs` call `Share.Default.RequestAsync` with `Title`, `Text`, and `Uri` built from `persona.Name`, `persona.MainContent`, and `persona.WikiPath`.
- No new DB tables. No new pages. No new ViewModels.
- One method, one button.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml.cs`

**Effort:** Small — half a day.

---

## Feature 2 — Favourites

**Status:** Not started

**Why:**
Gives users a personal collection inside the app.
A user with saved favourites has a reason to return.
Pairs naturally with Feature 3 (streak) to form a habit loop.

**What:**
A heart icon on each list card and on the detail page.
A "Favourites" filter chip in the list toolbar alongside the existing search.
Favourited items persist across sessions.

**How:**
- Add `FavouriteTrackerModel` to `src/WikiExtractor.Process/WikiExtractor.Process/DbModels/UserStore/` — mirrors the existing `ItemReadTrackerModel`.
- Add `UpdateFavourite` and `IsFavourite` methods to `WikiAppController`.
- Add `IsFavourite` bool property to `PersonaViewModel`.
- Add heart button to `WikiListOfItemsPage.xaml` list card (same position as the read badge).
- Add heart button to `PersonaDetailPage.xaml` toolbar.
- Add a "Favourites only" toggle to `PersonaListViewModel.ApplySortAndFilter`.

**Files touched:**
- `src/WikiExtractor.Process/.../DbModels/UserStore/FavouriteTrackerModel.cs` (new)
- `src/WikiExtractor.Process/.../Process/WikiAppController.cs`
- `src/WikiExtractor.Process/.../ViewModels/PersonaViewModel.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/ViewModels/PersonaListViewModel.cs`

**Effort:** Medium — one to two days.

---

## Feature 3 — Reading Streak

**Status:** Not started

**Why:**
A streak gives users a daily reason to open the app.
Read tracking already exists (`ItemReadTrackerModel`).
Streaks are one of the highest-retention mechanics in content apps.

**What:**
A small streak counter shown on the list page header or settings page.
Counts consecutive days the user opened the app and read at least one item.
Resets to zero if a day is skipped.
A "best streak" record is kept separately.

**How:**
- Add `StreakTrackerModel` to UserStore DB models with `LastOpenDate` and `CurrentStreak` and `BestStreak` int fields.
- Add `UpdateStreak` and `GetStreak` to `WikiAppController`.
- Call `UpdateStreak` in `WikiListOfItemsPage.xaml.cs` on `OnAppearing`.
- Show the streak count as a small badge/label in the list page toolbar or in `SettingsPage.xaml`.

**Files touched:**
- `src/WikiExtractor.Process/.../DbModels/UserStore/StreakTrackerModel.cs` (new)
- `src/WikiExtractor.Process/.../Process/WikiAppController.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml.cs`

**Effort:** Small-Medium — one day.

---

## Feature 4 — Image Full-Screen Viewer

**Status:** Not started

**Why:**
The pictures tab already has good images but no way to zoom or view them properly.
This is the kind of polish that drives App Store ratings up.
The tap handler `lstImageEffectsLayer_Tapped` in `PersonaDetailPage.xaml.cs` is already stubbed with a "reserved for future full-screen preview" comment — the hook is already there.

**What:**
Tap any image in the pictures tab to open it full-screen.
Pinch to zoom, swipe to dismiss.
Caption shown at the bottom.

**How:**
- Add a `FullScreenImagePage.xaml` — a simple dark `ContentPage` with a `ScrollView` containing a zoomable `Image`.
- Wire `lstImageEffectsLayer_Tapped` in `PersonaDetailPage.xaml.cs` to push `FullScreenImagePage` with the tapped item's `PictureLocalPath` and `PictureCaption`.
- Use MAUI's built-in `PinchGestureRecognizer` and `TapGestureRecognizer` (double-tap to zoom) for zoom.
- Swipe down or back button to dismiss.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/FullScreenImagePage.xaml` (new)
- `src/Maui/Library/WikiExtractor.Maui.App/Views/FullScreenImagePage.xaml.cs` (new)
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml.cs`

**Effort:** Medium — one to two days.

---

## Feature 5 — Random / Surprise Me

**Status:** Not started

**Why:**
Low effort, high delight.
Users who have read most items still get fresh engagement.
Works especially well in quiz-style apps where discovery is part of the fun.

**What:**
A shuffle/dice icon button in the list page toolbar.
Tapping it navigates directly to a random item's detail page.

**How:**
- Add a shuffle button to the toolbar in `WikiListOfItemsPage.xaml`.
- In `WikiListOfItemsPage.xaml.cs` pick a random `PersonaViewModel` from `FilteredPersonas` and navigate to `PersonaDetailPage` with that item's Id.
- No new DB tables or ViewModels.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml.cs`

**Effort:** Small — two hours.

---

## Feature 6 — Offline Indicator

**Status:** Not started

**Why:**
Now that images download on-demand, users on poor connections will wonder why images are missing.
A clear offline banner removes that confusion and sets expectations.
Prevents negative reviews that blame the app for a network problem.

**What:**
A slim banner at the top of the list page and detail page when the device has no internet.
Banner disappears automatically when connectivity is restored.
No action required from the user.

**How:**
- Subscribe to `Connectivity.Current.ConnectivityChanged` in `WikiListOfItemsPage.xaml.cs` and `PersonaDetailPage.xaml.cs`.
- Bind a `IsOffline` bool on the respective ViewModels.
- Add a collapsed `Border` banner at the top of each page XAML, visible only when `IsOffline` is true.
- Unsubscribe in `OnDisappearing`.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/ViewModels/PersonaListViewModel.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/PersonaDetailPage.xaml.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/ViewModels/PersonaDetailViewModel.cs`

**Effort:** Small — half a day.

---

## Feature 7 — Font Size Control in Settings

**Status:** Not started

**Why:**
`AppSettingsService` already has `DEFAULT_PARAGRAPH_FONT_SIZE`, `MIN_PARAGRAPH_FONT_SIZE`, `MAX_PARAGRAPH_FONT_SIZE`, and `GetParagraphFontSizeAsync` / `SetParagraphFontSizeAsync`.
The dynamic resource `WikiAppParagraphFontSize` is already applied in `PersonaDetailPage`.
This is almost entirely wiring an existing mechanism.
Accessibility win — older users, users with vision difficulty.

**What:**
A font size slider in `SettingsPage.xaml`.
Changes take effect immediately in the detail page paragraph text.
Setting persists across sessions.

**How:**
- Add a `Slider` to `SettingsPage.xaml` bound between `MIN_PARAGRAPH_FONT_SIZE` (10) and `MAX_PARAGRAPH_FONT_SIZE` (24).
- On value changed, call `AppSettingsService.SetParagraphFontSizeAsync` and update `Application.Current.Resources["WikiAppParagraphFontSize"]`.
- Load the saved value in `SettingsPage` `OnAppearing`.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/SettingsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/SettingsPage.xaml.cs`

**Effort:** Small — two to three hours.

---

## Feature 8 — Quiz Timed Mode

**Status:** Not started

**Why:**
Increases replay value for users who have already done the quiz.
Session length goes up — a key retention signal for app stores.
Adds tension and excitement to what is otherwise a relaxed quiz.

**What:**
An optional timer per question (e.g. 15 seconds).
A `ProgressBar` counts down visually.
If time runs out the question is marked wrong and the next question loads.
Timer mode is toggled before starting the quiz.

**How:**
- Add a timed mode toggle to the quiz start flow in `WikiListOfItemsPage.xaml` or a pre-quiz options sheet.
- In `QuizPageViewModel`, add a `TimeRemaining` double property and a `System.Timers.Timer` or `CancellationTokenSource`-based countdown per question.
- Bind a `ProgressBar` in `QuizPage.xaml` to `TimeRemaining` (normalised 0–1).
- On timer expiry, call the existing wrong-answer path.
- Pass a `IsTimedMode` bool through the navigation parameter to `QuizPageViewModel`.

**Files touched:**
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/WikiListOfItemsPage.xaml.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/QuizPage.xaml`
- `src/Maui/Library/WikiExtractor.Maui.App/ViewModels/QuizPageViewModel.cs`

**Effort:** Medium — one to two days.

---

## Feature 9 — Progress Stats Page

**Status:** Not started

**Why:**
Users who can see their progress feel invested.
"273 of 300 popes read" is a stronger hook than a plain list.
Builds on read tracking and quiz score data that already exists.

**What:**
A stats sheet or page accessible from the settings or list toolbar.
Shows: total items, items read, percentage complete, total quiz attempts, best quiz score, current streak, best streak.

**How:**
- Add `GetProgressStats` method to `WikiAppController` — queries `ItemReadTrackerModel` count against `MasterRepository` total count, and reads streak data.
- Add a `ProgressStatsPage.xaml` (or a bottom sheet / modal) with the stats displayed as cards.
- Navigate to it from a button in `SettingsPage.xaml` or the list toolbar.

**Files touched:**
- `src/WikiExtractor.Process/.../Process/WikiAppController.cs`
- `src/Maui/Library/WikiExtractor.Maui.App/Views/ProgressStatsPage.xaml` (new)
- `src/Maui/Library/WikiExtractor.Maui.App/Views/ProgressStatsPage.xaml.cs` (new)
- `src/Maui/Library/WikiExtractor.Maui.App/Views/SettingsPage.xaml`

**Effort:** Medium — one to two days.

---

## Recommended Implementation Order

| # | Feature | Impact | Effort | Do next |
|---|---|---|---|---|
| 1 | Share Item | Acquisition | Small | Yes — first |
| 2 | Favourites | Retention | Medium | Yes — second |
| 3 | Reading Streak | Retention | Small-Medium | Yes — third |
| 5 | Random / Surprise Me | Engagement | Small | Slot in anywhere |
| 7 | Font Size in Settings | Accessibility | Small | Slot in anywhere |
| 6 | Offline Indicator | Polish | Small | After image fixes settle |
| 4 | Full-Screen Image Viewer | Polish + Ratings | Medium | After Favourites |
| 8 | Quiz Timed Mode | Retention | Medium | After streak |
| 9 | Progress Stats Page | Retention | Medium | After streak + favourites |
