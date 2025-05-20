using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using Tokenidentifier1 = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class CommonError
	{
		[VariantTagProperty]
		public CommonErrorTag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public CommonError(CommonErrorTag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected CommonError()
		{
		}

		public static CommonError InvalidToken(Tokenidentifier1 info)
		{
			return new CommonError(CommonErrorTag.InvalidToken, info);
		}

		public static CommonError Other(string info)
		{
			return new CommonError(CommonErrorTag.Other, info);
		}

		public Tokenidentifier1 AsInvalidToken()
		{
			this.ValidateTag(CommonErrorTag.InvalidToken);
			return (Tokenidentifier1)this.Value!;
		}

		public string AsOther()
		{
			this.ValidateTag(CommonErrorTag.Other);
			return (string)this.Value!;
		}

		private void ValidateTag(CommonErrorTag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}
	}

	public enum CommonErrorTag
	{
		InvalidToken,
		Other
	}
}