using AmigosAPI.Data;
using AmigosAPI.DTOs.Summary;
using AmigosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AmigosAPI.Services
{
    public class SummaryService
    {
        private BillManagerContext _context;
        private CurrencyRateService _currencyRateService;

        public SummaryService(BillManagerContext context, IConfiguration config)
        {
            _context = context;
            _currencyRateService = new CurrencyRateService(config); 
        }

        public QuarterlySummaryDTO GetQuarterlySummaries(User user, string currency)
        {
            List<QuarterlyData> qsList = new List<QuarterlyData>();

            var today = DateTime.Now;
            int currQtr = (today.Month + 2) / 3;
            var qtrLastMonth = currQtr * 3;
            var qtrLastMonthDays = DateTime.DaysInMonth(today.Year, qtrLastMonth);
            var qtrStartDate = new DateTime(today.Year, qtrLastMonth, qtrLastMonthDays).AddMonths(-12).AddDays(1);

            var convRate = !currency.Equals("CAD") ? _currencyRateService.GetConversionRateAsync("CAD", currency, today).Result : 1;

            for (int q = 1; q <= 4; q++)
            {
                var qtrEndDate = qtrStartDate.AddMonths(3).AddDays(-1);
                qsList.Add(new QuarterlyData()
                {
                    QuarterName = "Q" + ((qtrStartDate.Month + 2) / 3) + " - " + qtrStartDate.Year,
                    TotalAmountOnBills = GetTotalAmountOnBills(user.ID, convRate, qtrStartDate, qtrEndDate),
                    AmountUserOwes = GetUserOwes(user.ID, convRate, qtrStartDate, qtrEndDate),
                    OthersOweUser = GetOthersOweUser(user.ID, convRate, qtrStartDate, qtrEndDate)
                });
                qtrStartDate = qtrEndDate.AddDays(1);
            }
            return new QuarterlySummaryDTO()
            {
                UserID = user.ID,
                CurrencyCode = currency,
                Quarters = qsList
            };


        }

        public double GetTotalAmountOnBills(int iD, double convRate, DateTime from, DateTime to)
        {
            return _context.Bills.Where(b =>
                                            b.PaidByUserID == iD &&
                                            b.BillDate > from &&
                                            b.BillDate < to && 
                                            !b.IsDeleted)
                                 .Select(b => b.Amount)
                                 .Sum() * convRate;
        }

        public List<Bill> GetBillsInDateRange(DateTime from, DateTime to)
        {
            return _context.Bills.Where(b =>
                                            b.BillDate > from &&
                                            b.BillDate < to && 
                                            !b.IsDeleted)
                                 .ToList();
        }
        public double GetUserOwes(int iD, double convRate, DateTime from, DateTime to)
        {
            var billIDs = GetBillsInDateRange(from, to).Select(b => b.ID).ToList();
            return _context.Ledger.Where(l =>
                                             billIDs.Contains((int)l.BillID) &&
                                             l.GivenToUserID == iD && 
                                             !l.IsDeleted)
                                  .Select(b => b.Amount)
                                  .Sum() * convRate;
        }
        
        public List<UserShare> GetUserOwedShares(int id, string currency, double convRate)
        {
             return _context.Ledger.Where(l => l.GivenToUserID == id && !l.IsDeleted)
                            .Join(_context.Bills,
                                 l => l.BillID,
                                 b => b.ID,
                                 (l, b) => new UserShare()
                                 {
                                     BillID = b.ID,
                                     ShareAmount = l.Amount * convRate,
                                     CurrencyCode = currency,
                                     OwedToName = b.PaidBy.FirstName + " " + b.PaidBy.LastName,
                                     OwedToEmail = b.PaidBy.Email
                                 }).ToList();                                                          
        }

        public double GetOthersOweUser(int iD, double convRate, DateTime from, DateTime to)
        {
            var billIDs = GetBillsInDateRange(from, to).Where(b => b.PaidByUserID == iD && !b.IsDeleted).Select(b => b.ID).ToList();
            return _context.Ledger.Where(l =>
                                             billIDs.Contains((int)l.BillID) &&
                                             l.GivenToUserID != iD && 
                                             !l.IsDeleted)
                                  .Select(b => b.Amount)
                                  .Sum() * convRate;
        }

        public UserShareDTO GetUserShares(User user, string currency)
        {
            var convRate = !currency.Equals("CAD") ? _currencyRateService.GetConversionRateAsync("CAD", currency, DateTime.Today).Result : 1;
            var userShares = GetUserOwedShares(user.ID, currency, convRate);
            return new UserShareDTO()
            {
                UserID = user.ID,
                UserEmail = user.Email,
                TotalAmountUserOwes = userShares.Select(us => us.ShareAmount).Sum(),
                UserShares = userShares
            };

        }
    }
}
