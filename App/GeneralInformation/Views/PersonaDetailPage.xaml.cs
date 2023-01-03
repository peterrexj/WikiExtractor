using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using Syncfusion.SfCarousel.XForms;
using Syncfusion.XForms.Border;
using Syncfusion.XForms.Expander;
using Syncfusion.XForms.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

                foreach (var para in personaDetailViewModel.Persona.Paragraphs)
                {
                    ParaContentsStack.Children.Add(RenderPara2ContentV2(para));
                }
                await ApplyTabSelectionChangeEvent();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private Grid RenderPara2ContentV2(Paragraph2ContentViewModel paraContent)
        {
            var mainGrid = new Grid();
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
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            if (paraContent.Content.HasValue())
            {
                stackLayout.Children.Add(RenderDynamicContentLabel(paraContent.Header2, "DetailsTabHeaderText"));
                stackLayout.Children.Add(RenderDynamicContentLabel(paraContent.Content, "DetailsTabContentText"));
            }

            if (paraContent.Para3s != null && paraContent.Para3s.Any())
            {
                foreach (var p3 in paraContent.Para3s)
                {
                    stackLayout.Children.Add(RenderDynamicContentLabel(p3.Header3, "DetailsTabSubHeaderText"));
                    stackLayout.Children.Add(RenderDynamicContentLabel(p3.Content, "DetailsTabContentText"));
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
                else if (tIndex == 101) data = "Pictures";
                else if (tIndex == 102) data = "Details";
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
                            else if (context == "Pictures")
                            {
                                selectedIndex = personaDetailViewModel.IsMetaDataAvailable ? 1 : 0;
                            }
                            else if (context == "Details")
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
                            applyStyle(btnTabPicture);
                            removeStyle(btnTabDetails);
                        }
                        else if (personaDetailViewModel.SelectedTabIndex == 2)
                        {
                            removeStyle(btnTabBasicInfo);
                            removeStyle(btnTabPicture);
                            applyStyle(btnTabDetails);
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