using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using AccountIdentifier = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result6
	{
		[VariantTagProperty]
		public Result6Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result6(Result6Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result6()
		{
		}

		public static Result6 Err(CommonError info)
		{
			return new Result6(Result6Tag.Err, info);
		}

		public static Result6 Ok((AccountIdentifier, ulong) info)
		{
			return new Result6(Result6Tag.Ok, info);
		}

		public CommonError AsErr()
		{
			this.ValidateTag(Result6Tag.Err);
			return (CommonError)this.Value!;
		}

		public (AccountIdentifier, ulong) AsOk()
		{
			this.ValidateTag(Result6Tag.Ok);
			return ((AccountIdentifier, ulong))this.Value!;
		}

		private void ValidateTag(Result6Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}
	}

	public enum Result6Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}