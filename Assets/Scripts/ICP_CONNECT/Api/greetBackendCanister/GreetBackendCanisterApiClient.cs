using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid;
using System.Threading.Tasks;
using InternetClients.greetBackendCanister;
using System.Collections.Generic;
using EdjCase.ICP.Agent.Responses;
using EdjCase.ICP.Candid.Mapping;
using TokenIndex = System.UInt32;
using TokenIdentifier = System.String;
using Time1 = EdjCase.ICP.Candid.Models.UnboundedInt;
using SubAccount = System.Collections.Generic.List<System.Byte>;
using AccountIdentifier = System.String;

namespace InternetClients.greetBackendCanister
{
	public class GreetBackendCanisterApiClient
	{
		public IAgent Agent { get; }
		public Principal CanisterId { get; }
		public CandidConverter? Converter { get; }

		public GreetBackendCanisterApiClient(IAgent agent, Principal canisterId, CandidConverter? converter = default)
		{
			this.Agent = agent;
			this.CanisterId = canisterId;
			this.Converter = converter;
		}

		public async Task<string> AddObject(Models.NftTypeMetadata arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "addObject", arg);
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<Models.Result4> AddToFavorites(AccountIdentifier arg0, TokenIdentifier arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "addToFavorites", arg);
			return reply.ToObjects<Models.Result4>(this.Converter);
		}

		public async Task<string> AddCollectionToMap(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "add_collection_to_map", arg);
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task AllSettelment(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			await this.Agent.CallAsync(this.CanisterId, "all_settelment", arg);
		}

		public async Task<Models.Result16> Alltransactions(UnboundedUInt arg0, UnboundedUInt arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "alltransactions", arg);
			return reply.ToObjects<Models.Result16>(this.Converter);
		}

		public async Task<Models.Result15> Alluseractivity(AccountIdentifier arg0, UnboundedUInt arg1, UnboundedUInt arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "alluseractivity", arg);
			return reply.ToObjects<Models.Result15>(this.Converter);
		}

		public async Task BalanceNftSettelment(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			await this.Agent.CallAsync(this.CanisterId, "balance_nft_settelment", arg);
		}

		public async Task BalanceSettelment(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			await this.Agent.CallAsync(this.CanisterId, "balance_settelment", arg);
		}

		public async Task<Models.Result14> Countlistings(Principal arg0, UnboundedUInt arg1, UnboundedUInt arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "countlistings", arg);
			return reply.ToObjects<Models.Result14>(this.Converter);
		}

		public async Task<(Principal ReturnArg0, Principal ReturnArg1)> CreateExtCollection(string arg0, string arg1, string arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "createExtCollection", arg);
			return reply.ToObjects<Principal, Principal>(this.Converter);
		}

		public async Task<Models.Result13> CreateUser(Principal arg0, string arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "create_user", arg);
			return reply.ToObjects<Models.Result13>(this.Converter);
		}

		public async Task<UnboundedUInt> FetchCycles()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "fetchCycles", arg);
			return reply.ToObjects<UnboundedUInt>(this.Converter);
		}

		public async Task<OptionalValue<ulong>> FindCost(string arg0, UnboundedUInt arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "findCost", arg);
			return reply.ToObjects<OptionalValue<ulong>>(this.Converter);
		}

		public async Task<Models.Result12> GetAllCollectionNFTs(Principal arg0, UnboundedUInt arg1, UnboundedUInt arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getAllCollectionNFTs", arg);
			return reply.ToObjects<Models.Result12>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0> GetAllCollections()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getAllCollections", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0>(this.Converter);
		}

		public async Task<List<string>> GetAllNFTNames()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getAllNFTNames", arg);
			return reply.ToObjects<List<string>>(this.Converter);
		}

		public async Task<Models.Result11> GetAllUsers(UnboundedUInt arg0, UnboundedUInt arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getAllUsers", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.Result11>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0> GetCollectionAndNFTNames()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getCollectionAndNFTNames", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0>(this.Converter);
		}

		public async Task<List<Models.Deposit>> GetDeposits()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getDeposits", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<List<Models.Deposit>>(this.Converter);
		}

		public async Task<Models.Result10> GetFavorites(AccountIdentifier arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getFavorites", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.Result10>(this.Converter);
		}

		public async Task<Models.Result9> GetFilteredCollectionNFTs(Principal arg0, UnboundedUInt arg1, UnboundedUInt arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getFilteredCollectionNFTs", arg);
			return reply.ToObjects<Models.Result9>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetFungibleTokensReturnArg0> GetFungibleTokens(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getFungibleTokens", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetFungibleTokensReturnArg0>(this.Converter);
		}

		public async Task<TokenIdentifier> GetNftTokenId(Principal arg0, TokenIndex arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getNftTokenId", arg);
			return reply.ToObjects<TokenIdentifier>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetNonFungibleTokensReturnArg0> GetNonFungibleTokens(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getNonFungibleTokens", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetNonFungibleTokensReturnArg0>(this.Converter);
		}

		public async Task<Dictionary<UnboundedUInt, Models.NftTypeMetadata>> GetObjects()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getObjects", arg);
			return reply.ToObjects<Dictionary<UnboundedUInt, Models.NftTypeMetadata>>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetObjectsAsPairsReturnArg0> GetObjectsAsPairs()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getObjectsAsPairs", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetObjectsAsPairsReturnArg0>(this.Converter);
		}

		public async Task<string> GetPrincipal()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getPrincipal", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetRedeemedTokensReturnArg0> GetRedeemedTokens(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getRedeemedTokens", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<GreetBackendCanisterApiClient.GetRedeemedTokensReturnArg0>(this.Converter);
		}

		public async Task<(GreetBackendCanisterApiClient.GetSingleCollectionDetailsReturnArg0 ReturnArg0, UnboundedUInt ReturnArg1)> GetSingleCollectionDetails(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getSingleCollectionDetails", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetSingleCollectionDetailsReturnArg0, UnboundedUInt>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetSingleNonFungibleTokensReturnArg0> GetSingleNonFungibleTokens(Principal arg0, TokenIndex arg1, AccountIdentifier arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getSingleNonFungibleTokens", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetSingleNonFungibleTokensReturnArg0>(this.Converter);
		}

		public async Task<UnboundedUInt> GetTotalNFTs()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getTotalNFTs", arg);
			return reply.ToObjects<UnboundedUInt>(this.Converter);
		}

		public async Task<UnboundedUInt> GetTotalUsers()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getTotalUsers", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<UnboundedUInt>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetUserCollectionDetailsReturnArg0> GetUserCollectionDetails()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getUserCollectionDetails", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.GetUserCollectionDetailsReturnArg0>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetUserCollectionsReturnArg0> GetUserCollections()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getUserCollections", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<GreetBackendCanisterApiClient.GetUserCollectionsReturnArg0>(this.Converter);
		}

		public async Task<Models.Result8> GetUserDetails(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getUserDetails", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<Models.Result8>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.GetallRedeemedTokensReturnArg0> GetallRedeemedTokens()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getallRedeemedTokens", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<GreetBackendCanisterApiClient.GetallRedeemedTokensReturnArg0>(this.Converter);
		}

		public async Task<string> Greet()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "greet", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<List<Models.SupportedStandard>> Icrc10SupportedStandards()
		{
			CandidArg arg = CandidArg.FromCandid();
			QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "icrc10_supported_standards", arg);
			CandidArg reply = response.ThrowOrGetReply();
			return reply.ToObjects<List<Models.SupportedStandard>>(this.Converter);
		}

		public async Task<Models.Icrc28TrustedOriginsResponse> Icrc28TrustedOrigins()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "icrc28_trusted_origins", arg);
			return reply.ToObjects<Models.Icrc28TrustedOriginsResponse>(this.Converter);
		}

		public async Task<bool> IsController(Principal arg0, Principal arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "isController", arg);
			return reply.ToObjects<bool>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.ListingsReturnArg0> Listings(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "listings", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.ListingsReturnArg0>(this.Converter);
		}

		public async Task<Models.Result3> Listprice(Principal arg0, Models.ListRequest arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "listprice", arg);
			return reply.ToObjects<Models.Result3>(this.Converter);
		}

		public async Task<(ulong ReturnArg0, ulong ReturnArg1, ulong ReturnArg2, ulong ReturnArg3, UnboundedUInt ReturnArg4, UnboundedUInt ReturnArg5, UnboundedUInt ReturnArg6)> Marketstats(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "marketstats", arg);
			return reply.ToObjects<ulong, ulong, ulong, ulong, UnboundedUInt, UnboundedUInt, UnboundedUInt>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.MintExtFungibleReturnArg0> MintExtFungible(Principal arg0, string arg1, string arg2, byte arg3, OptionalValue<Models.MetadataContainer> arg4, UnboundedUInt arg5)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter), CandidTypedValue.FromObject(arg4, this.Converter), CandidTypedValue.FromObject(arg5, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "mintExtFungible", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.MintExtFungibleReturnArg0>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.MintExtNonFungibleReturnArg0> MintExtNonFungible(Principal arg0, string arg1, string arg2, string arg3, string arg4, OptionalValue<Models.MetadataContainer> arg5, UnboundedUInt arg6)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter), CandidTypedValue.FromObject(arg4, this.Converter), CandidTypedValue.FromObject(arg5, this.Converter), CandidTypedValue.FromObject(arg6, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "mintExtNonFungible", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.MintExtNonFungibleReturnArg0>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.MintExtNonFungible3ReturnArg0> MintExtNonFungible3(Principal arg0, string arg1, string arg2, string arg3, string arg4, OptionalValue<Models.MetadataContainer> arg5, UnboundedUInt arg6, OptionalValue<ulong> arg7)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter), CandidTypedValue.FromObject(arg4, this.Converter), CandidTypedValue.FromObject(arg5, this.Converter), CandidTypedValue.FromObject(arg6, this.Converter), CandidTypedValue.FromObject(arg7, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "mintExtNonFungible3", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.MintExtNonFungible3ReturnArg0>(this.Converter);
		}

		public async Task<Models.Result7> Plistings(Principal arg0, UnboundedUInt arg1, UnboundedUInt arg2)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "plistings", arg);
			return reply.ToObjects<Models.Result7>(this.Converter);
		}

		public async Task<Models.Result6> PurchaseNft(Principal arg0, TokenIdentifier arg1, ulong arg2, AccountIdentifier arg3)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "purchaseNft", arg);
			return reply.ToObjects<Models.Result6>(this.Converter);
		}

		public async Task<Models.Result5> Redeemtoken(Principal arg0, TokenIdentifier arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "redeemtoken", arg);
			return reply.ToObjects<Models.Result5>(this.Converter);
		}

		public async Task<string> RemoveCollection(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "removeCollection", arg);
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<Models.Result4> RemoveFromFavorites(AccountIdentifier arg0, TokenIdentifier arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "removeFromFavorites", arg);
			return reply.ToObjects<Models.Result4>(this.Converter);
		}

		public async Task<string> RemoveObject(UnboundedUInt arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "removeObject", arg);
			return reply.ToObjects<string>(this.Converter);
		}

		public async Task<Models.Result1> RemoveUser(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "remove_user", arg);
			return reply.ToObjects<Models.Result1>(this.Converter);
		}

		public async Task<Models.Result2> SendBalanceAndNft(Principal arg0, AccountIdentifier arg1, ulong arg2, GreetBackendCanisterApiClient.SendBalanceAndNftArg3 arg3)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "send_balance_and_nft", arg);
			return reply.ToObjects<Models.Result2>(this.Converter);
		}

		public async Task<Models.Result3> Settlepurchase(Principal arg0, AccountIdentifier arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "settlepurchase", arg);
			return reply.ToObjects<Models.Result3>(this.Converter);
		}

		public async Task<UnboundedUInt> Totalcollections()
		{
			CandidArg arg = CandidArg.FromCandid();
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "totalcollections", arg);
			return reply.ToObjects<UnboundedUInt>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.TransactionsReturnArg0> Transactions(Principal arg0)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "transactions", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.TransactionsReturnArg0>(this.Converter);
		}

		public async Task<Models.Result2> TransferBalance(Principal arg0, AccountIdentifier arg1, ulong arg2, GreetBackendCanisterApiClient.TransferBalanceArg3 arg3)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "transfer_balance", arg);
			return reply.ToObjects<Models.Result2>(this.Converter);
		}

		public async Task<Models.Result1> UpdateUserDetails(Principal arg0, string arg1, string arg2, string arg3, OptionalValue<string> arg4)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter), CandidTypedValue.FromObject(arg4, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "updateUserDetails", arg);
			return reply.ToObjects<Models.Result1>(this.Converter);
		}

		public async Task<Models.Result> UserNFTcollection(Principal arg0, AccountIdentifier arg1, UnboundedUInt arg2, UnboundedUInt arg3)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter), CandidTypedValue.FromObject(arg2, this.Converter), CandidTypedValue.FromObject(arg3, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "userNFTcollection", arg);
			return reply.ToObjects<Models.Result>(this.Converter);
		}

		public async Task<GreetBackendCanisterApiClient.UseractivityReturnArg0> Useractivity(Principal arg0, AccountIdentifier arg1)
		{
			CandidArg arg = CandidArg.FromCandid(CandidTypedValue.FromObject(arg0, this.Converter), CandidTypedValue.FromObject(arg1, this.Converter));
			CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "useractivity", arg);
			return reply.ToObjects<GreetBackendCanisterApiClient.UseractivityReturnArg0>(this.Converter);
		}

		public class GetAllCollectionsReturnArg0 : List<GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0.GetAllCollectionsReturnArg0Element>
		{
			public GetAllCollectionsReturnArg0()
			{
			}

			public class GetAllCollectionsReturnArg0Element
			{
				[CandidTag(0U)]
				public Principal F0 { get; set; }

				[CandidTag(1U)]
				public GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0.GetAllCollectionsReturnArg0Element.F1Info F1 { get; set; }

				public GetAllCollectionsReturnArg0Element(Principal f0, GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0.GetAllCollectionsReturnArg0Element.F1Info f1)
				{
					this.F0 = f0;
					this.F1 = f1;
				}

				public GetAllCollectionsReturnArg0Element()
				{
				}

				public class F1Info : List<GreetBackendCanisterApiClient.GetAllCollectionsReturnArg0.GetAllCollectionsReturnArg0Element.F1Info.F1InfoElement>
				{
					public F1Info()
					{
					}

					public class F1InfoElement
					{
						[CandidTag(0U)]
						public Time1 F0 { get; set; }

						[CandidTag(1U)]
						public Principal F1 { get; set; }

						[CandidTag(2U)]
						public string F2 { get; set; }

						[CandidTag(3U)]
						public string F3 { get; set; }

						[CandidTag(4U)]
						public string F4 { get; set; }

						public F1InfoElement(Time1 f0, Principal f1, string f2, string f3, string f4)
						{
							this.F0 = f0;
							this.F1 = f1;
							this.F2 = f2;
							this.F3 = f3;
							this.F4 = f4;
						}

						public F1InfoElement()
						{
						}
					}
				}
			}
		}

		public class GetCollectionAndNFTNamesReturnArg0 : List<GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0.GetCollectionAndNFTNamesReturnArg0Element>
		{
			public GetCollectionAndNFTNamesReturnArg0()
			{
			}

			public class GetCollectionAndNFTNamesReturnArg0Element
			{
				[CandidTag(0U)]
				public string F0 { get; set; }

				[CandidTag(1U)]
				public GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0.GetCollectionAndNFTNamesReturnArg0Element.F1Info F1 { get; set; }

				public GetCollectionAndNFTNamesReturnArg0Element(string f0, GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0.GetCollectionAndNFTNamesReturnArg0Element.F1Info f1)
				{
					this.F0 = f0;
					this.F1 = f1;
				}

				public GetCollectionAndNFTNamesReturnArg0Element()
				{
				}

				public class F1Info : List<GreetBackendCanisterApiClient.GetCollectionAndNFTNamesReturnArg0.GetCollectionAndNFTNamesReturnArg0Element.F1Info.F1InfoElement>
				{
					public F1Info()
					{
					}

					public class F1InfoElement
					{
						[CandidTag(0U)]
						public Time1 F0 { get; set; }

						[CandidTag(1U)]
						public Principal F1 { get; set; }

						[CandidTag(2U)]
						public Dictionary<string, List<string>> F2 { get; set; }

						public F1InfoElement(Time1 f0, Principal f1, Dictionary<string, List<string>> f2)
						{
							this.F0 = f0;
							this.F1 = f1;
							this.F2 = f2;
						}

						public F1InfoElement()
						{
						}
					}
				}
			}
		}

		public class GetFungibleTokensReturnArg0 : List<GreetBackendCanisterApiClient.GetFungibleTokensReturnArg0.GetFungibleTokensReturnArg0Element>
		{
			public GetFungibleTokensReturnArg0()
			{
			}

			public class GetFungibleTokensReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public AccountIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Metadata1 F2 { get; set; }

				public GetFungibleTokensReturnArg0Element(TokenIndex f0, AccountIdentifier f1, Models.Metadata1 f2)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
				}

				public GetFungibleTokensReturnArg0Element()
				{
				}
			}
		}

		public class GetNonFungibleTokensReturnArg0 : List<GreetBackendCanisterApiClient.GetNonFungibleTokensReturnArg0.GetNonFungibleTokensReturnArg0Element>
		{
			public GetNonFungibleTokensReturnArg0()
			{
			}

			public class GetNonFungibleTokensReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public AccountIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Metadata1 F2 { get; set; }

				public GetNonFungibleTokensReturnArg0Element(TokenIndex f0, AccountIdentifier f1, Models.Metadata1 f2)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
				}

				public GetNonFungibleTokensReturnArg0Element()
				{
				}
			}
		}

		public class GetObjectsAsPairsReturnArg0
		{
			[CandidName("nft_type_quantity")]
			public List<UnboundedUInt> NftTypeQuantity { get; set; }

			[CandidName("nfttype")]
			public List<string> Nfttype { get; set; }

			public GetObjectsAsPairsReturnArg0(List<UnboundedUInt> nftTypeQuantity, List<string> nfttype)
			{
				this.NftTypeQuantity = nftTypeQuantity;
				this.Nfttype = nfttype;
			}

			public GetObjectsAsPairsReturnArg0()
			{
			}
		}

		public class GetRedeemedTokensReturnArg0 : List<GreetBackendCanisterApiClient.GetRedeemedTokensReturnArg0.GetRedeemedTokensReturnArg0Element>
		{
			public GetRedeemedTokensReturnArg0()
			{
			}

			public class GetRedeemedTokensReturnArg0Element
			{
				[CandidTag(0U)]
				public Principal F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				public GetRedeemedTokensReturnArg0Element(Principal f0, TokenIdentifier f1)
				{
					this.F0 = f0;
					this.F1 = f1;
				}

				public GetRedeemedTokensReturnArg0Element()
				{
				}
			}
		}

		public class GetSingleCollectionDetailsReturnArg0 : List<GreetBackendCanisterApiClient.GetSingleCollectionDetailsReturnArg0.GetSingleCollectionDetailsReturnArg0Element>
		{
			public GetSingleCollectionDetailsReturnArg0()
			{
			}

			public class GetSingleCollectionDetailsReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public AccountIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Metadata1 F2 { get; set; }

				public GetSingleCollectionDetailsReturnArg0Element(TokenIndex f0, AccountIdentifier f1, Models.Metadata1 f2)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
				}

				public GetSingleCollectionDetailsReturnArg0Element()
				{
				}
			}
		}

		public class GetSingleNonFungibleTokensReturnArg0 : List<GreetBackendCanisterApiClient.GetSingleNonFungibleTokensReturnArg0.GetSingleNonFungibleTokensReturnArg0Element>
		{
			public GetSingleNonFungibleTokensReturnArg0()
			{
			}

			public class GetSingleNonFungibleTokensReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public AccountIdentifier F2 { get; set; }

				[CandidTag(3U)]
				public Models.Metadata F3 { get; set; }

				[CandidTag(4U)]
				public OptionalValue<ulong> F4 { get; set; }

				[CandidTag(5U)]
				public bool F5 { get; set; }

				public GetSingleNonFungibleTokensReturnArg0Element(TokenIndex f0, TokenIdentifier f1, AccountIdentifier f2, Models.Metadata f3, OptionalValue<ulong> f4, bool f5)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
					this.F3 = f3;
					this.F4 = f4;
					this.F5 = f5;
				}

				public GetSingleNonFungibleTokensReturnArg0Element()
				{
				}
			}
		}

		public class GetUserCollectionDetailsReturnArg0 : OptionalValue<GreetBackendCanisterApiClient.GetUserCollectionDetailsReturnArg0.GetUserCollectionDetailsReturnArg0Value>
		{
			public GetUserCollectionDetailsReturnArg0()
			{
			}

			public GetUserCollectionDetailsReturnArg0(GreetBackendCanisterApiClient.GetUserCollectionDetailsReturnArg0.GetUserCollectionDetailsReturnArg0Value value) : base(value)
			{
			}

			public class GetUserCollectionDetailsReturnArg0Value : List<GreetBackendCanisterApiClient.GetUserCollectionDetailsReturnArg0.GetUserCollectionDetailsReturnArg0Value.GetUserCollectionDetailsReturnArg0ValueElement>
			{
				public GetUserCollectionDetailsReturnArg0Value()
				{
				}

				public class GetUserCollectionDetailsReturnArg0ValueElement
				{
					[CandidTag(0U)]
					public Time1 F0 { get; set; }

					[CandidTag(1U)]
					public Principal F1 { get; set; }

					[CandidTag(2U)]
					public string F2 { get; set; }

					[CandidTag(3U)]
					public string F3 { get; set; }

					[CandidTag(4U)]
					public string F4 { get; set; }

					public GetUserCollectionDetailsReturnArg0ValueElement(Time1 f0, Principal f1, string f2, string f3, string f4)
					{
						this.F0 = f0;
						this.F1 = f1;
						this.F2 = f2;
						this.F3 = f3;
						this.F4 = f4;
					}

					public GetUserCollectionDetailsReturnArg0ValueElement()
					{
					}
				}
			}
		}

		public class GetUserCollectionsReturnArg0 : OptionalValue<GreetBackendCanisterApiClient.GetUserCollectionsReturnArg0.GetUserCollectionsReturnArg0Value>
		{
			public GetUserCollectionsReturnArg0()
			{
			}

			public GetUserCollectionsReturnArg0(GreetBackendCanisterApiClient.GetUserCollectionsReturnArg0.GetUserCollectionsReturnArg0Value value) : base(value)
			{
			}

			public class GetUserCollectionsReturnArg0Value : List<GreetBackendCanisterApiClient.GetUserCollectionsReturnArg0.GetUserCollectionsReturnArg0Value.GetUserCollectionsReturnArg0ValueElement>
			{
				public GetUserCollectionsReturnArg0Value()
				{
				}

				public class GetUserCollectionsReturnArg0ValueElement
				{
					[CandidTag(0U)]
					public Time1 F0 { get; set; }

					[CandidTag(1U)]
					public Principal F1 { get; set; }

					public GetUserCollectionsReturnArg0ValueElement(Time1 f0, Principal f1)
					{
						this.F0 = f0;
						this.F1 = f1;
					}

					public GetUserCollectionsReturnArg0ValueElement()
					{
					}
				}
			}
		}

		public class GetallRedeemedTokensReturnArg0 : List<GreetBackendCanisterApiClient.GetallRedeemedTokensReturnArg0.GetallRedeemedTokensReturnArg0Element>
		{
			public GetallRedeemedTokensReturnArg0()
			{
			}

			public class GetallRedeemedTokensReturnArg0Element
			{
				[CandidTag(0U)]
				public Principal F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				public GetallRedeemedTokensReturnArg0Element(Principal f0, TokenIdentifier f1)
				{
					this.F0 = f0;
					this.F1 = f1;
				}

				public GetallRedeemedTokensReturnArg0Element()
				{
				}
			}
		}

		public class ListingsReturnArg0 : List<GreetBackendCanisterApiClient.ListingsReturnArg0.ListingsReturnArg0Element>
		{
			public ListingsReturnArg0()
			{
			}

			public class ListingsReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Listing F2 { get; set; }

				[CandidTag(3U)]
				public Models.Metadata F3 { get; set; }

				public ListingsReturnArg0Element(TokenIndex f0, TokenIdentifier f1, Models.Listing f2, Models.Metadata f3)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
					this.F3 = f3;
				}

				public ListingsReturnArg0Element()
				{
				}
			}
		}

		public class MintExtFungibleReturnArg0 : List<TokenIndex>
		{
			public MintExtFungibleReturnArg0()
			{
			}
		}

		public class MintExtNonFungibleReturnArg0 : List<GreetBackendCanisterApiClient.MintExtNonFungibleReturnArg0.MintExtNonFungibleReturnArg0Element>
		{
			public MintExtNonFungibleReturnArg0()
			{
			}

			public class MintExtNonFungibleReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				public MintExtNonFungibleReturnArg0Element(TokenIndex f0, TokenIdentifier f1)
				{
					this.F0 = f0;
					this.F1 = f1;
				}

				public MintExtNonFungibleReturnArg0Element()
				{
				}
			}
		}

		public class MintExtNonFungible3ReturnArg0 : List<GreetBackendCanisterApiClient.MintExtNonFungible3ReturnArg0.MintExtNonFungible3ReturnArg0Element>
		{
			public MintExtNonFungible3ReturnArg0()
			{
			}

			public class MintExtNonFungible3ReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Result3 F2 { get; set; }

				public MintExtNonFungible3ReturnArg0Element(TokenIndex f0, TokenIdentifier f1, Models.Result3 f2)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
				}

				public MintExtNonFungible3ReturnArg0Element()
				{
				}
			}
		}

		public class SendBalanceAndNftArg3 : OptionalValue<SubAccount>
		{
			public SendBalanceAndNftArg3()
			{
			}

			public SendBalanceAndNftArg3(SubAccount value) : base(value)
			{
			}
		}

		public class TransactionsReturnArg0 : List<GreetBackendCanisterApiClient.TransactionsReturnArg0.TransactionsReturnArg0Element>
		{
			public TransactionsReturnArg0()
			{
			}

			public class TransactionsReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Transaction F2 { get; set; }

				public TransactionsReturnArg0Element(TokenIndex f0, TokenIdentifier f1, Models.Transaction f2)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
				}

				public TransactionsReturnArg0Element()
				{
				}
			}
		}

		public class TransferBalanceArg3 : OptionalValue<SubAccount>
		{
			public TransferBalanceArg3()
			{
			}

			public TransferBalanceArg3(SubAccount value) : base(value)
			{
			}
		}

		public class UseractivityReturnArg0 : List<GreetBackendCanisterApiClient.UseractivityReturnArg0.UseractivityReturnArg0Element>
		{
			public UseractivityReturnArg0()
			{
			}

			public class UseractivityReturnArg0Element
			{
				[CandidTag(0U)]
				public TokenIndex F0 { get; set; }

				[CandidTag(1U)]
				public TokenIdentifier F1 { get; set; }

				[CandidTag(2U)]
				public Models.Transaction F2 { get; set; }

				[CandidTag(3U)]
				public string F3 { get; set; }

				public UseractivityReturnArg0Element(TokenIndex f0, TokenIdentifier f1, Models.Transaction f2, string f3)
				{
					this.F0 = f0;
					this.F1 = f1;
					this.F2 = f2;
					this.F3 = f3;
				}

				public UseractivityReturnArg0Element()
				{
				}
			}
		}
	}
}