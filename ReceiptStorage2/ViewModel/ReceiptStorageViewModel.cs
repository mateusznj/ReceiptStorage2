using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ReceiptStorage.DataModel.Enums;
using ReceiptStorage.DataModel.Simplified;
using ReceiptStorage.Model;
using ReceiptStorage.Extensions;

namespace ReceiptStorage.ViewModel
{
    public class ReceiptStorageViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private ReceiptStorageDataContext ReceiptDB;

        // Class constructor, create the data context object.
        public ReceiptStorageViewModel(string receiptDBConnectionString)
        {
            ReceiptDB = new ReceiptStorageDataContext(receiptDBConnectionString);
        }

        private ObservableCollection<Receipt> _allReceiptItems;

        public ObservableCollection<Receipt> AllReceiptItems
        {
            get { return _allReceiptItems; }
            set
            {
                _allReceiptItems = value;
                NotifyPropertyChanged("AllReceiptItems");
            }
        }

        private ObservableCollection<Shops> _allShopsItems;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Shops> AllShopsItems
        {
            get { return _allShopsItems; }
            set
            {
                _allShopsItems = value;
                NotifyPropertyChanged("AllShopsItems");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newReceiptItem"></param>
        public void AddReceiptItem(Receipt newReceiptItem)
        {
            ReceiptDB.Receipt.InsertOnSubmit(newReceiptItem);

            ReceiptDB.SubmitChanges();

            LoadReceiptFromDatabase();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFotoItem"></param>
        public void AddFotoItem(Foto newFotoItem)
        {
            ReceiptDB.Foto.InsertOnSubmit(newFotoItem);

            ReceiptDB.SubmitChanges();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newShopsItem"></param>
        public void AddShopsItem(Shops newShopsItem)
        {
            ReceiptDB.Shops.InsertOnSubmit(newShopsItem);

            ReceiptDB.SubmitChanges();

            //AllShopsItems.Add(newShopsItem);
            LoadShopsFromDatabase();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void LoadReceiptFromDatabase()
        {
            // Specify the query for all to-do items in the database.
            var rsItemsInDB = from Receipt rs in ReceiptDB.Receipt
                                select rs;

            // Query the database and load all to-do items.
            AllReceiptItems = new ObservableCollection<Receipt>(rsItemsInDB);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadShopsFromDatabase()
        {
            // Specify the query for all to-do items in the database.
            var rsShopsItemsInDB = from Shops rs in ReceiptDB.Shops
                                   orderby rs.ShopName
                                   select rs;

            // Query the database and load all to-do items.
            AllShopsItems = new ObservableCollection<Shops>(rsShopsItemsInDB);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptId"></param>
        public void DeleteReceiptItem(int receiptId)
        {
            var rsItemInDB = (from Receipt rs in ReceiptDB.Receipt
                                            where rs.ReceiptId == receiptId
                              select rs).FirstOrDefault();

            var rsFotoInDB = (from Foto ft in ReceiptDB.Foto
                              where ft.FotoReceiptId == receiptId
                              select ft).FirstOrDefault();

            ReceiptDB.Foto.DeleteOnSubmit(rsFotoInDB);
            ReceiptDB.Receipt.DeleteOnSubmit(rsItemInDB);

            ReceiptDB.SubmitChanges();

            LoadReceiptFromDatabase();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetAllReceiptWithShops()
        {
            return GetAllReceiptWithShops(String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetAllReceiptWithShops(string shopName)
        {
            var q = from Receipt rs in ReceiptDB.Receipt
                            join Shops sh in ReceiptDB.Shops on rs.ReceiptShopsId equals sh.ShopsId
                            join Foto ft in ReceiptDB.Foto on rs.ReceiptId equals ft.FotoReceiptId
                            where (shopName != String.Empty ? sh.ShopName.ToUpper().Contains(shopName.ToUpper()):true)
                            orderby  rs.ReceiptCreate
                    select new ReceiptSimplified
                            {
                                ReceiptId = rs.ReceiptId,
                                ShopName = sh.ShopName,
                                ReceiptDate = rs.ReceiptDate,
                                ReceiptMoney = rs.ReceiptMoney,
                                FotoImage = ft.FotoImage,
                                ReceiptCurrency = rs.ReceiptCurrency
                            };

            return q.ToList();
        }

        /// <summary>
        /// Pobieranie danych dla paragonu o ID
        /// </summary>
        /// <param name="receiptId"></param>
        /// <returns></returns>
        public ReceiptSimplified GetReceipt(int receiptId)
        {
            var q = from Receipt rs in ReceiptDB.Receipt
                            join Shops sh in ReceiptDB.Shops on rs.ReceiptShopsId equals sh.ShopsId
                            join Foto ft in ReceiptDB.Foto on rs.ReceiptId equals ft.FotoReceiptId
                            where rs.ReceiptId == receiptId
                            orderby rs.ReceiptDate descending
                    select new ReceiptSimplified
                    {
                        ReceiptId = rs.ReceiptId,
                        ShopName = sh.ShopName,
                        ReceiptDate = rs.ReceiptDate,
                        ReceiptMoney = rs.ReceiptMoney,
                        FotoImage = ft.FotoImage,
                        ReceiptCurrency = rs.ReceiptCurrency,
                        ReceiptOperationType = rs.ReceiptOperationType,
                        ReceiptShopsCategory = rs.ReceiptShopsCategory,
                        ReceiptShopsLocation = sh.ShopGpsLocalization
                    };

            return q.FirstOrDefault();
        }

        /// <summary>
        /// Pbiera dane wydatków z ostatniego miesiąca
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetReceiptExpensesChartsPerMonth()
        {
            //var q2 = Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))  // Days: 1, 2 ... 31 etc.
            //       .Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day)) // Map each day to a date
            //       .AsEnumerable();

            var dayOfMonth = (from b in Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                              select new {Number = b}).ToList();
 
            var q = (from Receipt rs in ReceiptDB.Receipt
                    where rs.ReceiptDate.Month == DateTime.Now.Month 
                    group rs by new {date = rs.ReceiptDate
                    } into g 
                    select new
                               {
                                   ReceiptDate = g.Key.date.Day, 
                                   ReceiptSum = g.Sum(rs => rs.ReceiptMoney)
                               }).ToList();

            var q2 = (from dm in dayOfMonth
                     join  dd in q
                     on dm.Number equals dd.ReceiptDate into JoinedEmpDept
                     from dd in JoinedEmpDept.DefaultIfEmpty()
                     select new
                                 {
                                     ReceiptDate = dm.Number, 
                                     ReceiptSum = dd != null ? dd.ReceiptSum : 0.0
                                 }).ToList();
            

           MathHelper.TrendLineOperand(q2.Select(d => d.ReceiptSum).ToArray());

            var q3 = (from dm in q2 select new
                                               {
                                                   dm.ReceiptDate, 
                                                   dm.ReceiptSum, 
                                                   ReceiptTrend = MathHelper.GetTrendLine(dm.ReceiptDate)
                                               }).ToList();

            return q3;
        }

        /// <summary>
        /// Pbiera dane wydatków z ostatniego tygodnia
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetReceiptExpensesChartsPerWeek()
        {

            var last7Days = (from b in Enumerable.Range(1,  DateTime.Now.Subtract(DateTime.Now.Date.AddDays(-7)).Days)
                             select new { Date = DateTime.Now.Date.AddDays(-7).AddDays(b).Date }).ToList();
            
             var q = (from Receipt rs in ReceiptDB.Receipt
                      where rs.ReceiptDate.Date >= DateTime.Now.Date.AddDays(-7).Date && rs.ReceiptDate.Date <= DateTime.Now.Date
                    group rs by rs.ReceiptDate.Date into g
                    select new
                               {
                                   ReceiptDate = g.Key, 
                                   ReceiptSum = g.Sum(rs => rs.ReceiptMoney)
                                })
                               .ToList();
                              // .OrderBy(o => DateHelper.GetDayOfWeekNumber(o.ReceiptDateOrgName));

             var q2 = (from dm in last7Days
                       join dd in q
                       on dm.Date equals dd.ReceiptDate into JoinedEmpDept
                       from dd in JoinedEmpDept.DefaultIfEmpty()
                       select new
                                  {
                                      ReceiptDate =String.Format("{0}", CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(dm.Date.DayOfWeek)), 
                                      ReceiptSum = dd != null ? dd.ReceiptSum : 0.0
                                  }).ToList();



            return q2;
        }

        /// <summary>
        /// Pbiera dane wydatków z ostatniego roku
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetReceiptExpensesChartsPerYear()
        {
            var q = from Receipt rs in ReceiptDB.Receipt
                    where rs.ReceiptDate.Year == DateTime.Now.Year
                    group rs by new {month = rs.ReceiptDate.Month,years = rs.ReceiptDate.Year} into g
                    select new
                               {
                                   ReceiptDate = String.Format("{0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.month), g.Key.years), 
                                   ReceiptSum = g.Sum(rs => rs.ReceiptMoney)
                               };

           
            return q.ToList();
        }

        /// <summary>
        /// Pbiera dane wydatków z wybranego okresu
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, double> GetReceiptExpensesPer(CalendarType typ)
        {
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            Dictionary<string, double> receiptExpenses = new Dictionary<string, double>();

            switch (typ)
            {
                case CalendarType.tydzień:
                    start = DateTime.Now.Date.AddDays(-7).Date;
                    end = DateTime.Now.Date;
                    break;
                case CalendarType.miesiąc:
                    start = DateTime.Now.Date.AddMonths(-1).Date;
                    end = DateTime.Now.Date;
                    break;
                case CalendarType.rok:
                    start = DateTime.Now.Date.AddYears(-1).Date;
                    end = DateTime.Now.Date;
                    break;
            }

            var q = (from Receipt rs in ReceiptDB.Receipt
                     where rs.ReceiptDate >= start && rs.ReceiptDate <= end
                     select rs.ReceiptMoney).ToList();
            if (q.Any())
            {
                var s = q.Select(b => b).Sum();
                var a = q.Select(b => b).Average();

                
                receiptExpenses.Add("ReceiptSum", Math.Round(s, 2));
                receiptExpenses.Add("ReceiptAvg", Math.Round(a, 2));
            }
            else
            {
                receiptExpenses.Add("ReceiptSum",0);
                receiptExpenses.Add("ReceiptAvg",0);
            }
            return receiptExpenses;
        }

        /// <summary>
        /// Pobiera dane wydatków z wybranego okresu
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetReceiptShopsCategory(CalendarType typ)
        {
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            switch (typ)
            {
                case CalendarType.tydzień:
                    start = DateTime.Now.Date.AddDays(-7).Date;
                    end = DateTime.Now.Date;
                    break;
                case CalendarType.miesiąc:
                    start = DateTime.Now.Date.AddMonths(-1).Date;
                    end = DateTime.Now.Date;
                    break;
                case CalendarType.rok:
                    start = DateTime.Now.Date.AddYears(-1).Date;
                    end = DateTime.Now.Date;
                    break;
            }

            var q = (from Receipt rs in ReceiptDB.Receipt
                     where rs.ReceiptDate >= start && rs.ReceiptDate <= end
                     group rs by new { shopsCategory = rs.ReceiptShopsCategory }
                         into g
                         select new PieChartsSimplified
                                    {
                                        ReceiptCateroryName = String.Format("{0}", g.Key.shopsCategory),
                                        ReceiptCount = g.Count()
                                    });

            return  q.ToList();

        }

        /// <summary>
        /// Pbiera dane wydatków z wybranego okresu
        /// </summary>
        /// <returns></returns>
        public IList<Foto> GetAllFotoToSync()
        {
            var q = (from Foto ft in ReceiptDB.Foto
                     where ft.FotoSyncStatus == StatusEnum.No
                     select ft);

            return q.ToList();

        }

        public void UpdateFotoSyncStatus(int fotoId, StatusEnum status)
        {
            var fotoToUpdate = (from Foto ft in ReceiptDB.Foto where ft.FotoId == fotoId select ft).FirstOrDefault();

            // update the city by changing its name
            if (fotoToUpdate != null) fotoToUpdate.FotoSyncStatus = status;

            ReceiptDB.SubmitChanges();
            //using (ReceiptStorageDataContext context = new ReceiptStorageDataContext("Data Source=isostore:/Receipt.sdf"))
            //{
            //    // find a city to update
            //    IQueryable<Foto> cityQuery = (from Foto ft in ReceiptDB.Foto where ft.FotoId == fotoId select ft);
            //    Foto cityToUpdate = cityQuery.FirstOrDefault();

            //    // update the city by changing its name
            //    cityToUpdate.FotoSyncStatus = status;

            //    // save changes to the database
            //    context.SubmitChanges();
            //}
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
