using AmigosAPI.Data;
using AmigosAPI.DTOs;
using AmigosAPI.DTOs.Bill;
using AmigosAPI.Models;

namespace AmigosAPI.Services
{
    public class BillService
    {
        private BillManagerContext _context;
        private UserService _userService;
        private LedgerService _ledgerService;
        private CurrencyRateService _rateService;

        public BillService(BillManagerContext context, IConfiguration config)
        {
            _context = context;
            _userService = new UserService(context);
            _ledgerService = new LedgerService(context);
            _rateService = new CurrencyRateService(config);
        }

        public async Task<BillDTO> AddBillAsync(NewBillDTO newBill)
        {
            var paidBy = _userService.GetUserByEmail(newBill.PaidByEmail);
            //converting into cad
            double amountCAD, rate;
            if (newBill.Currency.Equals("CAD"))
            {
                amountCAD = newBill.BillAmount;
                rate = 1;
            }
            else
            {
                rate = await _rateService.GetConversionRateAsync(newBill.Currency, "CAD", newBill.BillDate);
                amountCAD = newBill.BillAmount * rate;
            }
            //Create Bill object
            Bill bill = new Bill()
            {
                BillDate = newBill.BillDate,
                Description = newBill.Description,
                Amount = newBill.BillAmount,
                Currency = newBill.Currency,
                AmountCAD = amountCAD,
                ConversionRate = rate,
                PaidBy = paidBy,
                PaidByUserID = paidBy.ID,
                CreatedBy = paidBy,
                CreatedByUserID = paidBy.ID,
                ModifiedBy = paidBy,
                ModifiedByUserID = paidBy.ID
            };            
            Bill savedBill = _context.Bills.Add(bill).Entity;
            _context.SaveChanges();
            //Creating Loan entries
            var userShares = _ledgerService.AddLedgerEntries(bill, newBill.SharedByEmails);
            return ConstructBillDTO(savedBill, userShares);
        }

        public async Task<BillDTO> EditBillAsync(EditBillDTO ebDTO)
        {
            var billInDB = GetBillByID(ebDTO.ID);
            if (billInDB != null)
            {
                bool recalcShare = false;
                double rate=billInDB.ConversionRate, 
                       amountCAD = billInDB.AmountCAD;

                //Change in Bill Date
                if (!ebDTO.BillDate.Date.Equals(billInDB.BillDate.Date) || !ebDTO.Currency.Equals(billInDB.Currency))
                {
                    rate = await _rateService.GetConversionRateAsync(ebDTO.Currency, "CAD", ebDTO.BillDate);
                    amountCAD = ebDTO.BillAmount * rate;
                    recalcShare = true;
                }
                if(ebDTO.BillAmount != billInDB.Amount)
                {
                    recalcShare = true;
                }

                var newPaidBy = _userService.GetUserByEmail(ebDTO.PaidByEmail);

                Bill bill = new Bill()
                {
                    ID = ebDTO.ID,
                    BillDate = ebDTO.BillDate,
                    Description = ebDTO.Description,
                    Amount = ebDTO.BillAmount,
                    Currency = ebDTO.Currency,
                    AmountCAD = amountCAD,
                    ConversionRate = rate,
                    PaidBy = newPaidBy,
                    PaidByUserID = newPaidBy.ID,
                    CreatedBy = billInDB.CreatedBy,
                    CreatedByUserID = billInDB.CreatedByUserID,
                    ModifiedBy = newPaidBy,
                    ModifiedByUserID = newPaidBy.ID
                };

                _context.Entry(billInDB).CurrentValues.SetValues(bill);
                var shares = _ledgerService.UpdateLedgerEntries(bill, ebDTO.SharedByEmails, recalcShare);
                _context.SaveChanges();

                return ConstructBillDTO(bill, shares);
            }
            return null;
        }

        public bool DeleteBill(int billID)
        {
            var bill = GetBillByID(billID);
            if(bill != null)
            {
                _ledgerService.DeleteLedgerEntries(billID);
                bill.IsDeleted = true;
                _context.Bills.Attach(bill);
                _context.Entry(bill).Property(b => b.IsDeleted).IsModified = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public BillDTO ConstructBillDTO(Bill bill, List<UserShareDTO> shares)
        {
            return new BillDTO()
            {
                ID = bill.ID,
                BillDate = bill.BillDate,
                Description = bill.Description,
                Amount = bill.Amount,
                Currency = bill.Currency,
                AmountInCAD = bill.AmountCAD,
                ConversionRate = bill.ConversionRate,
                PaidBy = _userService.GetUserDTO(bill.PaidBy),
                UserShares = shares
            };
        }

        public Bill GetBillByID(int billID)
        {
            return _context.Bills.Where(bill => bill.ID == billID).SingleOrDefault();
        }
    }
}
