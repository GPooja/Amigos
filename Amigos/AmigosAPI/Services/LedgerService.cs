using AmigosAPI.Data;
using AmigosAPI.DTOs;
using AmigosAPI.Models;

namespace AmigosAPI.Services
{
    public class LedgerService
    {
        private readonly BillManagerContext _context;
        private UserService _userService;

        public LedgerService(BillManagerContext context)
        {
            _context= context;
            _userService = new UserService(context);
        }

        public List<UserShareDTO> AddLedgerEntries(Bill bill, List<string> sharedByEmails)
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
            return GetUserShareDTOList(bill.ID);
        }

        public int UpdatePaidByInLedger(int iD, User paidBy)
        {
            var entriesAffected = _context.Ledger.Where(l => l.BillID == iD).ToList();
            entriesAffected.ForEach(l => {
                l.GivenBy = paidBy;
                l.GivenByUserID = paidBy.ID;
            });
            return _context.SaveChanges();
        }

        public List<UserShareDTO> UpdateLedgerEntries(Bill bill, List<string> sharedByEmails,bool recalculateShare)
        {
            var oldEmailList = _context.Ledger.Where(l => l.BillID == bill.ID)
                                              .Join(_context.Users,
                                                    l => l.GivenToUserID,
                                                    u => u.ID,
                                                    (l, u) => u.Email).ToList();
            var toRemove = oldEmailList.Except(sharedByEmails).ToList(); //UserEmail Entries to remove from the bill
            var toAdd = sharedByEmails.Except(sharedByEmails).ToList(); //UserEmail Entries to add to the bill
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
                var updatables = _context.Ledger.Where(l => l.BillID == bill.ID)
                               .Join(_context.Users,
                                     l => new { ID = l.GivenToUserID, UpdateEmail = true },
                                     u => new { ID = u.ID, UpdateEmail = toUpdate.Contains(u.Email) },
                                     (l, u) => l).ToList();
                updatables.ForEach(l => l.Amount = shareValue);

            }

            UpdatePaidByInLedger(bill.ID, bill.PaidBy);
            _context.SaveChanges();

            return GetUserShareDTOList(bill.ID);
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

        private List<UserShareDTO> GetUserShareDTOList(int billID)
        {
            return _context.Ledger.Where(loan => loan.BillID == billID)
                                  .Select(loan => new UserShareDTO()
                                  {
                                      UserID = loan.GivenToUserID,
                                      ShareAmount = loan.Amount,
                                      Email = loan.GivenTo.Email
                                  }).ToList();
        }
    }
}
