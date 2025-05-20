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
	public class Result15
	{
		[VariantTagProperty]
		public Result15Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result15(Result15Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result15()
		{
		}

		public static Result15 Err(string info)
		{
			return new Result15(Result15Tag.Err, info);
		}

		public static Result15 Ok(Result15.OkInfo info)
		{
			return new Result15(Result15Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result15Tag.Err);
			return (string)this.Value!;
		}

		public Result15.OkInfo AsOk()
		{
			this.ValidateTag(Result15Tag.Ok);
			return (Result15.OkInfo)this.Value!;
		}

		private void ValidateTag(Result15Tag tag)
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
			public Result15.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result15.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result15.OkInfo.DataInfo.DataInfoElement>
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
					public Transaction F2 { get; set; }

					[CandidTag(3U)]
					public string F3 { get; set; }

					public DataInfoElement(TokenIndex f0, TokenIdentifier f1, Transaction f2, string f3)
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

	public enum Result15Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}