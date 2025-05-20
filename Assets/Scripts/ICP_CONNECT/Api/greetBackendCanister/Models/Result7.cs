using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using EdjCase.ICP.Candid.Models;
using System.Collections.Generic;
using TokenIndex = System.UInt32;
using TokenIdentifier = System.String;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result7
	{
		[VariantTagProperty]
		public Result7Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result7(Result7Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result7()
		{
		}

		public static Result7 Err(string info)
		{
			return new Result7(Result7Tag.Err, info);
		}

		public static Result7 Ok(Result7.OkInfo info)
		{
			return new Result7(Result7Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result7Tag.Err);
			return (string)this.Value!;
		}

		public Result7.OkInfo AsOk()
		{
			this.ValidateTag(Result7Tag.Ok);
			return (Result7.OkInfo)this.Value!;
		}

		private void ValidateTag(Result7Tag tag)
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
			public Result7.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result7.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result7.OkInfo.DataInfo.DataInfoElement>
			{
				public DataInfo()
				{
				}

				public class DataInfoElement
				{
					[CandidTag(0U)]
					public TokenIndex F0 { get; set; }

					[CandidTag(1U)]
					public TokenIdentifier F1 { get; set; }

					[CandidTag(2U)]
					public Listing F2 { get; set; }

					[CandidTag(3U)]
					public Metadata F3 { get; set; }

					public DataInfoElement(TokenIndex f0, TokenIdentifier f1, Listing f2, Metadata f3)
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

	public enum Result7Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}