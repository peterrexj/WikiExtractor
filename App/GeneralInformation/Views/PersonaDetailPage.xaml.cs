using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Pj.Library;
using Syncfusion.SfCarousel.XForms;
using Syncfusion.XForms.Border;
using Syncfusion.XForms.Expander;
using Syncfusion.XForms.Graphics;
using System;
using System.Collections.Generic;
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
            InitializeComponent();
            wikiAppController = new WikiAppController(DatabaseService.AppDatabase);
            personaDetailViewModel = new PersonaDetailViewModel();
            BindingContext = personaDetailViewModel = new PersonaDetailViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(MasterId, out var result);
            personaDetailViewModel.Persona = wikiAppController.GetViewModelById(result);
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

        private Grid RenderPara2ContentV2(Paragraph2ContentViewModel paraContent)
        {
            var mainGrid = new Grid();

            var sfGradient = new SfGradientView();
            sfGradient.BackgroundBrush = new SfLinearGradientBrush
            {
                GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection()
                {
                    new SfGradientStop { Color = Color.FromHex("#F2F3F4"), Offset = 0.0 },
                    new SfGradientStop { Color = Color.FromHex("#E5E7E9"), Offset = 1 },
                }
            };

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(10, 0, 8, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            if (paraContent.Content.HasValue())
            {
                var lblMainHeader = new Label
                {
                    Text = paraContent.Header2,
                    TextColor = Color.Black,
                    LineBreakMode = LineBreakMode.WordWrap,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18
                };

                var lblMainContent = new Label
                {
                    Text = paraContent.Content,
                    TextColor = Color.Black,
                    LineBreakMode = LineBreakMode.WordWrap,
                    FontSize = 13,
                    Padding = new Thickness(0, 0, 0, 4),
                };

                stackLayout.Children.Add(lblMainHeader);
                stackLayout.Children.Add(lblMainContent);
            }

            if (paraContent.Para3s != null && paraContent.Para3s.Any())
            {
                foreach (var p3 in paraContent.Para3s)
                {
                    var lblSubHeader = new Label
                    {
                        Text = p3.Header3,
                        TextColor = Color.Black,
                        LineBreakMode = LineBreakMode.WordWrap,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 15
                    };

                    var lblSubContent = new Label
                    {
                        Text = p3.Content,
                        TextColor = Color.Black,
                        LineBreakMode = LineBreakMode.WordWrap,
                        FontSize = 13,
                        Padding = new Thickness(0, 0, 0, 4),
                    };

                    stackLayout.Children.Add(lblSubHeader);
                    stackLayout.Children.Add(lblSubContent);
                }
            }

            mainGrid.Children.Add(sfGradient);
            mainGrid.Children.Add(stackLayout);
            //lblMainContent.TextColor = txtColor;
            //lblMainContent.Padding = new Thickness(10, 3, 0, 3);
            //lblMainContent.LineHeight = 1.3;
            //lblMainContent.FontSize = 15;
            //lblMainContent.VerticalOptions = LayoutOptions.CenterAndExpand;
            //lblMainContent.FormattedText = lblMainContentFormatString;

            //if (paraContent.Content.HasValue())
            //{
            //    lblMainContentFormatString.Spans.Add(new Span
            //    {
            //        FontSize = 13,
            //        Text = paraContent.Content
            //    });
            //}

            return mainGrid;
        }
        private SfExpander RenderPara2Content(Paragraph2ContentViewModel paraContent)
        {
            Color bgColor = Color.FromHex("#D1DBE1");
            var txtColor = Color.FromHex("#495F6E");

            var mainExpander = new SfExpander
            {
                HeaderIconPosition = IconPosition.End,
                BackgroundColor = bgColor,
                HeaderBackgroundColor = bgColor,
                AnimationDuration = 10,
                MinimumHeightRequest = 100,
                IsExpanded = true
            };

            var headerFrame = new Frame
            {
                Padding = new Thickness(4)
            };

            var headerLabel = new Label
            {
                TextColor = txtColor,
                BackgroundColor = bgColor,
                Text = paraContent.Header2,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(10, 3, 0, 3)
            };

            headerFrame.Content = headerLabel;
            mainExpander.Header = headerFrame;

            //Content
            var lblMainContentFormatString = new FormattedString();

            if (paraContent.Content.HasValue())
            {
                lblMainContentFormatString.Spans.Add(new Span
                {
                    FontSize = 13,
                    Text = paraContent.Content
                });
            }

            if (paraContent.Para3s != null && paraContent.Para3s.Any())
            {
                foreach (var p3 in paraContent.Para3s)
                {
                    lblMainContentFormatString.Spans.Add(new Span { Text = Environment.NewLine });
                    lblMainContentFormatString.Spans.Add(new Span
                    {
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                        Text = p3.Header3
                    });
                    lblMainContentFormatString.Spans.Add(new Span { Text = Environment.NewLine });
                    lblMainContentFormatString.Spans.Add(new Span
                    {
                        FontSize = 13,
                        Text = p3.Content
                    });
                }
            }

            var lblMainContent = new Label();
            lblMainContent.TextColor = txtColor;
            lblMainContent.Padding = new Thickness(10, 3, 0, 3);
            lblMainContent.LineHeight = 1.3;
            lblMainContent.FontSize = 15;
            lblMainContent.VerticalOptions = LayoutOptions.CenterAndExpand;
            lblMainContent.FormattedText = lblMainContentFormatString;

            var boxView = new BoxView
            {
                Color = bgColor,
                CornerRadius = 2,
            };

            var contentGrid = new Grid();
            contentGrid.BackgroundColor = bgColor;
            contentGrid.Children.Add(boxView);
            contentGrid.Children.Add(lblMainContent);

            mainExpander.Content = contentGrid;



            return mainExpander;
        }



        private void carousel_SelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
        {
            if (e != null && e.SelectedItem != null)
            {
                personaDetailViewModel.CurrentSelectedPictureCaption = (e.SelectedItem as PictureViewModel).PictureCaption;
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
                            if (context == "BasicInfo") selectedIndex = 0;
                            else if (context == "Pictures") selectedIndex = 1;
                            else if (context == "Details") selectedIndex = 2;
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
                    { }
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

            }
            finally
            {
                personaDetailViewModel.IsBusy = false;
            }
        }
    }
}