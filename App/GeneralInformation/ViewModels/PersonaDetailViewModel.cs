using Pj.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiExtractor.ViewModels;

namespace GeneralInformation.ViewModels
{
    public class PersonaDetailViewModel : BaseViewModel
    {
        public PersonaDetailViewModel()
        {
            selectedTabIndex = -1;
        }
        private int selectedTabIndex;
        public int SelectedTabIndex { get => selectedTabIndex; set => SetProperty(ref selectedTabIndex, value); }

        private PersonaViewModel _persona;
        public PersonaViewModel Persona
        {
            get { return _persona; }
            set
            {
                _persona = value;
                OnPropertyChanged("Persona");
            }
        }

        public bool IsPicturesAvailable => Persona.Pictures.Any();
        public bool IsPrimaryPictureAvailable => Persona != null && Persona.PicturePrimaryPath.HasValue();
        public bool IsMetaDataAvailable => Persona != null && Persona.Metadatas.Any();
        public bool IsDetailsAvailable => Persona != null && Persona.Paragraphs.Any();

        private int? _availableCount;
        public int? AvailableTabCount 
        {
            get
            {
                if (_availableCount == null)
                {
                    _availableCount = 0;
                    if (IsMetaDataAvailable) _availableCount++;
                    if (IsPicturesAvailable) _availableCount++;
                    if (IsDetailsAvailable) _availableCount++;
                }
                return _availableCount;
            }
        }

        private string _pictureTitle;
        public string PictureTitle
        {
            get
            {
                if (_pictureTitle == null)
                {
                    _pictureTitle = $"Pictures [{Persona?.Pictures.Count}]";
                }
                return _pictureTitle;
            }
            //set
            //{
            //    _pictureTitle =value;
            //    OnPropertyChanged("PictureTitle");
            //}
        }

        private string _currentSelectedPictureCaption;
        public string CurrentSelectedPictureCaption
        {
            get
            {
                return _currentSelectedPictureCaption;
            }
            set
            {
                _currentSelectedPictureCaption = value;
                OnPropertyChanged("CurrentSelectedPictureCaption");
            }
        }
    }
}
