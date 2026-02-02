using Pj.Library;
using Syncfusion.Maui.Buttons;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, ExtendedImage> extendImagesInPage = new();
        private readonly CancellationTokenSource _cancellationTokenSource;

        private const int DefaultHeightImageInDetailsPage = 300;
        private bool _isExternalImageLoadComplete = false;

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
                Application.Current.Dispatcher.Dispatch(() =>
                {
                    action();
                });
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }
        System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        public PersonaDetailPage()
        {
            Stopwatch.Start();
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
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

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
                ViewHelper.RunOnAppDispatcher(() =>
                {
                    DisplayAlert("Info", $"Page loaded in {Stopwatch.ElapsedMilliseconds} ms", "OK");
                });
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

            personaDetailViewModel.BannerAdsUnitId = SharedServiceCore.AdsConfig.BannerAdUnitId;
            personaDetailViewModel.IsPageBusy = true;
            personaDetailViewModel.IsDataLoading = true;
            personaDetailViewModel.LoadingMessage = "Loading details...";

            // Initialize loading facts control with quiz facts
            var loadingModel = new LoadingFactsModel
            {
                FactCount = 5,
                FactDisplayDurationMs = 4000,
                ShowMasterImage = true,
                AutoMarkFactsAsShown = true,
                MasterId = result
            };
            loadingFactsControl.Show(loadingModel);

            await Task.Yield();
            await Task.Delay(100);

            // Load data first, then bind to avoid ListView refresh issues
            await LoadSubPageItemDataDetails();
        }

        private async Task LoadSubPageItemDataDetails()
        {
            try
            {
                personaDetailViewModel.IsPageBusy = true;
                personaDetailViewModel.IsDataLoading = true;
                personaDetailViewModel.LoadingMessage = "Fetching data...";
                await Task.Yield();
                await Task.Delay(100);

                // Step A: Get the data from DB/Service on a background thread
                var persona = await SharedServices.WikiAppController.GetViewModelByIdAsync(MasterId.ToInteger());

                if (persona == null)
                {
                    personaDetailViewModel.IsDataLoading = false;
                    return;
                }

                // Step B: Set the Persona object immediately so headers/titles bind to the UI
                persona.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;
                if (!personaDetailViewModel.IsMetaDataAvailable)
                {
                    persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = persona.Name });
                }

                // Step C: Run heavy processing in parallel
                var buildListTask = Task.Run(() => BuildDetailItemModel(persona.Paragraphs));
                
                // Start image loading in background - don't block the UI
                _ = LoadImagesRequiredForThisPageAsync(persona.Pictures);

                var detailItems = await buildListTask;

                // Step D: Update UI in a single dispatcher call to minimize overhead
                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.LoadingMessage = "Rendering content...";
                });
                await Task.Yield();
                await Task.Delay(100);

                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.Persona = persona;
                });

                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.LoadingMessage = "Rendering detailed content...";
                });
                await Task.Yield();
                await Task.Delay(100);

                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.ItemDetailItems = detailItems;
                });
                
                // Step D: Finalize
                RunOnAppDispatcher(() =>
                {
                    personaDetailViewModel.LoadingMessage = "Finalizing...";
                    personaDetailViewModel.TriggerEvents();
                    loadingFactsControl.Hide();
                    personaDetailViewModel.IsDataLoading = false;
                    personaDetailViewModel.IsPageBusy = false;
                });
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
                RunOnAppDispatcher(() =>
                {
                    loadingFactsControl.Hide();
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

                                    if (tempList.Count > 0 && tempList.Last().Type == "Header3Text")
                                        tempList.Last().Content += $"{Environment.NewLine}{para3Content.Content}";
                                    else
                                        tempList.Add(new ItemDetailListViewModel { Type = "Header3Text", Content = para3Content.Content ?? "" });
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

                            // Update individual image as soon as it's downloaded
                            if (extendImagesInPage.TryGetValue(item.PictureLocalFileName ?? "", out var imageControl))
                            {
                                RunOnAppDispatcher(() =>
                                {
                                    var source = imageControl.CustomSource;
                                    imageControl.Source = null;
                                    imageControl.Source = source;
                                });
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
            try
            {
                // Image preview functionality can be added here if needed
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void btnCloseOnPopup_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Popup close functionality can be added here if needed
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void InitializeAdsControls()
        {
            try
            {
                // Ads removed as per migration plan
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        private async void btnStartQuiz_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Navigate to quiz page
                await Shell.Current.GoToAsync($"//QuizPage?MasterId={MasterId}");
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }


        public void CleanupResources()
        {
            personaDetailViewModel?.CleanupResources();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}