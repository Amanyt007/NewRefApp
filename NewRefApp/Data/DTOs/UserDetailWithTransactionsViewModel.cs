using NewRefApp.Models;

namespace NewRefApp.Data.DTOs
{
    public class UserDetailWithTransactionsViewModel
    {
        public User User { get; set; }
        public UserTransactionsViewModel TransactionsData { get; set; }
        public TeamMemberDataDto TeamMemberDetails { get; set; }
    }
}
