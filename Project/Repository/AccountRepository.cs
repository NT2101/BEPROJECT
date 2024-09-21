using Project.Data;
using Project.Interfaces;
using Project.Models;

namespace Project.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private DataContext _context;
        public AccountRepository(DataContext context)
        {
            _context = context;
        }
        public bool AccountExists(int ID)
        {
            return _context.Accounts.Any(c => c.ID == ID);
        }

        public bool CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteAccount(Account Account)
        {
            _context.Remove(Account);
            return Save();
        }

       

        public Account GetAccount(int ID)
        {
            return _context.Accounts.Where(e => e.ID == ID).FirstOrDefault();
        }

        public ICollection<Account> GetAccounts()
        {
            return _context.Accounts.ToList();
        }



        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAccount(Account Account)
        {
            _context.Update(Account);
            return Save();
        }
    }
}

