using EdjCase.ICP.Candid.Mapping;
using EdjCase.ICP.Candid.Models;
using TokenIndex = System.UInt32;
using Time1 = EdjCase.ICP.Candid.Models.UnboundedInt;

namespace InternetClients.greetBackendCanister.Models
{
	public class Deposit
	{
		[CandidName("collectionCanister")]
		public Principal CollectionCanister { get; set; }

		[CandidName("pubKey")]
		public Principal PubKey { get; set; }

		[CandidName("sender")]
		public Principal Sender { get; set; }

		[CandidName("timestamp")]
		public Time1 Timestamp { get; set; }

		[CandidName("tokenId")]
		public TokenIndex TokenId { get; set; }

		public Deposit(Principal collectionCanister, Principal pubKey, Principal sender, Time1 timestamp, TokenIndex tokenId)
		{
			this.CollectionCanister = collectionCanister;
			this.PubKey = pubKey;
			this.Sender = sender;
			this.Timestamp = timestamp;
			this.TokenId = tokenId;
		}

		public Deposit()
		{
		}
	}
}