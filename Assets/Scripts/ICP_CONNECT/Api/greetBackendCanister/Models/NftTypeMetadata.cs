using EdjCase.ICP.Candid.Mapping;
using EdjCase.ICP.Candid.Models;

namespace InternetClients.greetBackendCanister.Models
{
	public class NftTypeMetadata
	{
		[CandidName("nft_type_cost")]
		public ulong NftTypeCost { get; set; }

		[CandidName("nft_type_quantity")]
		public UnboundedUInt NftTypeQuantity { get; set; }

		[CandidName("nfttype")]
		public string Nfttype { get; set; }

		public NftTypeMetadata(ulong nftTypeCost, UnboundedUInt nftTypeQuantity, string nfttype)
		{
			this.NftTypeCost = nftTypeCost;
			this.NftTypeQuantity = nftTypeQuantity;
			this.Nfttype = nfttype;
		}

		public NftTypeMetadata()
		{
		}
	}
}