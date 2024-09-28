using GeneralInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WikiExtractor.XamarinForms.Exts;
using WikiExtractor.XamarinForms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WikiExtractor.XamarinForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizPage : ContentPage
    {
        private readonly QuizPageViewModel _viewModel;

        public QuizPage()
        {
            InitializeComponent();

            _viewModel = new QuizPageViewModel(SummaryPopup);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            try
            {
                _viewModel.Initialize();
                ViewHelper.RunOnAppDispatcher(InitializeAdsControls);
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
            finally
            {
                _viewModel.IsBusy = false;
            }
        }

        

        private async void answer1EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            _viewModel.OnAnswerClick(1);
        }

        private async void answer2EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            _viewModel.OnAnswerClick(2);
        }

        private async void answer3EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            _viewModel.OnAnswerClick(3);
        }

        private async void answer4EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            _viewModel.OnAnswerClick(4);
        }

        private async void lblNext_OnAnimationCompleted(object sender, EventArgs e)
        {
            await _viewModel.OnNextClick(busyIndicator);
        }

        #region Ads
        private void InitializeAdsControls()
        {
            try
            {
                if (AdsHelper.IsAdsServiceAvailable)
                {
                    if (StackBannerAds.Children.Count == 0)
                    {
                        var adsBanner = AdsHelper.BuildAdsBanner();
                        if (adsBanner != null)
                        {
                            StackBannerAds.Children.Add(adsBanner);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.CaptureException(ex);
            }
        }

        #endregion
    }
}