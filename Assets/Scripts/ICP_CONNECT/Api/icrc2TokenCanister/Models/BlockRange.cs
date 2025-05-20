using EdjCase.ICP.Candid.Mapping;
using System.Collections.Generic;
using InternetClients.icrc2TokenCanister.Models;

namespace InternetClients.icrc2TokenCanister.Models
{
	public class BlockRange
	{
		[CandidName("blocks")]
		public List<Block> Blocks { get; set; }

		public BlockRange(List<Block> blocks)
		{
			this.Blocks = blocks;
		}

		public BlockRange()
		{
		}
	}
}