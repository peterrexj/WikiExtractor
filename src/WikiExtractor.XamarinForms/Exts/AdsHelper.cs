using GeneralInformation.Repository;
using GeneralInformation.Services;
using MarcTron.Plugin;
using MarcTron.Plugin.Controls;
using Microsoft.AppCenter.Crashes;
using Pj.Library;
using System;
using Xamarin.Forms;

namespace WikiExtractor.XamarinForms.Exts
{
    public static class AdsHelper
    {
        public static bool DisplayAds { get; set; }
        public static string AdsInterstitialId { get; set; }
        public static string AdsBannerId { get; set; }
        public static string AdsQuizBannerId { get; set; }

        public static int AdsInterstitialLimitOnRecord { get; set; }
        private static int AdsInterstitialLimitOnRecordRound2 { get; set; }

        public static void InitializeAds()
        {
            try
            {
                var appInfo = DependencyService.Get<IAppInformation>();
                AdsInterstitialId = appInfo.AdsInterstitialId;
                AdsBannerId = appInfo.AdsBannerId;
                AdsQuizBannerId = appInfo.AdsQuizBannerId;
                DisplayAds = DependencyService.Get<IAppEnvironment>().DisplayAds;

                if (IsAdsServiceAvailable)
                {
                    if (DatabaseService.UserStoreDatabase.RequestRecordRepository.GetCount() == 0)
                    {
                        DatabaseService.UserStoreDatabase.AppSettingsRepository.UpdateGoogleAdsLimitOnIntersitial(appInfo.ShowFirstInterstitialAdOnClickLimit);
                        AdsInterstitialLimitOnRecord = appInfo.ShowFirstInterstitialAdOnClickLimit;
                    }
                    else
                    {
                        AdsInterstitialLimitOnRecord = DatabaseService.UserStoreDatabase.AppSettingsRepository.GetGoogleAdsIntersitialLimit();
                    }
                    CrossMTAdmob.Current.LoadInterstitial(AdsInterstitialId);
                    AdsInterstitialLimitOnRecordRound2 = appInfo.ShowLaterInterstitialAdOnClickLimit;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Crashes.TrackError(new Exception($"Ads Service initialization error: {ex.Message}"));
                }
                catch (Exception) { }
            }
        }

        private static bool? _isAdsServiceAvailable;
        public static bool IsAdsServiceAvailable
        {
            get
            {
                try
                {
                    if (_isAdsServiceAvailable.HasValue == false)
                    {
                        _isAdsServiceAvailable = DisplayAds && AdsInterstitialId.HasValue() && AdsBannerId.HasValue() && (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS);
                    }
                    return _isAdsServiceAvailable.Value;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(new Exception($"Ads Service error: {ex.Message}"));
                    return false;
                }
            }
        }

        public static bool IsInterstitialAvailable
        {
            get
            {
                try
                {
                    var currentCount = DatabaseService.UserStoreDatabase.RequestRecordRepository.GetCount();
                    if (currentCount < AdsInterstitialLimitOnRecord) return false;
                    if (currentCount % AdsInterstitialLimitOnRecord == 0)
                    {
                        DatabaseService.UserStoreDatabase.AppSettingsRepository.UpdateGoogleAdsLimitOnIntersitial(AdsInterstitialLimitOnRecordRound2);
                        AdsInterstitialLimitOnRecord = AdsInterstitialLimitOnRecordRound2;
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(new Exception($"Ads Service error: {ex.Message}"));
                    return false;
                }
            }
        }

        public static MTAdView? BuildAdsBanner()
        {
            try
            {
                return new()
                {
                    AdsId = AdsBannerId,
                    HeightRequest = 50
                };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(new Exception($"Ads Service error: {ex.Message}"));
                return null;
            }
        }
        public static MTAdView? BuildAdsQuizBanner()
        {
            try
            {
                return new()
                {
                    AdsId = AdsQuizBannerId,
                    HeightRequest = 50
                };
            }
            catch (Exception ex)
            {
                Crashes.TrackError(new Exception($"Ads Service error: {ex.Message}"));
                return null;
            }
        }
    }
}
