using GeneralInformation.Repository;
using GeneralInformation.ViewModels;
using Pj.Library;
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

        protected override void OnAppearing()
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
    }
}