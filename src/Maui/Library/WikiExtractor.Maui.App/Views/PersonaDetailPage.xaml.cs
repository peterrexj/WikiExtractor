using Pj.Library;
using Syncfusion.Maui.Buttons;
using RoundRectangle = Microsoft.Maui.Controls.Shapes.RoundRectangle;
using WikiExtractor.Exts;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.Controls;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.ViewModels;
using WikiExtractor.Maui.App.Services;
using WikiExtractor.Maui.App.Models;

namespace WikiExtractor.Maui.App.Views
{
    [QueryProperty(nameof(MasterId), nameof(MasterId))]
    public partial class PersonaDetailPage : ContentPage
    {
        public string MasterId { get; set; }

        private PersonaDetailViewModel personaDetailViewModel;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private const int DefaultHeightImageInDetailsPage = 300;
        private bool _isExternalImageLoadComplete = false;

        // Title bar elements — built imperatively so iOS Shell.TitleView renders correctly
        private Image imgPrimary;
        private Label lblTitleName;
        private Label lblTitleSubtitle;

        private void CaptureErrorOnPage(Exception exception)
        {
            ExceptionHandler.CaptureException(exception,
                personaDetailViewModel?.Persona?.Name ?? "",
                personaDetailViewModel?.Persona?.WikiPath ?? "");
        }

        public void RunOnAppDispatcher(Action action)
        {
            try
            {
                Application.Current?.Dispatcher.Dispatch(() =>
                {
                    try { action(); }
                    catch (Exception ex) { CaptureErrorOnPage(ex); }
                });
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }
        public PersonaDetailPage()
        {
            try
            {
                InitializeComponent();

                // Set the DataTemplateSelector properties
                var itemTemplateSelector = (ItemDetailListTemplateSelector)Resources["ItemDetailListTemplateSelector"];
                itemTemplateSelector.Header2Template = (DataTemplate)Resources["Header2ListItemTemplate"];
                itemTemplateSelector.Header3Template = (DataTemplate)Resources["Header3ListItemTemplate"];
                itemTemplateSelector.ParagraphContentTemplate = (DataTemplate)Resources["ParagraphContentListItemTemplate"];
                itemTemplateSelector.ImageTemplate = (DataTemplate)Resources["ImageListItemTemplate"];
                _cancellationTokenSource = new CancellationTokenSource();

                // Seed the dynamic resource so ParagraphContentListItemTemplate can resolve it before data loads
                if (!Application.Current.Resources.ContainsKey("WikiAppParagraphFontSize"))
                    Application.Current.Resources["WikiAppParagraphFontSize"] = AppSettingsService.DEFAULT_PARAGRAPH_FONT_SIZE;

                BuildTitleView();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void BuildTitleView()
        {
            bool isTablet = DeviceIdiom.Tablet == DeviceInfo.Idiom;
            bool isIos = DeviceInfo.Platform == DevicePlatform.iOS;

            double imageSize = isIos ? (isTablet ? 36 : 32) : (isTablet ? 40 : 34);
            double cornerRadius = imageSize / 2;
            double imageColWidth = imageSize;
            double navHeight = isIos ? 44 : 56;
            double nameFontSize = isTablet ? 18 : 16;
            double subtitleFontSize = isTablet ? 13 : 11;
            double titleViewWidth = isTablet ? 460 : 300;

            imgPrimary = new Image
            {
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var imageBorder = new Border
            {
                HeightRequest = imageSize,
                WidthRequest = imageSize,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                StrokeThickness = 2,
                Content = imgPrimary
            };
            imageBorder.SetDynamicResource(Border.BackgroundProperty, "WikiAppPersonaDetailProfileImageBg");
            imageBorder.SetDynamicResource(Border.StrokeProperty, "WikiAppPrimaryAccentColor");
            imageBorder.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(cornerRadius) };

            lblTitleName = new Label
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = nameFontSize,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.End,
                VerticalTextAlignment = TextAlignment.End,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            lblTitleName.SetDynamicResource(Label.TextColorProperty, "WikiAppPersonaDetailNameTextColor");

            lblTitleSubtitle = new Label
            {
                FontAttributes = FontAttributes.Italic,
                FontSize = subtitleFontSize,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Start,
                VerticalTextAlignment = TextAlignment.Start,
                LineBreakMode = LineBreakMode.TailTruncation,
                Opacity = 0.8
            };
            lblTitleSubtitle.SetDynamicResource(Label.TextColorProperty, "WikiAppPersonaDetailSubtitleTextColor");

            var textGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(6, 0, isTablet ? 8 : 6, 0),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };
            Grid.SetRow(lblTitleName, 0);
            Grid.SetRow(lblTitleSubtitle, 1);
            textGrid.Children.Add(lblTitleName);
            textGrid.Children.Add(lblTitleSubtitle);

            var wikiLabel = new Label
            {
                Text = "Wiki",
                FontAttributes = FontAttributes.Bold,
                FontSize = isTablet ? 12 : 10,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var wikiTapGesture = new TapGestureRecognizer();
            wikiTapGesture.SetBinding(TapGestureRecognizer.CommandProperty,
                new Binding("BindingContext.TapHyperLinkToWikiPage", source: this));
            wikiTapGesture.SetBinding(TapGestureRecognizer.CommandParameterProperty,
                new Binding("BindingContext.Persona.WikiPath", source: this));

            var wikiBorder = new Border
            {
                Padding = new Thickness(isTablet ? 12 : 8, isTablet ? 6 : 4),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                Content = wikiLabel
            };
            wikiBorder.SetDynamicResource(Border.BackgroundProperty, "WikiAppPrimaryAccentColor");
            wikiBorder.GestureRecognizers.Add(wikiTapGesture);

            var titleGrid = new Grid
            {
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = navHeight,
                WidthRequest = titleViewWidth,
                Margin = new Thickness(isIos ? -12 : -16, 0, 0, 0),
                Padding = new Thickness(0, 0, isTablet ? 8 : 6, 0),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(imageColWidth) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            Grid.SetColumn(imageBorder, 0);
            Grid.SetColumn(textGrid, 1);
            Grid.SetColumn(wikiBorder, 2);

            titleGrid.Children.Add(imageBorder);
            titleGrid.Children.Add(textGrid);
            titleGrid.Children.Add(wikiBorder);

            _titleGrid = titleGrid;
            // Applied in OnNavigatedTo once the Shell nav bar is live
        }

        private Grid _titleGrid;

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (_titleGrid != null)
                Shell.SetTitleView(this, _titleGrid);
        }

        private void ApplySwitchThemeColors()
        {
            try
            {
                var settings = swtReadItem.SwitchSettings;
                settings.SetDynamicResource(Syncfusion.Maui.Buttons.SwitchSettings.TrackBackgroundProperty, "WikiAppSwitchTrackColorOn");
                settings.SetDynamicResource(Syncfusion.Maui.Buttons.SwitchSettings.ThumbBackgroundProperty, "WikiAppSwitchThumbColorOn");
            }
            catch { }
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                await LoadWithPageBinding();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            personaDetailViewModel?.CancelSpeech();
            _cancellationTokenSource?.Cancel();
        }

        private async Task LoadWithPageBinding()
        {
            int.TryParse(MasterId, out var result);

            personaDetailViewModel ??= new PersonaDetailViewModel();

            if (BindingContext == null)
            {
                BindingContext = personaDetailViewModel;
            }

            await personaDetailViewModel.LoadFontSizeAsync();

            personaDetailViewModel.BannerAdsUnitId = SharedServiceCore.AdsConfig.BannerAdUnitId;
            personaDetailViewModel.IsPageBusy = true;
            personaDetailViewModel.IsDataLoading = true;
            personaDetailViewModel.LoadingMessage = "Loading details...";

            // Initialize loading facts control (simplified - one fact per overlay)
            /*var loadingModel = new LoadingFactsModel
            {
                ShowMasterImage = true,
                AutoMarkFactsAsShown = true
            };
            loadingFactsControl.Show(loadingModel);*/

            await LoadSubPageItemDataDetails();
        }

        private async Task LoadSubPageItemDataDetails()
        {
            try
            {
                personaDetailViewModel.IsPageBusy = true;
                personaDetailViewModel.IsDataLoading = true;
                personaDetailViewModel.LoadingMessage = "Fetching data...";

                // Let the Shell enter animation complete before hitting the DB on the main thread
                await Task.Delay(80);

                var persona = await SharedServices.WikiAppController.GetViewModelByIdAsync(MasterId.ToInteger());

                if (persona == null)
                {
                    personaDetailViewModel.IsDataLoading = false;
                    return;
                }

                persona.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;
                if (!personaDetailViewModel.IsMetaDataAvailable)
                {
                    persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = persona.Name });
                }

                var buildListTask = Task.Run(() => BuildDetailItemModel(persona.Paragraphs));
                _ = LoadImagesRequiredForThisPageAsync(persona.Pictures);
                var detailItems = await buildListTask;

                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.Persona = persona;
                    personaDetailViewModel.ItemDetailItems = detailItems;
                    personaDetailViewModel.LoadingMessage = "Finalizing...";
                    personaDetailViewModel.TriggerEvents();

                    // Shell.TitleView does not inherit BindingContext on iOS — set directly
                    lblTitleName.Text = persona.Name;
                    lblTitleSubtitle.Text = persona.NameSubstitueFormatted;
                    if (imgPrimary != null)
                    {
                        var picUrl = persona.PicturePrimaryPath;
                        if (string.IsNullOrEmpty(picUrl) || picUrl == "NoImageAvailable.png")
                        {
                            imgPrimary.Source = ImageSource.FromFile("no_image_available.png");
                        }
                        else
                        {
                            var localPath = Path.Combine(ConfigData.LocalStorageCacheFolderPath, persona.PicturePrimaryLocalFileName);
                            imgPrimary.Source = File.Exists(localPath)
                                ? ImageSource.FromFile(localPath)
                                : ImageSource.FromFile("no_image_available.png");
                            if (!File.Exists(localPath))
                                _ = DownloadAndRefreshTitleImageAsync(picUrl, localPath);
                        }
                    }

                    // Re-apply after text is populated — MAUI Shell nav bar does not re-render
                    // child label text changes unless the TitleView reference is reassigned.
                    if (_titleGrid != null)
                        Shell.SetTitleView(this, _titleGrid);

                    // Set initial selected tab colour (Syncfusion VSM+DynamicResource is unreliable)
                    if (Application.Current?.Resources.TryGetValue("WikiAppTabTextColorSelected", out var selObj) == true && selObj is Color selectedColor &&
                        Application.Current?.Resources.TryGetValue("WikiAppTabTextColorNormal", out var normObj) == true && normObj is Color normalColor)
                    {
                        tabUsefulInfo.TextColor = selectedColor;
                        tabDetailContent.TextColor = normalColor;
                        tabPictures.TextColor = normalColor;
                    }

                    ApplySwitchThemeColors();

                    /*loadingFactsControl.Hide();*/
                    personaDetailViewModel.IsDataLoading = false;
                    personaDetailViewModel.IsPageBusy = false;
                });
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
                RunOnAppDispatcher(() =>
                {
                    /*loadingFactsControl.Hide();*/
                    personaDetailViewModel.IsDataLoading = false;
                });
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<ItemDetailListViewModel> BuildDetailItemModel(List<Paragraph2ContentViewModel> paraContents)
        {
            if (paraContents == null) return new System.Collections.ObjectModel.ObservableCollection<ItemDetailListViewModel>();

            // USE A LOCAL LIST to avoid triggering UI updates for every single row
            var tempList = new List<ItemDetailListViewModel>();

            try
            {
                var paragraphs = paraContents.OrderBy(f => f.Sequence).GroupBy(f => f.Header2);

                foreach (var grpContent in paragraphs)
                {
                    // Logic to check if group has content...
                    if (grpContent.Any(f => f.Content.HasValue()) || grpContent.Any(f => f.Para3Containers.SelectMany(f => f.Para3s).Any(g => g.Content.HasValue())))
                    {
                        var header2 = grpContent.FirstOrDefault(f => f.Header2.HasValue());
                        if (header2?.Header2.HasValue() == true)
                            tempList.Add(BuildHeader2Row(header2));

                        foreach (var paraContent in grpContent)
                        {
                            foreach (var img in paraContent.PicLinks) tempList.Add(BuildImageRow(img));

                            if (paraContent.Content.HasValue())
                            {
                                // Optimization: Combine sequential text into one block
                                if (tempList.Count > 0 && tempList.Last().Type == "Header2Text")
                                    tempList.Last().Content += $"{Environment.NewLine}{paraContent.Content}";
                                else
                                    tempList.Add(BuildPara2ContentRow(paraContent));
                            }

                            // Process Sub-paragraphs (Para3)
                            foreach (var para3Grp in paraContent.Para3Containers)
                            {
                                if (para3Grp.Header.HasValue()) tempList.Add(BuildHeader3Row(para3Grp));

                                foreach (var para3Content in para3Grp.Para3s)
                                {
                                    foreach (var img in para3Content.PicLinks) tempList.Add(BuildImageRow(img));

                                    if (!para3Content.Content.HasValue()) continue;

                                    if (tempList.Count > 0 && tempList.Last().Type == "Header3Text")
                                        tempList.Last().Content += $"{Environment.NewLine}{para3Content.Content}";
                                    else
                                        tempList.Add(new ItemDetailListViewModel { Type = "Header3Text", Content = para3Content.Content });
                                }
                            }
                        }
                    }
                }

                return new System.Collections.ObjectModel.ObservableCollection<ItemDetailListViewModel>(tempList);
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
                return new System.Collections.ObjectModel.ObservableCollection<ItemDetailListViewModel>();
            }
        }

        private async Task LoadImagesRequiredForThisPageAsync(List<PictureViewModel> pictureViewModels)
        {
            System.Threading.SemaphoreSlim semaphore = null;
            try
            {
                // Null-safe access prevents the "Object Reference" error
                if (pictureViewModels == null || !pictureViewModels.Any())
                {
                    _isExternalImageLoadComplete = true;
                    return;
                }

                var requiredItems = (from pic in pictureViewModels
                                     let lPath = Path.Combine(ConfigData.LocalStorageCacheFolderPath, pic.PictureLocalFileName ?? "")
                                     let toDownload = CacheImageDownloadHelper.ValidateCachedLocalFile(lPath, pic.PicturePath)
                                     select new { LocalFilePath = lPath, pic.PicturePath, pic.Width, pic.Height, pic.PictureLocalFileName, DownloadRequired = toDownload })
                                     .ToList();

                var downloads = requiredItems.Where(f => f.DownloadRequired).ToList();
                
                if (!downloads.Any())
                {
                    _isExternalImageLoadComplete = true;
                    return;
                }

                // Update UI to show we're loading images
                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.LoadingMessage = $"Loading {downloads.Count} images...";
                });

                // Throttle concurrent downloads to avoid overwhelming slow networks
                const int maxConcurrentDownloads = 3;
                semaphore = new System.Threading.SemaphoreSlim(maxConcurrentDownloads, maxConcurrentDownloads);
                
                int completedCount = 0;
                var downloadTasks = downloads.Select(async item =>
                {
                    try
                    {
                        await semaphore.WaitAsync(_cancellationTokenSource.Token);
                        try
                        {
                            await CacheImageDownloadHelper.DownloadImage(
                                item.LocalFilePath, 
                                item.PicturePath, 
                                _cancellationTokenSource.Token, 
                                item.Width, 
                                item.Height, 
                                90);

                            // Notify the bound ExtendedImage by reassigning ImageLocalPath — the
                            // CustomSource binding fires ApplySource which now finds the file on disk.
                            var localPath = item.LocalFilePath;
                            var detailItem = personaDetailViewModel.ItemDetailItems
                                ?.FirstOrDefault(d => d.ImageFileName == item.PictureLocalFileName);
                            if (detailItem != null)
                            {
                                RunOnAppDispatcher(() => detailItem.ImageLocalPath = localPath);
                            }

                            // Update progress
                            var current = System.Threading.Interlocked.Increment(ref completedCount);
                            RunOnAppDispatcher(() =>
                            {
                                personaDetailViewModel.LoadingMessage = $"Loaded {current}/{downloads.Count} images...";
                            });
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when user navigates away - silently ignore
                        System.Diagnostics.Debug.WriteLine($"Image download cancelled: {item.PicturePath}");
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail - continue with other downloads
                        System.Diagnostics.Debug.WriteLine($"Failed to download image: {item.PicturePath} - {ex.Message}");
                    }
                });

                await Task.WhenAll(downloadTasks);
                
                _isExternalImageLoadComplete = true;
                
                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.LoadingMessage = "All images loaded!";
                });
            }
            catch (OperationCanceledException)
            {
                // Expected when user navigates away from page
                System.Diagnostics.Debug.WriteLine("Image loading cancelled by user navigation");
                _isExternalImageLoadComplete = true;
            }
            catch (Exception ex) 
            { 
                CaptureErrorOnPage(ex);
                _isExternalImageLoadComplete = true;
            }
            finally
            {
                // Clean up semaphore
                semaphore?.Dispose();
            }
        }

        private async Task DownloadAndRefreshTitleImageAsync(string url, string localPath)
        {
            try
            {
                using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                const int maxAttempts = 2;
                byte[] bytes = null;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        bytes = await client.GetByteArrayAsync(url, _cancellationTokenSource.Token);
                        break;
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception) when (attempt < maxAttempts)
                    {
                        await Task.Delay(1000, _cancellationTokenSource.Token);
                    }
                }
                if (bytes == null || _cancellationTokenSource.Token.IsCancellationRequested) return;
                await File.WriteAllBytesAsync(localPath, bytes, _cancellationTokenSource.Token);
                RunOnAppDispatcher(() =>
                {
                    if (imgPrimary != null && File.Exists(localPath))
                        imgPrimary.Source = ImageSource.FromFile(localPath);
                });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DetailPage] Title image download failed: {ex.Message}");
            }
        }

        private ItemDetailListViewModel BuildPara2ContentRow(Paragraph2ContentViewModel para) => new() { Type = "Header2Text", Content = para.Content, };
        private ItemDetailListViewModel BuildPara3ContentRow(Paragraph3ContentViewModel para) => new() { Type = "Header3Text", Content = para.Content, };

        private ItemDetailListViewModel BuildHeader2Row(Paragraph2ContentViewModel para) => new()
        {
            Type = "Header2",
            Content = para.Header2 ?? "",
            ContentLinkId = para.Id,
            IsPlayButtonRequired = para.ContainsHeader2Content
        };

        private ItemDetailListViewModel BuildHeader3Row(Paragraph3ContainerViewModel para) => new()
        {
            Type = "Header3",
            Content = para.Header ?? "",
            ContentLinkId = para.Para3s.FirstOrDefault()?.Id ?? 0,
            IsPlayButtonRequired = para.Para3s.Any(f => f.Content.HasValue())
        };

        private ItemDetailListViewModel BuildImageRow(PictureViewModel pictureViewModel) => new()
        {
            Type = "Image",
            ImageLocalPath = Path.Combine(ConfigData.LocalStorageCacheFolderPath, pictureViewModel.PictureLocalFileName),
            ImageFileName = pictureViewModel.PictureLocalFileName,
            ImageHeight = (pictureViewModel.Height <= 0 || pictureViewModel.Height > DefaultHeightImageInDetailsPage) ? DefaultHeightImageInDetailsPage : pictureViewModel.Height,
            ImageDimension = $"{(pictureViewModel.Height <= 0 ? DefaultHeightImageInDetailsPage : pictureViewModel.Height)},{(pictureViewModel.Width <= 0 ? DefaultHeightImageInDetailsPage : pictureViewModel.Width)}",
            ImageCaption = pictureViewModel.PictureCaption,
        };

        private void First_TabItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                tabView.SelectedIndex = 0;
                personaDetailViewModel.CancelSpeech();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void Second_TabItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_isExternalImageLoadComplete)
                {
                    tabView.SelectedIndex = personaDetailViewModel.IsMetaDataAvailable ? 1 : 0;
                }
                else
                {
                    tabView.SelectedIndex = 0;
                }
                personaDetailViewModel.CancelSpeech();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void Third_TabItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (_isExternalImageLoadComplete)
                {
                    int tabIndex = personaDetailViewModel.IsMetaDataAvailable && personaDetailViewModel.IsPicturesAvailable ? 2 :
                                        personaDetailViewModel.IsMetaDataAvailable == false && personaDetailViewModel.IsPicturesAvailable == false ? 0 : 1;
                    tabView.SelectedIndex = tabIndex;
                }
                else
                {
                    tabView.SelectedIndex = 0;
                }

                personaDetailViewModel.CancelSpeech();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private async void swtReadItem_StateChanged(object sender, SwitchStateChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var newValue = e.NewValue ?? false;
                    SharedServices.WikiAppController.UpdateItemRead(SharedServices.PageDataTransferModel.Name, newValue);
                    SharedServices.PageDataTransferModel.IsMarkedAsViewed = newValue;
                }
                catch (Exception ex)
                {
                    CaptureErrorOnPage(ex);
                }
            });
        }

        private void lstImageEffectsLayer_Tapped(object sender, EventArgs e)
        {
            // Image tap — reserved for future full-screen preview
        }

        private void tabView_SelectionChanged(object sender, Syncfusion.Maui.TabView.TabSelectionChangedEventArgs e)
        {
            try
            {
                var tabs = new[] { tabUsefulInfo, tabDetailContent, tabPictures };
                if (Application.Current?.Resources.TryGetValue("WikiAppTabTextColorSelected", out var selObj) == true && selObj is Color selectedColor &&
                    Application.Current?.Resources.TryGetValue("WikiAppTabTextColorNormal", out var normObj) == true && normObj is Color normalColor)
                {
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        if (tabs[i] != null)
                            tabs[i].TextColor = i == e.NewIndex ? selectedColor : normalColor;
                    }
                }
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }
    }
}