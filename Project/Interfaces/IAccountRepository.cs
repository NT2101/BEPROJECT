    using Project.Models;

    namespace Project.Interfaces
    {
        public interface IAccountRepository
        {
            ICollection<Account> GetAccounts();
            Account GetAccount(int ID);
            bool AccountExists(int ID);
        bool CreateAccount(Account account);
        bool UpdateAccount(Account Account);
            bool DeleteAccount(Account Account);
            bool Save();
        }
    }
