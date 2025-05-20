using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using EdjCase.ICP.Candid.Models;
using System.Collections.Generic;
using TokenIndex = System.UInt32;
using AccountIdentifier = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result12
	{
		[VariantTagProperty]
		public Result12Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result12(Result12Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result12()
		{
		}

		public static Result12 Err(string info)
		{
			return new Result12(Result12Tag.Err, info);
		}

		public static Result12 Ok(Result12.OkInfo info)
		{
			return new Result12(Result12Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result12Tag.Err);
			return (string)this.Value!;
		}

		public Result12.OkInfo AsOk()
		{
			this.ValidateTag(Result12Tag.Ok);
			return (Result12.OkInfo)this.Value!;
		}

		private void ValidateTag(Result12Tag tag)
		{
			if (!this.Tag.Equals(tag))
			{
				throw new InvalidOperationException($"Cannot cast '{this.Tag}' to type '{tag}'");
			}
		}

		public class OkInfo
		{
			[CandidName("current_page")]
			public UnboundedUInt CurrentPage { get; set; }

			[CandidName("data")]
			public Result12.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result12.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result12.OkInfo.DataInfo.DataInfoElement>
			{
				public DataInfo()
				{
				}

				public class DataInfoElement
				{
					[CandidTag(0U)]
					public TokenIndex F0 { get; set; }

					[CandidTag(1U)]
					public AccountIdentifier F1 { get; set; }

					[CandidTag(2U)]
					public Metadata1 F2 { get; set; }

					[CandidTag(3U)]
					public OptionalValue<ulong> F3 { get; set; }

					public DataInfoElement(TokenIndex f0, AccountIdentifier f1, Metadata1 f2, OptionalValue<ulong> f3)
					{
						this.F0 = f0;
						this.F1 = f1;
						this.F2 = f2;
						this.F3 = f3;
					}

					public DataInfoElement()
					{
					}
				}
			}
		}
	}

	public enum Result12Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}