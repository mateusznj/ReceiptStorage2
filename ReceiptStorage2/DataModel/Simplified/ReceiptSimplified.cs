using System;
using ReceiptStorage.DataModel.Enums;

namespace ReceiptStorage.DataModel.Simplified
{
    public class ReceiptSimplified
    {
        public int ReceiptId { get; set; }
        public string ShopName { get; set; } 
        public DateTime ReceiptDate { get; set; }
        public double ReceiptMoney { get; set; }
        public byte[] FotoImage { get; set; }
        public Currency ReceiptCurrency { get; set; }
        public OperationType ReceiptOperationType { get; set; }
        public ShopsCategory ReceiptShopsCategory { get; set; }
        public string ReceiptShopsLocation { get; set; }
    }
}
