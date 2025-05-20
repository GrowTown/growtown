using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using EdjCase.ICP.Candid.Models;
using Time1 = EdjCase.ICP.Candid.Models.UnboundedInt;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result13
	{
		[VariantTagProperty]
		public Result13Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result13(Result13Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result13()
		{
		}

		public static Result13 Err(string info)
		{
			return new Result13(Result13Tag.Err, info);
		}

		public static Result13 Ok(Result13.OkInfo info)
		{
			return new Result13(Result13Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result13Tag.Err);
			return (string)this.Value!;
		}

		public Result13.OkInfo AsOk()
		{
			this.ValidateTag(Result13Tag.Ok);
			return (Result13.OkInfo)this.Value!;
		}

		private void ValidateTag(Result13Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class OkInfo
		{
			[CandidTag(0U)]
			public UnboundedUInt F0 { get; set; }

			[CandidTag(1U)]
			public Time1 F1 { get; set; }

			public OkInfo(UnboundedUInt f0, Time1 f1)
			{
				this.F0 = f0;
				this.F1 = f1;
			}

			public OkInfo()
			{
			}
		}
	}

	public enum Result13Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}