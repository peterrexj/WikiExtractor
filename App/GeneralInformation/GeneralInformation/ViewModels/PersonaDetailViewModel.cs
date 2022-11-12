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

        public bool ArePicturesAvailable => Persona.Pictures.Any();
        public bool IsPrimaryPictureAvailable => Persona.PicturePrimaryPath.HasValue();


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
