using EdjCase.ICP.Candid.Mapping;
using EdjCase.ICP.Candid.Models;
using InternetClients.icrc2TokenCanister.Models;
using Icrc1Tokens = EdjCase.ICP.Candid.Models.UnboundedUInt;

namespace InternetClients.icrc2TokenCanister.Models
{
	public class Allowance
	{
		[CandidName("allowance")]
		public Icrc1Tokens Allowance_ { get; set; }

		[CandidName("expires_at")]
		public OptionalValue<TimeStamp> ExpiresAt { get; set; }

		public Allowance(Icrc1Tokens allowance, OptionalValue<TimeStamp> expiresAt)
		{
			this.Allowance_ = allowance;
			this.ExpiresAt = expiresAt;
		}

		public Allowance()
		{
		}
	}
}