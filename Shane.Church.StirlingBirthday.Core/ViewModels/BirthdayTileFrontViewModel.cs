using GalaSoft.MvvmLight;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Strings;
using System;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
    public class BirthdayTileFrontViewModel : ObservableObject
    {
        public BirthdayTileFrontViewModel(BirthdayContact contact, byte[] picture)
        {
            if (contact != null)
            {
                Name = contact.DisplayName;
                Birthdate = contact.Date.ToString("M");
                Age = contact.Age;
            }
            if (picture != null)
            {
                ContactImage = picture;
            }
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                Set(() => Name, ref _name, value);
            }
        }

        private string _birthdate = "";
        public string Birthdate
        {
            get { return _birthdate; }
            set
            {
                Set(() => Birthdate, ref _birthdate, value);
            }
        }

        private int _age = 0;
        public int Age
        {
            get { return _age; }
            set
            {
                if (Set(() => Age, ref _age, value))
                    RaisePropertyChanged(() => AgeText);
            }
        }

        public virtual string AgeText
        {
            get
            {
                return Math.Abs(Age) == 1 ? string.Format(Resources.SingularAgeText, Age) : string.Format(Resources.PluralAgeText, Age);
            }
        }

        private byte[] _contactImage;
        public byte[] ContactImage
        {
            get { return _contactImage; }
            set
            {
                if (Set(() => ContactImage, ref _contactImage, value))
                {
                    RaisePropertyChanged(() => ContactImageVisible);
                    RaisePropertyChanged(() => IconCanvasVisible);
                    RaisePropertyChanged(() => IconCanvasSmallVisible);
                }
            }
        }

        public bool ContactImageVisible
        {
            get { return ContactImage != null && ContactImage.Length > 0; }
        }

        public bool IconCanvasVisible
        {
            get { return !ContactImageVisible; }
        }
        public bool IconCanvasSmallVisible
        {
            get { return ContactImageVisible; }
        }

    }
}
