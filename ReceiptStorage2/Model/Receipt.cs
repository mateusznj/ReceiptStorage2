using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.Model;

namespace ReceiptStorage.Model
{
    [Table(Name = "Receipt")]
    public class Receipt : INotifyPropertyChanged, INotifyPropertyChanging
    {

        #region Prywatne zmienne

        private int _receiptId;
        private DateTime _receiptCreate;
        private DateTime _receiptDate;
        private double _receiptMoney;
        private Currency _receiptCurrency; //enum i narazie tylko PLN - nie dostpeny w programie
        private int _receiptShopsId;

        private ShopsCategory _receiptShopsCategory;
        private OperationType _receiptOperationType;

        [Column(IsVersion = true)]
        private Binary _version;

        #endregion Prywatne zmienne

        #region Publiczne zmienne

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ReceiptId
        {
            get { return _receiptId; }
            set
            {
                if (_receiptId != value)
                {
                    NotifyPropertyChanging("ReceiptId");
                    _receiptId = value;
                    NotifyPropertyChanged("ReceiptId");
                }
            }
        }

        //Kolumny
        [Column]
        public DateTime ReceiptCreate
        {
            get { return _receiptCreate; }
            set { _receiptCreate = value; }
        }

        [Column]
        public DateTime ReceiptDate
        {
            get { return _receiptDate; }
            set { _receiptDate = value; }
        }

        [Column]
        public double ReceiptMoney
        {
            get { return _receiptMoney; }
            set { _receiptMoney = value; }
        }

        [Column]
        public Currency ReceiptCurrency
        {
            get { return _receiptCurrency; }
            set { _receiptCurrency = value; }
        }

        private EntityRef<Shops> _shops;

        [Column]
        public int ReceiptShopsId
        {
            get { return _receiptShopsId; }
            set { _receiptShopsId = value; }
        }
     
        [Association(OtherKey = "ShopsId",ThisKey = "ReceiptShopsId",Storage = "_shops")]
        public Shops ShopsId
        {
            get
            {
                return _shops.Entity;
            }
            set
            {
                if (value != null)
                {
                    _shops.Entity = value;
                    ReceiptShopsId = value.ShopsId;
                }
                
            }
        }

        [Column]
        public ShopsCategory ReceiptShopsCategory
        {
            get { return _receiptShopsCategory; }
            set { _receiptShopsCategory = value; }
        }

        [Column]
        public OperationType ReceiptOperationType
        {
            get { return _receiptOperationType; }
            set { _receiptOperationType = value; }
        }

        //[Column]
        private EntitySet<Foto> _foto;

        [Association(Storage = "_foto", ThisKey = "ReceiptId", OtherKey = "FotoReceiptId")]
        public EntitySet<Foto> ReceiptFoto
        {
            get { return _foto; }
            set { _foto.Assign(value); }
            
        }

        // Called during an add operation
        private void AttachFoto(Foto foto)
        {
            NotifyPropertyChanging("ReceiptFoto");
            foto.ReceiptData = this;
        }

        // Called during a remove operation
        private void DetachFoto(Foto foto)
        {
            NotifyPropertyChanging("ReceiptFoto");
            foto.ReceiptData = null;
        }

        public Receipt()
        {
            _foto = new EntitySet<Foto>(AttachFoto, DetachFoto);
        }

        #endregion Publiczne zmienne

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
