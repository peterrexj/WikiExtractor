using Pj.Library;
using Syncfusion.Maui.Buttons;
using System.Collections.Concurrent;
using WikiExtractor.Exts;
using WikiExtractor.ViewModels;
using WikiExtractor.Maui.App.Controls;
using WikiExtractor.Maui.App.Exts;
using WikiExtractor.Maui.App.ViewModels;
using WikiExtractor.Maui.App.Services;

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
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                LoadWithPageBinding();
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

        private void LoadWithPageBinding()
        {
            int.TryParse(MasterId, out var result);

            personaDetailViewModel ??= new PersonaDetailViewModel();
            personaDetailViewModel.IsBusy = true;
            personaDetailViewModel.IsDataLoading = true;
            personaDetailViewModel.LoadingMessage = "Loading details...";
            personaDetailViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();

            // Set binding context early to show loading indicator
            if (BindingContext == null)
            {
                BindingContext = personaDetailViewModel;
            }

            // Load data first, then bind to avoid ListView refresh issues
            LoadSubPageItemDataDetails();
        }

        private void LoadImagesRequiredForThisPageAsync()
        {
            try
            {
                if (personaDetailViewModel.Persona.Pictures.Any())
                {
                    //Build required parameters
                    var requiredItems = from pic in personaDetailViewModel.Persona.Pictures
                                        let lPath = Path.Combine(ConfigData.LocalStorageCacheFolderPath, pic.PictureLocalFileName)
                                        let toDownload = CacheImageDownloadHelper.ValidateCachedLocalFile(lPath, pic.PicturePath)
                                        select new
                                        {
                                            LocalFilePath = lPath,
                                            pic.PicturePath,
                                            pic.Width,
                                            pic.Height,
                                            DownloadRequired = toDownload
                                        };

                    if (requiredItems.Any(f => f.DownloadRequired))
                    {
                        //Only select download those images which are rendered in the Details tab. The carousel uses image rather the extendedImage
                        var tasks = requiredItems.Where(f => f.DownloadRequired).Select(async item =>
                        {
                            await CacheImageDownloadHelper.DownloadImage(item.LocalFilePath, item.PicturePath, _cancellationTokenSource.Token, item.Width, item.Height, 90);
                        });

                        Task.WhenAll(tasks).ContinueWith(t =>
                        {
                            if (extendImagesInPage != null)
                            {
                                foreach (var exImgCtrl in extendImagesInPage)
                                {
                                    try
                                    {
                                        var currentSource = exImgCtrl.Value.CustomSource;
                                        exImgCtrl.Value.Source = null;
                                        exImgCtrl.Value.Source = currentSource;
                                    }
                                    catch (Exception ex)
                                    {
                                        CaptureErrorOnPage(ex);
                                    }
                                }
                                extendImagesInPage = null;
                            }
                        });
                        _isExternalImageLoadComplete = true;
                    }
                    else
                    {
                        _isExternalImageLoadComplete = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private async void LoadSubPageItemDataDetails()
        {
            await Task.Run(() =>
            {
                try
                {
                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.LoadingMessage = "Loading persona data...";
                    });

                    if (personaDetailViewModel?.Persona == null)
                    {
                        personaDetailViewModel.Persona = SharedServices.WikiAppController.GetViewModelById(MasterId.ToInteger());
                    }

                    personaDetailViewModel.Persona.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;

                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.LoadingMessage = "Processing metadata...";
                    });

                    // Populate metadata before binding to prevent ListView refresh
                    if (personaDetailViewModel.IsMetaDataAvailable == false)
                    {
                        personaDetailViewModel.Persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = personaDetailViewModel.Persona.Name });
                    }

                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.LoadingMessage = "Building content structure...";
                    });

                    BuildDetailItemModel();

                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.LoadingMessage = "Loading images...";
                    });

                    LoadImagesRequiredForThisPageAsync();
                    
                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.LoadingMessage = "Finalizing...";
                        
                        //InitializeAdsControls();
                        // Trigger events but without Persona refresh to prevent scroll reset
                        personaDetailViewModel.TriggerEvents();
                        
                        // Hide loading indicator
                        personaDetailViewModel.IsDataLoading = false;
                    });
                }
                catch (Exception ex)
                {
                    CaptureErrorOnPage(ex);
                    RunOnAppDispatcher(() =>
                    {
                        personaDetailViewModel.IsDataLoading = false;
                    });
                }
                finally
                {
                    personaDetailViewModel.IsBusy = false;
                }
            });
        }

        private void BuildDetailItemModel()
        {
            try
            {
                foreach (var grpContent in personaDetailViewModel?.Persona?.Paragraphs.OrderBy(f => f.Sequence).GroupBy(f => f.Header2))
                {
                    if (grpContent.Any(f => f.Content.HasValue()) || grpContent.Any(f => f.Para3Containers.SelectMany(f => f.Para3s).Any(g => g.Content.HasValue())))
                    {
                        //Render Para2 header
                        var header2 = grpContent.FirstOrDefault(f => f.Header2.HasValue());
                        if (header2?.Header2.HasValue() == true)
                        {
                            personaDetailViewModel.ItemDetailItems.Add(BuildHeader2Row(header2));
                        }

                        //Render Para2 contents
                        foreach (var paraContent in grpContent)
                        {
                            foreach (var img in paraContent.PicLinks)
                            {
                                personaDetailViewModel.ItemDetailItems.Add(BuildImageRow(img));
                            }
                            if (paraContent.Content.HasValue())
                            {
                                if (personaDetailViewModel.ItemDetailItems.Any() && personaDetailViewModel.ItemDetailItems.Last()?.Type == "Header2Text")
                                {
                                    personaDetailViewModel.ItemDetailItems.Last().Content = $"{personaDetailViewModel.ItemDetailItems.Last().Content}{Environment.NewLine}{paraContent.Content}";
                                }
                                else
                                {
                                    personaDetailViewModel.ItemDetailItems.Add(BuildPara2ContentRow(paraContent));
                                }
                            }
                            //Para3 here
                            foreach (var para3Grp in paraContent.Para3Containers)
                            {
                                if (para3Grp.Header.HasValue() == true)
                                {
                                    //Render Para3 header
                                    personaDetailViewModel.ItemDetailItems.Add(BuildHeader3Row(para3Grp));
                                }

                                foreach (var para3Content in para3Grp.Para3s)
                                {
                                    foreach (var img in para3Content.PicLinks)
                                    {
                                        personaDetailViewModel.ItemDetailItems.Add(BuildImageRow(img));
                                    }

                                    if (personaDetailViewModel.ItemDetailItems.Any() && personaDetailViewModel.ItemDetailItems.Last()?.Type == "Header3Text")
                                    {
                                        personaDetailViewModel.ItemDetailItems.Last().Content = $"{personaDetailViewModel.ItemDetailItems.Last().Content}{Environment.NewLine}{para3Content.Content}";
                                    }
                                    else
                                    {
                                        personaDetailViewModel.ItemDetailItems.Add(new WikiExtractor.Maui.App.ViewModels.ItemDetailListViewModel
                                        {
                                            Type = "Header3Text",
                                            Content = para3Content.Content ?? "",
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
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
            // try
            // {
            //     if (sender is Border border && border.BindingContext is PictureViewModel pic)
            //     {
            //         personaDetailViewModel.PopupImage = pic;
            //         popupImageDisplay.IsOpen = true;
            //     }
            // }
            // catch (Exception ex)
            // {
            //     CaptureErrorOnPage(ex);
            // }
        }

        private void btnCloseOnPopup_Clicked(object sender, EventArgs e)
        {
            // try
            // {
            //     popupImageDisplay.IsOpen = false;
            // }
            // catch (Exception ex)
            // {
            //     CaptureErrorOnPage(ex);
            // }
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