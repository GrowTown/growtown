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
	public class Result
	{
		[VariantTagProperty]
		public ResultTag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result(ResultTag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result()
		{
		}

		public static Result Err(CommonError info)
		{
			return new Result(ResultTag.Err, info);
		}

		public static Result Ok(Result.OkInfo info)
		{
			return new Result(ResultTag.Ok, info);
		}

		public CommonError AsErr()
		{
			this.ValidateTag(ResultTag.Err);
			return (CommonError)this.Value!;
		}

		public Result.OkInfo AsOk()
		{
			this.ValidateTag(ResultTag.Ok);
			return (Result.OkInfo)this.Value!;
		}

		private void ValidateTag(ResultTag tag)
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
			public Result.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result.OkInfo.DataInfo.DataInfoElement>
			{
				public DataInfo()
				{
				}

				public class DataInfoElement
				{
					[CandidTag(0U)]
					public TokenIdentifier F0 { get; set; }

					[CandidTag(1U)]
					public TokenIndex F1 { get; set; }

					[CandidTag(2U)]
					public Metadata F2 { get; set; }

					[CandidTag(3U)]
					public string F3 { get; set; }

					[CandidTag(4U)]
					public Principal F4 { get; set; }

					[CandidTag(5U)]
					public OptionalValue<ulong> F5 { get; set; }

					public DataInfoElement(TokenIdentifier f0, TokenIndex f1, Metadata f2, string f3, Principal f4, OptionalValue<ulong> f5)
					{
						this.F0 = f0;
						this.F1 = f1;
						this.F2 = f2;
						this.F3 = f3;
						this.F4 = f4;
						this.F5 = f5;
					}

					public DataInfoElement()
					{
					}
				}
			}
		}
	}

	public enum ResultTag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}