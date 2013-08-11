using GalaSoft.MvvmLight;
using Ninject;
using Shane.Church.StirlingBirthday.Core.Data;
using Shane.Church.StirlingBirthday.Core.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingBirthday.Core.ViewModels
{
    public class BirthdayTileFrontViewModel : ObservableObject
    {
        public BirthdayTileFrontViewModel(BirthdayContact contact)
        {
            if (contact != null)
            {
                if (contact.Picture != null)
                {
                    ContactImage = new byte[contact.Picture.Length];
                    contact.Picture.CopyTo(ContactImage, 0);
                }
                Name = contact.DisplayName;
                Birthdate = contact.Date.ToString("M");
                Age = contact.Age;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                Set(() => Name, ref _name, value);
            }
        }

        private string _birthdate;
        public string Birthdate
        {
            get { return _birthdate; }
            set
            {
                Set(() => Birthdate, ref _birthdate, value);
            }
        }

        private int _age;
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
            get { return Age.ToString(); }
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
