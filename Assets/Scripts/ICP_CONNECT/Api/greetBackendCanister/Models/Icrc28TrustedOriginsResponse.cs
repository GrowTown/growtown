using EdjCase.ICP.Candid.Mapping;
using System.Collections.Generic;

namespace InternetClients.greetBackendCanister.Models
{
	public class Icrc28TrustedOriginsResponse
	{
		[CandidName("trusted_origins")]
		public List<string> TrustedOrigins { get; set; }

		public Icrc28TrustedOriginsResponse(List<string> trustedOrigins)
		{
			this.TrustedOrigins = trustedOrigins;
		}

		public Icrc28TrustedOriginsResponse()
		{
		}
	}
}