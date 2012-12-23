using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace ReceiptStorage.Model
{
    [Table(Name = "Dictionary")]
    public class Dictionary : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _dictionaryId;
        private string _dictionaryType;
        private string _dictionaryCode;
        private string _dictionaryDescription;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int DictionaryId
        {
            get { return _dictionaryId; }
            set
            {
                if (_dictionaryId != value)
                {
                    NotifyPropertyChanging("DictionaryId");
                    _dictionaryId = value;
                    NotifyPropertyChanged("DictionaryId");
                }
            }
        }

        //Kolumny

        public string DictionaryType
        {
            get { return _dictionaryType; }
            set { _dictionaryType = value; }
        }

        public string DictionaryCode
        {
            get { return _dictionaryCode; }
            set { _dictionaryCode = value; }
        }

        public string DictionaryDescription
        {
            get { return _dictionaryDescription; }
            set { _dictionaryDescription = value; }
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
