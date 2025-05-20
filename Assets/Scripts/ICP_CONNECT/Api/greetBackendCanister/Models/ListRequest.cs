using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using EdjCase.ICP.Candid.Models;
using TokenIdentifier = System.String;
using SubAccount = System.Collections.Generic.List<System.Byte>;

namespace InternetClients.greetBackendCanister.Models
{
	public class ListRequest
	{
		[CandidName("from_subaccount")]
		public ListRequest.FromSubaccountInfo FromSubaccount { get; set; }

		[CandidName("price")]
		public OptionalValue<ulong> Price { get; set; }

		[CandidName("token")]
		public TokenIdentifier Token { get; set; }

		public ListRequest(ListRequest.FromSubaccountInfo fromSubaccount, OptionalValue<ulong> price, TokenIdentifier token)
		{
			this.FromSubaccount = fromSubaccount;
			this.Price = price;
			this.Token = token;
		}

		public ListRequest()
		{
		}

		public class FromSubaccountInfo : OptionalValue<SubAccount>
		{
			public FromSubaccountInfo()
			{
			}

			public FromSubaccountInfo(SubAccount value) : base(value)
			{
			}
		}
	}
}