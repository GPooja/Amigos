using AmigosAPI.Data;
using AmigosAPI.DTOs.Bill;
using AmigosAPI.DTOs.Ledger;
using AmigosAPI.DTOs.User;
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
            _userService = new UserService(context,config);
            _ledgerService = new LedgerService(context, config);
            _rateService = new CurrencyRateService(config);
        }

        public async Task<BillDTO> AddBillAsync(NewBillDTO newBill)
        {
            var paidBy = _userService.GetUserByEmail(newBill.PaidByEmail);
            //converting into cad
            double amountCAD, rate;
            if (newBill.CurrencyCode.Equals("CAD"))
            {
                amountCAD = newBill.BillAmount;
                rate = 1;
            }
            else
            {
                rate = await _rateService.GetConversionRateAsync(newBill.CurrencyCode, "CAD", newBill.BillDate);
                amountCAD = newBill.BillAmount * rate;
            }
            //Create Bill object
            Bill bill = new Bill()
            {
                BillDate = newBill.BillDate,
                Description = newBill.Description,
                Amount = newBill.BillAmount,
                CurrencyCode = newBill.CurrencyCode,
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
            var billShares = _ledgerService.AddLedgerEntries(bill, newBill.SharedByEmails);
            return ConstructBillDTO(savedBill, billShares, null);
        }

        public async Task<BillDTO> EditBillAsync(EditBillDTO ebDTO)
        {
            var billInDB = GetBillByID(ebDTO.ID);
            if (billInDB != null)
            {
                bool recalcShare = false;
                double rate = billInDB.ConversionRate,
                       amountCAD = billInDB.AmountCAD;

                //Change in Bill Date
                if (!ebDTO.BillDate.Date.Equals(billInDB.BillDate.Date) || !ebDTO.CurrencyCode.Equals(billInDB.CurrencyCode))
                {
                    rate = await _rateService.GetConversionRateAsync(ebDTO.CurrencyCode, "CAD", ebDTO.BillDate);
                    amountCAD = ebDTO.BillAmount * rate;
                    recalcShare = true;
                }
                if (ebDTO.BillAmount != billInDB.Amount)
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
                    CurrencyCode = ebDTO.CurrencyCode,
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

                return ConstructBillDTO(bill, shares, null);
            }
            return null;
        }

        public bool DeleteBill(int billID)
        {
            var bill = GetBillByID(billID);
            if (bill != null)
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

        public BillDTO ConstructBillDTO(Bill bill, List<BillShareDTO> shares, string? currency)
        {
            var billDTO = new BillDTO()
            {
                ID = bill.ID,
                BillDate = bill.BillDate,
                Description = bill.Description,                
                AmountInCAD = bill.AmountCAD,
                ConversionRate = bill.ConversionRate,
                PaidBy = _userService.GetUserDTO(bill.PaidBy),
                BillShares = shares
            };
            if(string.IsNullOrEmpty(currency))
            {
                if (currency.Equals("CAD"))
                {
                    billDTO.Amount = bill.AmountCAD;
                }
                else 
                {
                    var convRate = _rateService.GetConversionRateAsync(bill.CurrencyCode, currency, billDTO.BillDate).Result;
                    billDTO.Amount = bill.Amount * convRate;
                }
                billDTO.CurrencyCode = currency;
            }
            else
            {
                billDTO.Amount = bill.Amount;
                billDTO.CurrencyCode = bill.CurrencyCode;
            }
            return billDTO;
        }

        public Bill GetBillByID(int billID)
        {
            return _context.Bills.Where(bill => bill.ID == billID && !bill.IsDeleted).SingleOrDefault();
        }

        public BillHistoryDTO GetBillHistory(User user, string currency)
        {
            var bills = GetBillsForUser(user.ID);            

            if (bills != null)
            {
                List<BillDTO> billList = new List<BillDTO>();

                bills.ForEach(bill =>
                {
                    billList.Add(
                        ConstructBillDTO(
                            bill,
                            _ledgerService.GetBillShareDTOList(bill.ID),
                            currency
                        )
                    );
                });


                return new BillHistoryDTO()
                {
                    UserID = user.ID,
                    UserEmail = user.Email,
                    TotalBills = bills.Count,
                    TotalBillAmount = bills.Select(bill => bill.Amount).Sum(),
                    Bills = billList
                };
            }
            return null;
        }

        public List<Bill> GetBillsForUser(int userID)
        {
            return _context.Bills.Where(b => b.PaidByUserID == userID && !b.IsDeleted).ToList();
        }

        
    }
}
