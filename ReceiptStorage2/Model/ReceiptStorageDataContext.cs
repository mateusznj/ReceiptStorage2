using System.Data.Linq;


namespace ReceiptStorage.Model
{
    public class ReceiptStorageDataContext : DataContext
    {
        // Pass the connection string to the base class.
        public ReceiptStorageDataContext(string connectionString)
            : base(connectionString)
        { }

        /// <summary>
        /// Inicjalizacja obiektu tabeli Receipt
        /// </summary>
        public Table<Receipt> Receipt;

        /// <summary>
        /// Inicjalizacja obiektu tabeli Shops
        /// </summary>
        public Table<Shops> Shops;

        /// <summary>
        /// Inicjalizacja obiektu tabeli Foto
        /// </summary>
        public Table<Foto> Foto;

        /// <summary>
        /// Inicjalizacja obiektu tabeli Foto
        /// </summary>
        public Table<Dictionary> Dictionary;
    }

    

   
}
