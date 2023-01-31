using AmigosAPI.Data;
using AmigosAPI.DTOs.Bill;
using AmigosAPI.DTOs.Ledger;
using AmigosAPI.Models;
using Microsoft.Identity.Client;

namespace AmigosAPI.Services
{
    public class LedgerService
    {
        private readonly BillManagerContext _context;
        private UserService _userService;
        public LedgerService(BillManagerContext context, IConfiguration config)
        {
            _context= context;
            _userService = new UserService(context, config);

        }

        public List<BillShareDTO> AddLedgerEntries(Bill bill, List<string> sharedByEmails)
        {
            if(sharedByEmails== null)
            {
                sharedByEmails= new List<string>();
            }
            if (!sharedByEmails.Contains(bill.PaidBy.Email))
            {
                sharedByEmails.Add(bill.PaidBy.Email);
            }
            var shareValue = bill.AmountCAD / sharedByEmails.Count;

            AddNewLedgerEntries(sharedByEmails, shareValue, bill.PaidBy, bill.ID);

            _context.SaveChanges();
            return GetBillShareDTOList(bill.ID);
        }

        public List<BillShareDTO> UpdateLedgerEntries(Bill bill, List<string> sharedByEmails,bool recalculateShare)
        {
            if (sharedByEmails == null)
            {
                sharedByEmails = new List<string>();
            }
            if (!sharedByEmails.Contains(bill.PaidBy.Email))
            {
                sharedByEmails.Add(bill.PaidBy.Email);
            }

            var oldEmailList = _context.Ledger.Where(l => l.BillID == bill.ID && !l.IsDeleted)
                                              .Join(_context.Users,
                                                    l => l.GivenToUserID,
                                                    u => u.ID,
                                                    (l, u) => u.Email).ToList();
            var toRemove = oldEmailList.Except(sharedByEmails).ToList(); //UserEmail Entries to remove from the bill
            var toAdd = sharedByEmails.Except(oldEmailList).ToList(); //UserEmail Entries to add to the bill
            var toUpdate = sharedByEmails.Intersect(oldEmailList).ToList(); //Existing UserEmail Entries to update

            if(toRemove.Count > 0)
            {
                var markDeleted = _context.Ledger.Where(l => l.BillID == bill.ID)
                                  .Join(_context.Users,
                                     l => new { ID = l.GivenToUserID, RemovableEmail = true },
                                     u => new { ID = u.ID, RemovableEmail = toRemove.Contains(u.Email) },
                                     (l, u) => l).ToList();
                markDeleted.ForEach(l => l.IsDeleted = true);
                recalculateShare = true;
            }
            if (recalculateShare || toAdd.Count > 0)
            {
                var shareValue = bill.AmountCAD / sharedByEmails.Count;
                AddNewLedgerEntries(toAdd, shareValue, bill.PaidBy, bill.ID);
                var updatables = _context.Ledger.Where(l => l.BillID == bill.ID && !l.IsDeleted)
                               .Join(_context.Users,
                                     l => new { ID = l.GivenToUserID, UpdateEmail = true },
                                     u => new { ID = u.ID, UpdateEmail = toUpdate.Contains(u.Email) },
                                     (l, u) => l).ToList();
                updatables.ForEach(l => {
                    l.Amount = shareValue;
                    l.GivenBy = bill.PaidBy;
                    l.GivenByUserID = bill.PaidBy.ID;
                });

            }

            _context.SaveChanges();

            return GetBillShareDTOList(bill.ID);
        }

        public void DeleteLedgerEntries(int billID)
        {
            var entries = _context.Ledger.Where(entry =>entry.BillID == billID).ToList();
            entries.ForEach(entry => { entry.IsDeleted= true; });
        }

        private void AddNewLedgerEntries(List<string> emails, double shareValue, User paidBy, int billID)
        {
            foreach (var email in emails)
            {
                var lentTo = _userService.GetUserByEmail(email);
                var entry = new LedgerEntry()
                {
                    Amount = shareValue,
                    EntryType = LedgerEntryType.Loan,
                    GivenByUserID = paidBy.ID,
                    GivenBy = paidBy,
                    GivenTo = lentTo,
                    GivenToUserID = lentTo.ID,
                    BillID = billID
                };

                _context.Ledger.Add(entry);
            }
        }

        public List<BillShareDTO> GetBillShareDTOList(int billID)
        {
            return _context.Ledger.Where(loan => loan.BillID == billID && !loan.IsDeleted)
                                  .Select(loan => new BillShareDTO()
                                  {
                                      UserID = loan.GivenToUserID,
                                      ShareAmount = loan.Amount,
                                      Email = loan.GivenTo.Email
                                  }).ToList();
        }

        
    }
}
