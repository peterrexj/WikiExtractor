using GeneralInformation.Exts;
using GeneralInformation.Services;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.SfCarousel.XForms;
using Syncfusion.XForms.Border;
using Syncfusion.XForms.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestAny.Essentials.Api;
using WikiExtractor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeneralInformation.Views
{
    [QueryProperty(nameof(MasterId), nameof(MasterId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonaDetailPage : ContentPage
    {
        public string MasterId { get; set; }

        private List<Grid> _paraGrids;
        private const int DefaultHeightImageInDetailsPage = 300;
        ConcurrentDictionary<string, ExtendedImage> extendImageCtrlsInPage = new();

        private PersonaDetailViewModel personaDetailViewModel;

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
                if (Device.RuntimePlatform != Device.UWP)
                {
                    carousel.SetBinding(SfCarousel.ItemHeightProperty, new Binding("Height", source: rowImageInContainerGrid));
                    carousel.SetBinding(SfCarousel.LoadMoreItemsCountProperty, "CarouselImageLoadMoreItemsCount");
                }
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

        private void LoadWithPageBinding()
        {
            int.TryParse(MasterId, out var result);

            personaDetailViewModel ??= new PersonaDetailViewModel();

            personaDetailViewModel.IsBusy = true;

            personaDetailViewModel.DefaultStyle = ThemeHelper.GetDefaultStyle();

            if (Device.RuntimePlatform == Device.UWP)
            {
                Task.Run(LoadSubPageItemDataDetails);
                BindingContext = personaDetailViewModel;
            }
            else
            {
                BindingContext = personaDetailViewModel;
                Task.Run(LoadSubPageItemDataDetails);
            }
        }
        private void LoadImagesRequiredForThisPageAsync()
        {
            try
            {
                if (personaDetailViewModel.Persona.Pictures.Any())
                {
                    //Only select download those images which are rendered in the Details tab. The curasel uses image rather the extendedImage
                    var tasks = personaDetailViewModel.Persona.Pictures.Select(async item =>
                    {
                        var _localFileName = Path.Combine(ExtendedImage.CacheFolder, item.PictureLocalFileName);
                        await CacheImageDownloadHelper.DownloadImage(_localFileName, item.PicturePath);
                    });
                    Task.WhenAll(tasks).ContinueWith(t =>
                    {
                        foreach (var exImgCtrl in extendImageCtrlsInPage)
                        {
                            try
                            {
                                RunOnAppDispatcher(() =>
                                {
                                    var currentSource = exImgCtrl.Value.CustomSource;
                                    exImgCtrl.Value.Source = null;
                                    exImgCtrl.Value.Source = currentSource;
                                });
                            }
                            catch (Exception ex)
                            {
                                CaptureErrorOnPage(ex);
                            }
                        }
                        extendImageCtrlsInPage = null;
                    });
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

                    imgPrimary.CustomSource = Path.Combine(ExtendedImage.CacheFolder, personaDetailViewModel.Persona.PicturePrimaryLocalFileName);

                    personaDetailViewModel.Persona.ItemReadStatus = SharedServices.PageDataTransferModel.IsMarkedAsViewed;

                    if (personaDetailViewModel.IsMetaDataAvailable == false)
                    {
                        personaDetailViewModel.Persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = personaDetailViewModel.Persona.Name });
                    }

                    if (personaDetailViewModel.IsPicturesAvailable)
                    {
                        personaDetailViewModel.CurrentSelectedPictureCaption = personaDetailViewModel.Persona.Pictures.FirstOrDefault().PictureCaption;
                    }

                    personaDetailViewModel.CarouselImageCurrentClickIndex = 0;
                    if (personaDetailViewModel.Persona.Pictures.Count > personaDetailViewModel.CarouselImageLoadMoreItemsCount)
                    {
                        personaDetailViewModel.CarouselImageTotalClicksToLoadComplete = personaDetailViewModel.Persona.Pictures.Count / personaDetailViewModel.CarouselImageLoadMoreItemsCount + 1;
                    }
                    else
                    {
                        personaDetailViewModel.CarouselImageTotalClicksToLoadComplete = 0;
                        personaDetailViewModel.CarouselImageLoadComplete = true;
                    }

                    LoadParaDetails();
                    personaDetailViewModel.TriggerEvents();
                    //RunOnAppDispatcher(() => tabView.VisibleHeaderCount = personaDetailViewModel.AvailableTabCount);
                    tabView.SelectedIndex = 0;
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
        public void LoadParaGrids()
        {
            _paraGrids = new List<Grid>();

            foreach (var grpContent in personaDetailViewModel?.Persona?.Paragraphs.OrderBy(f => f.Sequence).GroupBy(f => f.Header2))
            {
                try
                {
                    _paraGrids.Add(RenderPara2ContentV2(grpContent.ToList()));
                }
                catch (Exception ex)
                {
                    CaptureErrorOnPage(ex);
                }
            }
            LoadImagesRequiredForThisPageAsync();
        }
        private void LoadParaDetails()
        {
            try
            {
                LoadParaGrids();
                RenderParaContents();
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }
        private void RenderParaContents()
        {
            RunOnAppDispatcher(() =>
            {
                foreach (var grpContent in _paraGrids)
                {
                    try
                    {
                        ParaContentsStack.Children.Add(grpContent);
                    }
                    catch (Exception ex)
                    {
                        CaptureErrorOnPage(ex);
                    }
                }
            });
        }
        private Grid RenderPara2ContentV2(List<Paragraph2ContentViewModel> paraContents)
        {
            var mainGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var sfGradient = new SfGradientView();

            var grStart = new SfGradientStop();
            grStart.Offset = 0.0;
            grStart.SetBinding(SfGradientStop.ColorProperty, "DefaultStyle.SubPageDetailsContentGradientStartColor", converter: SharedServices.ToColorConverterAsValueConverter);

            var grStop = new SfGradientStop();
            grStop.Offset = 0.8;
            grStart.SetBinding(SfGradientStop.ColorProperty, "DefaultStyle.SubPageDetailsContentGradientEndColor", converter: SharedServices.ToColorConverterAsValueConverter);

            sfGradient.BackgroundBrush = new SfLinearGradientBrush
            {
                GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection() { grStart, grStop }
            };

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 8, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            if (paraContents.Any(f => f.Content.HasValue()) || paraContents.Any(f => f.Para3s.Any(g => g.Content.HasValue())))
            {
                stackLayout.Children.Add(RenderDynamicContentLabel(paraContents.Where(f => f.Header2.HasValue()).FirstOrDefault().Header2 ?? "", "DetailsTabHeaderText", "DefaultStyle.DefaultFontFamilyBold"));
                foreach (var paraContent in paraContents)
                {
                    if (paraContent.Content.HasValue())
                    {
                        foreach (var img in paraContent.PicLinks)
                        {
                            stackLayout.Children.Add(RenderParagraphContentImage(img));
                        }
                        stackLayout.Children.Add(RenderDynamicContentLabel(paraContent.Content, "DetailsTabContentText", "DefaultStyle.DefaultFontFamily"));
                    }

                    //Para3 here
                    if (paraContent.Para3s != null && paraContent.Para3s.Any())
                    {
                        foreach (var para3Grp in paraContent.Para3s.OrderBy(f => f.Sequence).GroupBy(f => f.Header3))
                        {
                            stackLayout.Children.Add(RenderDynamicContentLabel(para3Grp.Where(f => f.Header3.HasValue()).FirstOrDefault().Header3 ?? "", "DetailsTabSubHeaderText", "DefaultStyle.DefaultFontFamilyBold"));

                            foreach (var para3Content in para3Grp)
                            {
                                foreach (var img in para3Content.PicLinks)
                                {
                                    stackLayout.Children.Add(RenderParagraphContentImage(img));
                                }
                                stackLayout.Children.Add(RenderDynamicContentLabel(para3Content.Content, "DetailsTabContentText", "DefaultStyle.DefaultFontFamily"));
                            }
                        }
                    }
                }
            }
            mainGrid.Children.Add(sfGradient);
            mainGrid.Children.Add(stackLayout);
            return mainGrid;
        }

        private Label RenderDynamicContentLabel(string content, string style, string bindingPathToFontFamily)
        {
            var lbl = new Label { Text = content };
            lbl.SetBinding(Label.FontFamilyProperty, bindingPathToFontFamily);
            lbl.SetBinding(Label.TextColorProperty, "DefaultStyle.DefaultFontColor", converter: SharedServices.ToColorConverterAsValueConverter);
            var resource = Application.Current.Resources[style];
            if (resource != null && resource.GetType() == typeof(Style))
                lbl.Style = (Style)resource;
            return lbl;
        }
        private Grid RenderParagraphContentImage(PictureViewModel picModel)
        {
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) },
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            try
            {
                var sfBorder = new SfBorder
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.LightGray,
                    BorderWidth = 1,
                    CornerRadius = 5,
                    HeightRequest = (picModel.Height <= 0 || picModel.Height > DefaultHeightImageInDetailsPage) ? DefaultHeightImageInDetailsPage : picModel.Height,
                    AutomationId = $"{(picModel.Height <= 0 ? DefaultHeightImageInDetailsPage : picModel.Height)},{(picModel.Width <= 0 ? DefaultHeightImageInDetailsPage : picModel.Width)}"
                };

                sfBorder.SizeChanged += SfBorderOnContentDetailImage_SizeChanged;
                var img = new ExtendedImage
                {
                    CustomSource = Path.Combine(ExtendedImage.CacheFolder, picModel.PictureLocalFileName),
                    LocalFileName = picModel.PictureLocalFileName,
                    Margin = new Thickness(5, 2, 5, 2),
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                RunOnAppDispatcher(() =>
                {
                    if (extendImageCtrlsInPage != null) //This can be null when all images are loaded or nothing to load
                    {
                        extendImageCtrlsInPage.AddOrUpdate(img.LocalFileName, img);
                    }
                });

                sfBorder.Content = img;

                var lbl = new Label { Text = picModel.PictureCaption };
                lbl.SetBinding(Label.FontFamilyProperty, "DefaultStyle.DefaultFontFamilyBold");
                lbl.SetBinding(Label.TextColorProperty, "DefaultStyle.DefaultFontColor", converter: SharedServices.ToColorConverterAsValueConverter);

                var resource = Application.Current.Resources["DetailsTabImageCaptionText"];
                if (resource != null && resource.GetType() == typeof(Style))
                    lbl.Style = (Style)resource;

                Grid.SetRow(sfBorder, 0);
                Grid.SetRow(lbl, 1);

                grid.Children.Add(sfBorder);
                grid.Children.Add(lbl);

            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
            return grid;
        }

        private void SfBorderOnContentDetailImage_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    var width = ((SfBorder)sender).Bounds.Width;
                    if (width > 600) width = 600; //For Tablet with higher width, the width is set back to 600
                    var automationId = ((SfBorder)sender).AutomationId?.SplitAndTrim(",")?.ToList();
                    if (width > 0 && automationId?.Count() == 2)
                    {
                        //item 0 - height
                        //item 1 - width

                        var actualHeight = (automationId[0].ToDouble() / automationId[1].ToDouble()) * width;
                        ((SfBorder)sender).HeightRequest = actualHeight;
                        //For tablets, since the width is shortend, the picture will sit in the centre with gaps around the border.
                        //hence removing the border and radius
                        if (width >= 600)
                        {
                            ((SfBorder)sender).WidthRequest = width;
                            ((SfBorder)sender).BorderColor = Color.Transparent;
                            ((SfBorder)sender).BorderWidth = 0;
                            ((SfBorder)sender).CornerRadius = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }
        private void carousel_SelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                if (e != null && (e.SelectedItem != null || e.SelectedIndex >= 0))
                {
                    if (e.SelectedItem != null)
                    {
                        personaDetailViewModel.CurrentSelectedPictureCaption = (e.SelectedItem as PictureViewModel).PictureCaption;
                    }
                    else
                    {
                        personaDetailViewModel.CurrentSelectedPictureCaption = personaDetailViewModel.Persona.Pictures[e.SelectedIndex]?.PictureCaption ?? string.Empty;
                    }
                    if (personaDetailViewModel.CarouselImageLoadComplete == false)
                    {
                        if (personaDetailViewModel.CarouselImageCurrentClickIndex < personaDetailViewModel.CarouselImageTotalClicksToLoadComplete)
                        {
                            personaDetailViewModel.CarouselImageCurrentClickIndex++;
                            carousel.LoadMore();
                        }
                        else
                        {
                            personaDetailViewModel.CarouselImageLoadComplete = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CaptureErrorOnPage(ex);
            }
        }

        private void First_TabItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                tabView.SelectedIndex = 0;
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
                tabView.SelectedIndex = personaDetailViewModel.IsMetaDataAvailable ? 1 : 0;
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
                tabView.SelectedIndex = personaDetailViewModel.IsMetaDataAvailable && personaDetailViewModel.IsPicturesAvailable ? 2 :
                                    personaDetailViewModel.IsMetaDataAvailable == false && personaDetailViewModel.IsPicturesAvailable == false ? 0 : 1;
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
    }
}