using EdjCase.ICP.Candid.Mapping;
using InternetClients.icrc2TokenCanister.Models;

namespace InternetClients.icrc2TokenCanister.Models
{
	public class AllowanceArgs
	{
		[CandidName("account")]
		public Account Account { get; set; }

		[CandidName("spender")]
		public Account Spender { get; set; }

		public AllowanceArgs(Account account, Account spender)
		{
			this.Account = account;
			this.Spender = spender;
		}

		public AllowanceArgs()
		{
		}
	}
}