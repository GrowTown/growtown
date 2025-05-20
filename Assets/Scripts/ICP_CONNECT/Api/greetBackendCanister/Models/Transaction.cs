using EdjCase.ICP.Candid.Mapping;
using TokenIndex = System.UInt32;
using Time = EdjCase.ICP.Candid.Models.UnboundedInt;
using AccountIdentifier = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	public class Transaction
	{
		[CandidName("buyer")]
		public AccountIdentifier Buyer { get; set; }

		[CandidName("price")]
		public ulong Price { get; set; }

		[CandidName("seller")]
		public AccountIdentifier Seller { get; set; }

		[CandidName("time")]
		public Time Time { get; set; }

		[CandidName("token")]
		public TokenIndex Token { get; set; }

		public Transaction(AccountIdentifier buyer, ulong price, AccountIdentifier seller, Time time, TokenIndex token)
		{
			this.Buyer = buyer;
			this.Price = price;
			this.Seller = seller;
			this.Time = time;
			this.Token = token;
		}

		public Transaction()
		{
		}
	}
}