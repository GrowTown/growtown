using EdjCase.ICP.Candid.Mapping;
using InternetClients.icrc2TokenCanister.Models;
using EdjCase.ICP.Candid.Models;
using System.Collections.Generic;
using SubAccount = System.Collections.Generic.List<System.Byte>;
using Memo = System.UInt64;
using Icrc1Tokens = EdjCase.ICP.Candid.Models.UnboundedUInt;

namespace InternetClients.icrc2TokenCanister.Models
{
	public class ApproveArgs
	{
		[CandidName("from_subaccount")]
		public ApproveArgs.FromSubaccountInfo FromSubaccount { get; set; }

		[CandidName("spender")]
		public Account Spender { get; set; }

		[CandidName("amount")]
		public Icrc1Tokens Amount { get; set; }

		[CandidName("expected_allowance")]
		public ApproveArgs.ExpectedAllowanceInfo ExpectedAllowance { get; set; }

		[CandidName("expires_at")]
		public OptionalValue<TimeStamp> ExpiresAt { get; set; }

		[CandidName("fee")]
		public ApproveArgs.FeeInfo Fee { get; set; }

		[CandidName("memo")]
		public OptionalValue<List<byte>> Memo { get; set; }

		[CandidName("created_at_time")]
		public OptionalValue<TimeStamp> CreatedAtTime { get; set; }

		public ApproveArgs(ApproveArgs.FromSubaccountInfo fromSubaccount, Account spender, Icrc1Tokens amount, ApproveArgs.ExpectedAllowanceInfo expectedAllowance, OptionalValue<TimeStamp> expiresAt, ApproveArgs.FeeInfo fee, OptionalValue<List<byte>> memo, OptionalValue<TimeStamp> createdAtTime)
		{
			this.FromSubaccount = fromSubaccount;
			this.Spender = spender;
			this.Amount = amount;
			this.ExpectedAllowance = expectedAllowance;
			this.ExpiresAt = expiresAt;
			this.Fee = fee;
			this.Memo = memo;
			this.CreatedAtTime = createdAtTime;
		}

		public ApproveArgs()
		{
		}

		public class FromSubaccountInfo : OptionalValue<SubAccount>
		{
			public FromSubaccountInfo()
			{
			}

			public FromSubaccountInfo(SubAccount value) : base(value)
			{
			}
		}

		public class ExpectedAllowanceInfo : OptionalValue<Icrc1Tokens>
		{
			public ExpectedAllowanceInfo()
			{
			}

			public ExpectedAllowanceInfo(Icrc1Tokens value) : base(value)
			{
			}
		}

		public class FeeInfo : OptionalValue<Icrc1Tokens>
		{
			public FeeInfo()
			{
			}

			public FeeInfo(Icrc1Tokens value) : base(value)
			{
			}
		}
	}
}