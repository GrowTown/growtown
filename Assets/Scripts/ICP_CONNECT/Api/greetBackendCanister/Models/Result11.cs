using EdjCase.ICP.Candid.Mapping;
using InternetClients.greetBackendCanister.Models;
using System;
using EdjCase.ICP.Candid.Models;
using System.Collections.Generic;
using Time1 = EdjCase.ICP.Candid.Models.UnboundedInt;

namespace InternetClients.greetBackendCanister.Models
{
	[Variant]
	public class Result11
	{
		[VariantTagProperty]
		public Result11Tag Tag { get; set; }

		[VariantValueProperty]
		public object? Value { get; set; }

		public Result11(Result11Tag tag, object? value)
		{
			this.Tag = tag;
			this.Value = value;
		}

		protected Result11()
		{
		}

		public static Result11 Err(string info)
		{
			return new Result11(Result11Tag.Err, info);
		}

		public static Result11 Ok(Result11.OkInfo info)
		{
			return new Result11(Result11Tag.Ok, info);
		}

		public string AsErr()
		{
			this.ValidateTag(Result11Tag.Err);
			return (string)this.Value!;
		}

		public Result11.OkInfo AsOk()
		{
			this.ValidateTag(Result11Tag.Ok);
			return (Result11.OkInfo)this.Value!;
		}

		private void ValidateTag(Result11Tag tag)
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
			public Result11.OkInfo.DataInfo Data { get; set; }

			[CandidName("total_pages")]
			public UnboundedUInt TotalPages { get; set; }

			public OkInfo(UnboundedUInt currentPage, Result11.OkInfo.DataInfo data, UnboundedUInt totalPages)
			{
				this.CurrentPage = currentPage;
				this.Data = data;
				this.TotalPages = totalPages;
			}

			public OkInfo()
			{
			}

			public class DataInfo : List<Result11.OkInfo.DataInfo.DataInfoElement>
			{
				public DataInfo()
				{
				}

				public class DataInfoElement
				{
					[CandidTag(0U)]
					public Principal F0 { get; set; }

					[CandidTag(1U)]
					public UnboundedUInt F1 { get; set; }

					[CandidTag(2U)]
					public Time1 F2 { get; set; }

					[CandidTag(3U)]
					public string F3 { get; set; }

					[CandidTag(4U)]
					public string F4 { get; set; }

					[CandidTag(5U)]
					public OptionalValue<string> F5 { get; set; }

					public DataInfoElement(Principal f0, UnboundedUInt f1, Time1 f2, string f3, string f4, OptionalValue<string> f5)
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

	public enum Result11Tag
	{
		[CandidName("err")]
		Err,
		[CandidName("ok")]
		Ok
	}
}