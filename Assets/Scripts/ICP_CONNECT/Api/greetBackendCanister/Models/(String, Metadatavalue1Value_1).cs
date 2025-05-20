using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System.Collections.Generic;
using EdjCase.ICP.Candid.Models;
using System;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Metadatavalue1Value_1
	{
		[VariantTagProperty]
		public Metadatavalue1Value_1Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Metadatavalue1Value_1(Metadatavalue1Value_1Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Metadatavalue1Value_1()
		{
		}

		public static Metadatavalue1Value_1 Blob(List<byte> info)
		{
			return new Metadatavalue1Value_1(Metadatavalue1Value_1Tag.Blob, info);
		}

		public static Metadatavalue1Value_1 Nat(UnboundedUInt info)
		{
			return new Metadatavalue1Value_1(Metadatavalue1Value_1Tag.Nat, info);
		}

		public static Metadatavalue1Value_1 Nat8(byte info)
		{
			return new Metadatavalue1Value_1(Metadatavalue1Value_1Tag.Nat8, info);
		}

		public static Metadatavalue1Value_1 Text(string info)
		{
			return new Metadatavalue1Value_1(Metadatavalue1Value_1Tag.Text, info);
		}

		public List<byte> AsBlob()
		{
			this.ValidateTag(Metadatavalue1Value_1Tag.Blob);
			return (List<byte>)this.Value!;
		}

		public UnboundedUInt AsNat()
		{
			this.ValidateTag(Metadatavalue1Value_1Tag.Nat);
			return (UnboundedUInt)this.Value!;
		}

		public byte AsNat8()
		{
			this.ValidateTag(Metadatavalue1Value_1Tag.Nat8);
			return (byte)this.Value!;
		}

		public string AsText()
		{
			this.ValidateTag(Metadatavalue1Value_1Tag.Text);
			return (string)this.Value!;
		}

		private void ValidateTag(Metadatavalue1Value_1Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}
	}

	public enum Metadatavalue1Value_1Tag
	{
		[CandidName("blob")]
		Blob,
		[CandidName("nat")]
		Nat,
		[CandidName("nat8")]
		Nat8,
		[CandidName("text")]
		Text
	}
}