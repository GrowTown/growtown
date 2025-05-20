using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System.Collections.Generic;
using System;
using Metadatavalue1 = System.ValueTuple<System.String, InternetClients.greetBackendCanister.Models.Metadatavalue1Value_1>;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Metadatacontainer1
	{
		[VariantTagProperty]
		public Metadatacontainer1Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Metadatacontainer1(Metadatacontainer1Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Metadatacontainer1()
		{
		}

		public static Metadatacontainer1 Blob(List<byte> info)
		{
			return new Metadatacontainer1(Metadatacontainer1Tag.Blob, info);
		}

		public static Metadatacontainer1 Data(Metadatacontainer1.DataInfo info)
		{
			return new Metadatacontainer1(Metadatacontainer1Tag.Data, info);
		}

		public static Metadatacontainer1 Json(string info)
		{
			return new Metadatacontainer1(Metadatacontainer1Tag.Json, info);
		}

		public List<byte> AsBlob()
		{
			this.ValidateTag(Metadatacontainer1Tag.Blob);
			return (List<byte>)this.Value!;
		}

		public Metadatacontainer1.DataInfo AsData()
		{
			this.ValidateTag(Metadatacontainer1Tag.Data);
			return (Metadatacontainer1.DataInfo)this.Value!;
		}

		public string AsJson()
		{
			this.ValidateTag(Metadatacontainer1Tag.Json);
			return (string)this.Value!;
		}

		private void ValidateTag(Metadatacontainer1Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class DataInfo : List<Metadatavalue1>
		{
			public DataInfo()
			{
			}
		}
	}

	public enum Metadatacontainer1Tag
	{
		[CandidName("blob")]
		Blob,
		[CandidName("data")]
		Data,
		[CandidName("json")]
		Json
	}
}