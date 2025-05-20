using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using EdjCase.ICP.Candid.Models;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Metadata1
	{
		[VariantTagProperty]
		public Metadata1Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Metadata1(Metadata1Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Metadata1()
		{
		}

		public static Metadata1 Fungible(Metadata1.FungibleInfo info)
		{
			return new Metadata1(Metadata1Tag.Fungible, info);
		}

		public static Metadata1 Nonfungible(Metadata1.NonfungibleInfo info)
		{
			return new Metadata1(Metadata1Tag.Nonfungible, info);
		}

		public Metadata1.FungibleInfo AsFungible()
		{
			this.ValidateTag(Metadata1Tag.Fungible);
			return (Metadata1.FungibleInfo)this.Value!;
		}

		public Metadata1.NonfungibleInfo AsNonfungible()
		{
			this.ValidateTag(Metadata1Tag.Nonfungible);
			return (Metadata1.NonfungibleInfo)this.Value!;
		}

		private void ValidateTag(Metadata1Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class FungibleInfo
		{
			[CandidName("decimals")]
			public byte Decimals { get; set; }

			[CandidName("metadata")]
			public OptionalValue<Metadatacontainer1> Metadata { get; set; }

			[CandidName("name")]
			public string Name { get; set; }

			[CandidName("symbol")]
			public string Symbol { get; set; }

			public FungibleInfo(byte decimals, OptionalValue<Metadatacontainer1> metadata, string name, string symbol)
			{
				this.Decimals = decimals;
				this.Metadata = metadata;
				this.Name = name;
				this.Symbol = symbol;
			}

			public FungibleInfo()
			{
			}
		}

		public class NonfungibleInfo
		{
			[CandidName("asset")]
			public string Asset { get; set; }

			[CandidName("description")]
			public string Description { get; set; }

			[CandidName("metadata")]
			public OptionalValue<Metadatacontainer1> Metadata { get; set; }

			[CandidName("name")]
			public string Name { get; set; }

			[CandidName("thumbnail")]
			public string Thumbnail { get; set; }

			public NonfungibleInfo(string asset, string description, OptionalValue<Metadatacontainer1> metadata, string name, string thumbnail)
			{
				this.Asset = asset;
				this.Description = description;
				this.Metadata = metadata;
				this.Name = name;
				this.Thumbnail = thumbnail;
			}

			public NonfungibleInfo()
			{
			}
		}
	}

	public enum Metadata1Tag
	{
		[CandidName("fungible")]
		Fungible,
		[CandidName("nonfungible")]
		Nonfungible
	}
}