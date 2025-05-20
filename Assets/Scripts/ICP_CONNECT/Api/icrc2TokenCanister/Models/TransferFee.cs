using EdjCase.ICP.Candid.Mapping;
using InternetClients.icrc2TokenCanister.Models;

namespace InternetClients.icrc2TokenCanister.Models
{
	public class TransferFee
	{
		[CandidName("transfer_fee")]
		public Tokens TransferFee_ { get; set; }

		public TransferFee(Tokens transferfee)
		{
			this.TransferFee_ = transferfee;
		}

		public TransferFee()
		{
		}
	}
}