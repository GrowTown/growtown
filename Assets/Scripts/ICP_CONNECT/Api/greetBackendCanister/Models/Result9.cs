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
	public class Result9
	{
		[VariantTagProperty]
		public Result9Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result9(Result9Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result9()
		{
		}

		public static Result9 Err(string info)
		{
			return new Result9(Result9Tag.Err, info);
		}

		public static Result9 Ok(Result9.OkInfo info)
		{
			return new Result9(Result9Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result9Tag.Err);
			return (string)this.Value!;
		}

		public Result9.OkInfo AsOk()
		{
			this.ValidateTag(Result9Tag.Ok);
			return (Result9.OkInfo)this.Value!;
		}

		private void ValidateTag(Result9Tag tag)
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
			public Result9.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result9.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result9.OkInfo.DataInfo.DataInfoElement>
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

					[CandidTag(4U)]
					public UnboundedUInt F4 { get; set; }

					public DataInfoElement(TokenIndex f0, AccountIdentifier f1, Metadata1 f2, OptionalValue<ulong> f3, UnboundedUInt f4)
					{
						this.F0 = f0;
						this.F1 = f1;
						this.F2 = f2;
						this.F3 = f3;
						this.F4 = f4;
					}

					public DataInfoElement()
					{
					}
				}
			}
		}
	}

	public enum Result9Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}