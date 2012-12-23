using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using ReceiptStorage.DataModel.Enums;

namespace ReceiptStorage.Model
{
    [Table(Name = "Shops")]
    public class Shops : INotifyPropertyChanged, INotifyPropertyChanging
    {
       

        // Define ID: private field, public property, and database column.
        private int _shopsId;
        private string _shopName;
        private string _shopCity;
        private string _shopAdres;
        private Country _shopCountry;
        private string _shopGPSLocalization;
        private string _shopCategories;


        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ShopsId
        {
            get { return _shopsId; }
            set
            {
                if (_shopsId != value)
                {
                    NotifyPropertyChanging("ShopsId");
                    _shopsId = value;
                    NotifyPropertyChanged("ShopsId");
                }
            }
        }

        //Kolumny
        [Column]
        public Country ShopCountry
        {
            get { return _shopCountry; }
            set { _shopCountry = value; }
        }

        [Column]
        public string ShopGpsLocalization
        {
            get { return _shopGPSLocalization; }
            set { _shopGPSLocalization = value; }
        }

        [Column]
        public string ShopAdres
        {
            get { return _shopAdres; }
            set { _shopAdres = value; }
        }

        [Column]
        public string ShopCity
        {
            get { return _shopCity; }
            set { _shopCity = value; }
        }

        [Column]
        public string ShopName
        {
            get { return _shopName; }
            set { _shopName = value; }
        }

        [Column]
        public string ShopCategories
        {
            get { return _shopCategories; }
            set { _shopCategories = value; }
        }

        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
