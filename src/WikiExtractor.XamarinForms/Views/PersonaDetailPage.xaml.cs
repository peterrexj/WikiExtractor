using GeneralInformation.Exts;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using MarcTron.Plugin.Controls;
using Pj.Library;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WikiExtractor.Exts;
using WikiExtractor.ViewModels;
using WikiExtractor.XamarinForms.Controls;
using WikiExtractor.XamarinForms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeneralInformation.Views
{
    [QueryProperty(nameof(MasterId), nameof(MasterId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonaDetailPage : ContentPage
    {
        public string MasterId { get; set; }

        private PersonaDetailViewModel personaDetailViewModel;
        private ConcurrentDictionary<string, ExtendedImage> extendImagesInPage = new();

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
                App.Current.Dispatcher.BeginInvokeOnMainThread(() =>
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
        }
        private void LoadWithPageBinding()
        {
            int.TryParse(MasterId, out var result);

            personaDetailViewModel ??= new PersonaDetailViewModel();

            personaDetailViewModel.IsBusy = true;

            personaDetailViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();

            LoadSubPageItemDataDetails();
            BindingContext = personaDetailViewModel;

            //if (Device.RuntimePlatform == Device.UWP)
            //{
            //var task = Task.Run(LoadSubPageItemDataDetails);
            //Task.WhenAll(task).ContinueWith(t =>
            //{
            //    BindingContext = personaDetailViewModel;
            //});
            //}
            //else
            //{
            //    BindingContext = personaDetailViewModel;
            //    Task.Run(LoadSubPageItemDataDetails);
            //}
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
                                            DownloadRequired = toDownload
                                        };

                    if (requiredItems.Any(f => f.DownloadRequired))
                    {
                        //Only select download those images which are rendered in the Details tab. The curasel uses image rather the extendedImage
                        var tasks = requiredItems.Where(f => f.DownloadRequired).Select(async item =>
                        {
                            await CacheImageDownloadHelper.DownloadImage(item.LocalFilePath, item.PicturePath);
                        });

                        Task.WhenAll(tasks).ContinueWith(t =>
                        {
                            if (extendImagesInPage != null)
                            {
                                foreach (var exImgCtrl in extendImagesInPage)
                                {
                                    try
                                    {
                                        //RunOnAppDispatcher(() =>
                                        //{
                                        var currentSource = exImgCtrl.Value.CustomSource;
                                        exImgCtrl.Value.Source = null;
                                        exImgCtrl.Value.Source = currentSource;
                                        //});
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
                    if (personaDetailViewModel?.Persona == null)
                    {
                        personaDetailViewModel.Persona = SharedServices.WikiAppController.GetViewModelById(MasterId.ToInteger());
                    }

                    imgPrimary.CustomSource = Path.Combine(ConfigData.LocalStorageCacheFolderPath, personaDetailViewModel.Persona.PicturePrimaryLocalFileName);

                    personaDetailViewModel.Persona.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;

                    if (personaDetailViewModel.IsMetaDataAvailable == false)
                    {
                        personaDetailViewModel.Persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = personaDetailViewModel.Persona.Name });
                    }

                    BuildDetailItemModel();
                    //personaDetailViewModel.TriggerEvents();
                    //RunOnAppDispatcher(() => LoadImagesRequiredForThisPageAsync());
                    //RunOnAppDispatcher(() => tabView.SelectedIndex = 0);
                    //RunOnAppDispatcher(InitializeAdsControls);

                    LoadImagesRequiredForThisPageAsync();
                    tabView.SelectedIndex = 0;
                    InitializeAdsControls();
                    personaDetailViewModel.TriggerEvents();
                }
                catch (Exception ex)
                {
                    CaptureErrorOnPage(ex);
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
                    //if (paraContents.Any(f => f.ContainsHeader2Content) || paraContents.Any(f => f.ContainsHeader3Content))
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
                                        personaDetailViewModel.ItemDetailItems.Add(new WikiExtractor.XamarinForms.ViewModels.ItemDetailListViewModel
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
                //tabView.SelectedIndex = personaDetailViewModel.IsMetaDataAvailable ? 1 : 0;
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
                    tabView.SelectedIndex = personaDetailViewModel.IsMetaDataAvailable && personaDetailViewModel.IsPicturesAvailable ? 2 :
                                        personaDetailViewModel.IsMetaDataAvailable == false && personaDetailViewModel.IsPicturesAvailable == false ? 0 : 1;
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

        private async void swtReadItem_StateChanged(object sender, Syncfusion.XForms.Buttons.SwitchStateChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    SharedServices.WikiAppController.UpdateItemRead(SharedServices.PageDataTransferModel.Name, e.NewValue ?? false);
                    SharedServices.PageDataTransferModel.IsMarkedAsViewed = e.NewValue ?? false;
                }
                catch (Exception ex)
                {
                    CaptureErrorOnPage(ex);
                }
            });
        }

        private void lstImageEffectsLayer_AnimationCompleted(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is SfEffectsView && (sender as SfEffectsView).AutomationId.HasValue())
                    {
                        var pic = personaDetailViewModel.Persona.Pictures.FirstOrDefault(p => p.Id == (sender as SfEffectsView).AutomationId.ToInteger());
                        if (pic != null)
                        {
                            personaDetailViewModel.PopupImage = pic;

                            popupImageDisplay.PopupView.IsFullScreen = true;
                            popupImageDisplay.ClosePopupOnBackButtonPressed = true;

                            popupImageDisplay.Show(true);
                        }
                    }
                }
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
                popupImageDisplay.Dismiss();
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
                if (ConfigData.DisplayAds && (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS))
                {
                    if (stackBannerAds.Children.Count == 0)
                    {
                        MTAdView ads = new MTAdView
                        {
                            AdsId = personaDetailViewModel.AdsBannerId,
                            HeightRequest = 50
                        };
                        stackBannerAds.Children.Add(ads);
                    }

                    //if (stackBannerAdsOnPopup.Children.Count == 0)
                    //{
                    //    MTAdView ads = new MTAdView();
                    //    ads.AdsId = personaDetailViewModel.AdsBannerId;
                    //    ads.HeightRequest = 50;
                    //    stackBannerAds.Children.Add(ads);
                    //}
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }
    }
}