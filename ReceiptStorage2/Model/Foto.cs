using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using ReceiptStorage.DataModel.Enums;

namespace ReceiptStorage.Model
{
    [Table(Name = "Foto")]
    public class Foto : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _fotoId;
        private string _fotoPath;
        private byte[] _fotoImage;
        private StatusEnum _fotoSyncStatus;
        
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity")]
        public int FotoId
        {
            get { return _fotoId; }
            set
            {
                if (_fotoId != value)
                {
                    NotifyPropertyChanging("FotoId");
                    _fotoId = value;
                    NotifyPropertyChanged("FotoId");
                }
            }
        }

        //Kolumny

        [Column]
        public string FotoPath
        {
            get { return _fotoPath; }
            set { _fotoPath = value; }
        }

        [Column(DbType = "image")]
        public byte[] FotoImage
        {
            get { return _fotoImage; }
            set
            {
                if (_fotoImage != value)
                {
                    NotifyPropertyChanging("FotoId");
                    _fotoImage = value;
                    NotifyPropertyChanged("FotoId");
                }
            }
        }

        private EntityRef<Receipt> _receipt;
        
        [Column]
        internal int FotoReceiptId;
        
        [Association(Storage = "_receipt", OtherKey = "ReceiptId", ThisKey = "FotoReceiptId", IsForeignKey = true)]
        public Receipt ReceiptData
        {
            get
            {
                return _receipt.Entity;
            }
            set
            {
                NotifyPropertyChanging("ReceiptData");
                _receipt.Entity = value;

                if (value != null)
                {
                    FotoReceiptId = value.ReceiptId;
                }

                NotifyPropertyChanging("ReceiptData");
            }
        }

        [Column]
        public StatusEnum FotoSyncStatus
        {
            get { return _fotoSyncStatus; }
            set
            {
                if (_fotoSyncStatus != value)
                {
                    NotifyPropertyChanging("FotoSyncStatus");
                    _fotoSyncStatus = value;
                    NotifyPropertyChanged("FotoSyncStatus");
                }
            }
        }

        [Column(IsVersion=true)]
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
