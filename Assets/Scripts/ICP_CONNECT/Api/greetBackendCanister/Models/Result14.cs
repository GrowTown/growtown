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
	public class Result14
	{
		[VariantTagProperty]
		public Result14Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result14(Result14Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result14()
		{
		}

		public static Result14 Err(string info)
		{
			return new Result14(Result14Tag.Err, info);
		}

		public static Result14 Ok(Result14.OkInfo info)
		{
			return new Result14(Result14Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result14Tag.Err);
			return (string)this.Value!;
		}

		public Result14.OkInfo AsOk()
		{
			this.ValidateTag(Result14Tag.Ok);
			return (Result14.OkInfo)this.Value!;
		}

		private void ValidateTag(Result14Tag tag)
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
			public Result14.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result14.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result14.OkInfo.DataInfo.DataInfoElement>
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

					[CandidTag(4U)]
					public UnboundedUInt F4 { get; set; }

					public DataInfoElement(TokenIndex f0, TokenIdentifier f1, Listing f2, Metadata f3, UnboundedUInt f4)
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

	public enum Result14Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}