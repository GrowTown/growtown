using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result4
	{
		[VariantTagProperty]
		public Result4Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result4(Result4Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result4()
		{
		}

		public static Result4 Err(CommonError info)
		{
			return new Result4(Result4Tag.Err, info);
		}

		public static Result4 Ok(string info)
		{
			return new Result4(Result4Tag.Ok, info);
		}

		public CommonError AsErr()
		{
			this.ValidateTag(Result4Tag.Err);
			return (CommonError)this.Value!;
		}

		public string AsOk()
		{
			this.ValidateTag(Result4Tag.Ok);
			return (string)this.Value!;
		}

		private void ValidateTag(Result4Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}
	}

	public enum Result4Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}