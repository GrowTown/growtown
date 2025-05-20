using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using System.Collections.Generic;
using TokenIdentifier = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result10
	{
		[VariantTagProperty]
		public Result10Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result10(Result10Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result10()
		{
		}

		public static Result10 Err(CommonError info)
		{
			return new Result10(Result10Tag.Err, info);
		}

		public static Result10 Ok(Result10.OkInfo info)
		{
			return new Result10(Result10Tag.Ok, info);
		}

		public CommonError AsErr()
		{
			this.ValidateTag(Result10Tag.Err);
			return (CommonError)this.Value!;
		}

		public Result10.OkInfo AsOk()
		{
			this.ValidateTag(Result10Tag.Ok);
			return (Result10.OkInfo)this.Value!;
		}

		private void ValidateTag(Result10Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class OkInfo : List<TokenIdentifier>
		{
			public OkInfo()
			{
			}
		}
	}

	public enum Result10Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}