using GeneralInformation.Exts;
using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.XForms.Border;
using Syncfusion.XForms.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiExtractor.Process;
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
        private readonly WikiAppController wikiAppController;
        private PersonaDetailViewModel personaDetailViewModel;
        private const int DefaultHeightImageInDetailsPage = 300;

        private IDictionary<string, string> BuildErrorContext()
        {
            if (personaDetailViewModel != null &&
                personaDetailViewModel.Persona != null &&
                personaDetailViewModel.Persona.Name.HasValue())
            {
                return DeviceDetails.GenerateMetaInformation(new Dictionary<string, string>
                {
                    { "Name", personaDetailViewModel.Persona.Name },
                    { "WikiPath", personaDetailViewModel.Persona.WikiPath },
                });
            }
            else
            {

                return DeviceDetails.GenerateDeviceInformation();
            }
        }

        public PersonaDetailPage()
        {
            try
            {
                InitializeComponent();
                wikiAppController = new WikiAppController(DatabaseService.AppDatabase);
                BindingContext = personaDetailViewModel = new PersonaDetailViewModel { Persona = new PersonaViewModel() };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                int.TryParse(MasterId, out var result);
                personaDetailViewModel.Persona = wikiAppController.GetViewModelById(result);
                if (personaDetailViewModel.IsMetaDataAvailable == false)
                {
                    personaDetailViewModel.Persona.Metadatas.Add(new MetadataViewModel { Key = "", Description = personaDetailViewModel.Persona.Name });
                }

                tabView.VisibleHeaderCount = personaDetailViewModel.AvailableTabCount;
                if (personaDetailViewModel.IsPicturesAvailable)
                {
                    personaDetailViewModel.CurrentSelectedPictureCaption = personaDetailViewModel.Persona.Pictures.FirstOrDefault().PictureCaption;
                }
                var taskGroup = new TaskGroup();
                taskGroup.Add(() =>
                    RunOnAppDispatcher(() =>
                    {
                        RenderParaContents(personaDetailViewModel.Persona.Paragraphs);
                    })
                );
                taskGroup.Add(() => ApplyTabSelectionChangeEvent());
                taskGroup.WaitAll();

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
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, BuildErrorContext());
            }
        }

        private void RenderParaContents(List<Paragraph2ContentViewModel> contents)
        {
            foreach (var grpContent in contents.OrderBy(f => f.Sequence).GroupBy(f => f.Header2))
            {
                try
                {
                    ParaContentsStack.Children.Add(RenderPara2ContentV2(grpContent.ToList()));
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, BuildErrorContext());
                }
            }
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
            grStart.SetAppThemeColor(SfGradientStop.ColorProperty,
                (Color)App.Current.Resources["DetailContentBackgroundColorGradientStartLight"],
                (Color)App.Current.Resources["DetailContentBackgroundColorGradientStartDark"]);

            var grStop = new SfGradientStop();
            grStop.Offset = 0.8;
            grStop.SetAppThemeColor(SfGradientStop.ColorProperty,
                (Color)App.Current.Resources["DetailContentBackgroundColorGradientEndLight"],
                (Color)App.Current.Resources["DetailContentBackgroundColorGradientEndDark"]);

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
                stackLayout.Children.Add(RenderDynamicContentLabel(paraContents.Where(f => f.Header2.HasValue()).FirstOrDefault().Header2 ?? "", "DetailsTabHeaderText"));
                foreach (var paraContent in paraContents)
                {
                    if (paraContent.Content.HasValue())
                    {
                        foreach (var img in paraContent.PicLinks)
                        {
                            stackLayout.Children.Add(RenderParagraphContentImage(img));
                        }
                        stackLayout.Children.Add(RenderDynamicContentLabel(paraContent.Content, "DetailsTabContentText"));
                    }

                    //Para3 here
                    if (paraContent.Para3s != null && paraContent.Para3s.Any())
                    {
                        foreach (var para3Grp in paraContent.Para3s.OrderBy(f => f.Sequence).GroupBy(f => f.Header3))
                        {
                            stackLayout.Children.Add(RenderDynamicContentLabel(para3Grp.Where(f => f.Header3.HasValue()).FirstOrDefault().Header3 ?? "", "DetailsTabSubHeaderText"));

                            foreach (var para3Content in para3Grp)
                            {
                                foreach (var img in para3Content.PicLinks)
                                {
                                    stackLayout.Children.Add(RenderParagraphContentImage(img));
                                }
                                stackLayout.Children.Add(RenderDynamicContentLabel(para3Content.Content, "DetailsTabContentText"));
                            }
                        }
                    }
                }
            }
            mainGrid.Children.Add(sfGradient);
            mainGrid.Children.Add(stackLayout);
            return mainGrid;
        }

        private Label RenderDynamicContentLabel(string content, string style)
        {
            var lbl = new Label { Text = content };
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
                var img = new Image
                {
                    Source = picModel.PicturePath,
                    Margin = new Thickness(5, 2, 5, 2),
                    Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                sfBorder.Content = img;

                var lbl = new Label { Text = picModel.PictureCaption };
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
                Crashes.TrackError(ex, BuildErrorContext());
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
                Crashes.TrackError(ex, BuildErrorContext());
            }
        }

        //private Label RenderHeaderLabel(string content)
        //{
        //    var lbl = new Label
        //    {
        //        Text = content,
        //        //TextColor = Color.Black,
        //        //LineBreakMode = LineBreakMode.WordWrap,
        //        //FontAttributes = FontAttributes.Bold,
        //        //FontSize = fontSize
        //    };
        //    //lbl.SetAppThemeColor(Label.TextColorProperty,
        //    //    (Color)App.Current.Resources["DetailContentHeaderTextColorLight"],
        //    //    (Color)App.Current.Resources["DetailContentHeaderTextColorDark"]);
        //    return lbl;
        //}
        //private Label RenderContentLabel(string content)
        //{
        //    var lbl = new Label
        //    {
        //        Text = content,
        //        //TextColor = Color.Black,
        //        //LineBreakMode = LineBreakMode.WordWrap,
        //        //FontSize = fontSize,
        //        //Padding = new Thickness(0, 0, 0, 4),
        //        //CharacterSpacing = 0.5,
        //    };

        //    var resource = Application.Current.Resources["DetailsTabContentText"];
        //    if (resource != null && resource.GetType() == typeof(Style))
        //        lbl.Style = (Style)resource;

        //    //lbl.SetAppThemeColor(Label.TextColorProperty,
        //    //    (Color)App.Current.Resources["DetailContentBodyTextColorLight"],
        //    //    (Color)App.Current.Resources["DetailContentBodyTextColorDark"]);
        //    return lbl;
        //}
        //private int FontSizeHeaderBasedOnDevice() => Device.RuntimePlatform switch
        //    {
        //        Device.Android => Device.Idiom switch
        //        {
        //            TargetIdiom.Phone => 20,
        //            TargetIdiom.Tablet => 26,
        //            _ => 20,
        //        },
        //        Device.iOS => Device.Idiom switch
        //        {
        //            TargetIdiom.Phone => 20,
        //            TargetIdiom.Tablet => 26,
        //            _ => 20,
        //        },
        //        _ => 20
        //    };

        //private int FontSizeSubHeaderBasedOnDevice() => Device.RuntimePlatform switch
        //{
        //    Device.Android => Device.Idiom switch
        //    {
        //        TargetIdiom.Phone => 16,
        //        TargetIdiom.Tablet => 22,
        //        _ => 16,
        //    },
        //    Device.iOS => Device.Idiom switch
        //    {
        //        TargetIdiom.Phone => 16,
        //        TargetIdiom.Tablet => 22,
        //        _ => 16,
        //    },
        //    _ => 16
        //};
        //private int FontSizeContentBasedOnDevice() => Device.RuntimePlatform switch
        //{
        //    Device.Android => Device.Idiom switch
        //    {
        //        TargetIdiom.Phone => 14,
        //        TargetIdiom.Tablet => 17,
        //        _ => 14,
        //    },
        //    Device.iOS => Device.Idiom switch
        //    {
        //        TargetIdiom.Phone => 14,
        //        TargetIdiom.Tablet => 17,
        //        _ => 14,
        //    },
        //    _ => 14
        //};




        //private SfExpander RenderPara2Content(Paragraph2ContentViewModel paraContent)
        //{
        //    Color bgColor = Color.FromHex("#D1DBE1");
        //    var txtColor = Color.FromHex("#495F6E");

        //    var mainExpander = new SfExpander
        //    {
        //        HeaderIconPosition = IconPosition.End,
        //        BackgroundColor = bgColor,
        //        HeaderBackgroundColor = bgColor,
        //        AnimationDuration = 10,
        //        MinimumHeightRequest = 100,
        //        IsExpanded = true
        //    };

        //    var headerFrame = new Frame
        //    {
        //        Padding = new Thickness(4)
        //    };

        //    var headerLabel = new Label
        //    {
        //        TextColor = txtColor,
        //        BackgroundColor = bgColor,
        //        Text = paraContent.Header2,
        //        FontSize = 16,
        //        HorizontalTextAlignment = TextAlignment.Start,
        //        VerticalOptions = LayoutOptions.Center,
        //        Padding = new Thickness(10, 3, 0, 3)
        //    };

        //    headerFrame.Content = headerLabel;
        //    mainExpander.Header = headerFrame;

        //    //Content
        //    var lblMainContentFormatString = new FormattedString();

        //    if (paraContent.Content.HasValue())
        //    {
        //        lblMainContentFormatString.Spans.Add(new Span
        //        {
        //            FontSize = 13,
        //            Text = paraContent.Content
        //        });
        //    }

        //    if (paraContent.Para3s != null && paraContent.Para3s.Any())
        //    {
        //        foreach (var p3 in paraContent.Para3s)
        //        {
        //            lblMainContentFormatString.Spans.Add(new Span { Text = Environment.NewLine });
        //            lblMainContentFormatString.Spans.Add(new Span
        //            {
        //                FontSize = 15,
        //                FontAttributes = FontAttributes.Bold,
        //                Text = p3.Header3
        //            });
        //            lblMainContentFormatString.Spans.Add(new Span { Text = Environment.NewLine });
        //            lblMainContentFormatString.Spans.Add(new Span
        //            {
        //                FontSize = 13,
        //                Text = p3.Content
        //            });
        //        }
        //    }

        //    var lblMainContent = new Label();
        //    lblMainContent.TextColor = txtColor;
        //    lblMainContent.Padding = new Thickness(10, 3, 0, 3);
        //    lblMainContent.LineHeight = 1.3;
        //    lblMainContent.FontSize = 15;
        //    lblMainContent.VerticalOptions = LayoutOptions.CenterAndExpand;
        //    lblMainContent.FormattedText = lblMainContentFormatString;

        //    var boxView = new BoxView
        //    {
        //        Color = bgColor,
        //        CornerRadius = 2,
        //    };

        //    var contentGrid = new Grid();
        //    contentGrid.BackgroundColor = bgColor;
        //    contentGrid.Children.Add(boxView);
        //    contentGrid.Children.Add(lblMainContent);

        //    mainExpander.Content = contentGrid;



        //    return mainExpander;
        //}



        private void carousel_SelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                if (e != null && e.SelectedItem != null)
                {
                    personaDetailViewModel.CurrentSelectedPictureCaption = (e.SelectedItem as PictureViewModel).PictureCaption;
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
                Crashes.TrackError(ex);
            }
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
                Crashes.TrackError(ex);
            }
        }

        private async void btnTabBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                personaDetailViewModel.IsBusy = true;
                var tIndex = ((Syncfusion.XForms.Buttons.SfButton)sender).TabIndex;
                var data = string.Empty;
                if (tIndex == 100) data = "BasicInfo";
                else if (tIndex == 101) data = "Details";
                else if (tIndex == 102) data = "Pictures";
                await ApplyTabSelectionChangeEvent(context: data);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                personaDetailViewModel.IsBusy = false;
            }
        }

        async Task ApplyTabSelectionChangeEvent(int selectedIndex = -1, string context = "")
        {
            await Task.Run(() =>
            {
                RunOnAppDispatcher(() =>
                {
                    try
                    {
                        if (selectedIndex == -1 && context.HasValue())
                        {
                            if (context == "BasicInfo")
                            {
                                selectedIndex = 0;
                            }
                            else if (context == "Details")
                            {
                                selectedIndex = personaDetailViewModel.IsMetaDataAvailable ? 1 : 0;
                            }
                            else if (context == "Pictures")
                            {
                                selectedIndex = personaDetailViewModel.IsMetaDataAvailable && personaDetailViewModel.IsPicturesAvailable ? 2 :
                                    personaDetailViewModel.IsMetaDataAvailable == false && personaDetailViewModel.IsPicturesAvailable == false ? 0 : 1;
                            }
                            else selectedIndex = 0;

                            tabView.SelectedIndex = selectedIndex;
                            personaDetailViewModel.SelectedTabIndex = selectedIndex;
                        }
                        else if (personaDetailViewModel.SelectedTabIndex == -1)
                        {
                            personaDetailViewModel.SelectedTabIndex = 0;
                        }
                        else if (personaDetailViewModel.SelectedTabIndex != selectedIndex && selectedIndex != -1)
                        {
                            tabView.SelectedIndex = selectedIndex;
                            personaDetailViewModel.SelectedTabIndex = selectedIndex;
                        }

                        var applyBackColor = Application.Current.UserAppTheme == OSAppTheme.Light ?
                            (Color)Application.Current.Resources["TabBackColorApplyLight"] :
                            (Color)Application.Current.Resources["TabBackColorApplyDark"];
                        var removeBackColor = Application.Current.UserAppTheme == OSAppTheme.Light ?
                            (Color)Application.Current.Resources["TabBackColorApplyDark"] :
                            (Color)Application.Current.Resources["TabBackColorApplyLight"];
                        var applyTextColor = Application.Current.UserAppTheme == OSAppTheme.Light ?
                            (Color)Application.Current.Resources["TabBackColorApplyDark"] :
                            (Color)Application.Current.Resources["TabBackColorApplyLight"];
                        var removeTextColor = Application.Current.UserAppTheme == OSAppTheme.Light ?
                            (Color)Application.Current.Resources["TabBackColorApplyLight"] :
                            (Color)Application.Current.Resources["TabBackColorApplyDark"];


                        Action<Syncfusion.XForms.Buttons.SfButton> applyStyle = (btn) =>
                        {
                            btn.BackgroundColor = applyBackColor;
                            btn.TextColor = applyTextColor;
                        };

                        Action<Syncfusion.XForms.Buttons.SfButton> removeStyle = (btn) =>
                        {
                            btn.BackgroundColor = removeBackColor;
                            btn.TextColor = removeTextColor;
                        };

                        if (personaDetailViewModel.SelectedTabIndex == 0)
                        {
                            applyStyle(btnTabBasicInfo);
                            removeStyle(btnTabPicture);
                            removeStyle(btnTabDetails);
                        }
                        else if (personaDetailViewModel.SelectedTabIndex == 1)
                        {
                            removeStyle(btnTabBasicInfo);
                            removeStyle(btnTabPicture);
                            applyStyle(btnTabDetails);
                        }
                        else if (personaDetailViewModel.SelectedTabIndex == 2)
                        {
                            removeStyle(btnTabBasicInfo);
                            applyStyle(btnTabPicture);
                            removeStyle(btnTabDetails);
                        }
                    }
                    catch (Exception ex)
                    {
                        Crashes.TrackError(ex);
                    }
                });
            });

        }

        private async void tabView_SelectionChanged(object sender, Syncfusion.XForms.TabView.SelectionChangedEventArgs e)
        {
            try
            {
                personaDetailViewModel.IsBusy = true;
                if (tabView.SelectedIndex != personaDetailViewModel.SelectedTabIndex)
                {
                    await ApplyTabSelectionChangeEvent(selectedIndex: tabView.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                personaDetailViewModel.IsBusy = false;
            }
        }
    }
}