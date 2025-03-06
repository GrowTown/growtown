#nullable enable
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Responses;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid.Models.Types;
using EdjCase.ICP.Candid.Models.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GreetingClient
{
    public class GreetingClient
    {
        public IAgent Agent { get; }
        public Principal CanisterId { get; }
        public EdjCase.ICP.Candid.CandidConverter? Converter { get; }

        public GreetingClient(IAgent agent, Principal canisterId)
        {
            this.CanisterId = canisterId;
            this.Agent = agent;
            this.Converter = null;
        }

        public async Task<string> Greet()
        { 
            try
            {
                Debug.Log("🔄 Sending Greet request...");
                CandidArg arg = CandidArg.FromCandid();
                QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "greet", arg);
                CandidArg reply = response.ThrowOrGetReply();
                string result = reply.ToObjects<string>(this.Converter);
                Debug.Log($"✅ Greeting received: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch greeting: {e.Message}");
                return "Error fetching greeting";
            }
        }
public async Task<string> GetPrincipal() 
{
    CandidArg arg = CandidArg.FromCandid();
    QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getPrincipal", arg);
    CandidArg reply = response.ThrowOrGetReply();
    return reply.ToObjects<string>(this.Converter);
}
   

public async Task<List<(Principal, List<(long, Principal, string, string, string)>)>> GetAllCollections()
        {
            try
            {
                Debug.Log("🔄 Calling getAllCollections...");
                CandidArg arg = CandidArg.FromCandid();
                QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getAllCollections", arg);
                CandidArg reply = response.ThrowOrGetReply();
                var rawResult = reply.ToObjects<List<(Principal, List<(long, Principal, string, string, string)>)>>(this.Converter) 
                    ?? new List<(Principal, List<(long, Principal, string, string, string)>)>();
                Debug.Log($"✅ Successfully retrieved {rawResult.Count} user collections.");
                return rawResult;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch collections: {e.Message}");
                return new List<(Principal, List<(long, Principal, string, string, string)>)>();
            }
        }

        public async Task<(List<(uint, string, object, string, ulong)>, ulong, ulong)> CountListings(Principal collectionCanisterId, ulong chunkSize, ulong pageNo)
        {
            try
            {
                Debug.Log("🔄 Calling countListings...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId),
                    CandidTypedValue.Nat(chunkSize),
                    CandidTypedValue.Nat(pageNo)
                );

              CandidArg reply = await this.Agent.CallAsynchronousAndWaitAsync(this.CanisterId, "countlistings", args);

                // Log the raw response for debugging
                Debug.Log($"✅ Raw response from canister: {reply}");

                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);
                if (result.ContainsKey("err"))
                {
                    Debug.LogWarning($"⚠️ Error from canister: {result["err"]}");
                    return (new List<(uint, string, object, string, ulong)>(), 0, 0);
                }

                var okResult = (Dictionary<string, object>)result["ok"];
                var data = ((List<object>)okResult["data"])
                    .Select(item =>
                    {
                        var tuple = (List<object>)item;
                        return (
                            Convert.ToUInt32(tuple[0]), // TokenIndex
                            tuple[1].ToString()!,       // TokenIdentifier
                            tuple[2],                   // Listing (kept as object)
                            tuple[3].ToString()!,       // Metadata
                            Convert.ToUInt64(tuple[4])  // Price (Nat)
                        );
                    }).ToList();

                ulong currentPage = Convert.ToUInt64(okResult["current_page"]);
                ulong totalPages = Convert.ToUInt64(okResult["total_pages"]);

                Debug.Log($"✅ Listings fetched: {data.Count} on page {currentPage}/{totalPages}");
                return (data, currentPage, totalPages);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch listings: {e.Message}");
                return (new List<(uint, string, object, string, ulong)>(), 0, 0);
            }
        }

        public async Task<(bool success, string? error)> PurchaseNft(Principal collectionCanisterId, string tokenId, ulong price, string buyerAccountId)
        {
            try
            {
                Debug.Log("🔄 Calling purchaseNft...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId),
                    CandidTypedValue.Text(tokenId),
                    CandidTypedValue.Nat64(price),
                    CandidTypedValue.Text(buyerAccountId)
                );

                CandidArg reply = await this.Agent.CallAsynchronousAndWaitAsync(this.CanisterId, "purchaseNft", args);
                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);

                if (result.ContainsKey("ok"))
                {
                    var okResult = (List<object>)result["ok"];
                    Debug.Log("✅ NFT purchase successful");
                    return (true, null);
                }
                
                Debug.LogWarning($"⚠️ Purchase failed: {result["err"]}");
                return (false, result["err"].ToString());
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Purchase NFT failed: {e.Message}");
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string? error)> SettlePurchase(Principal collectionCanisterId, string paymentAddress)
        {
            try
            {
                Debug.Log("🔄 Calling settlepurchase...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId),
                    CandidTypedValue.Text(paymentAddress)
                );

                CandidArg reply = await this.Agent.CallAsynchronousAndWaitAsync(this.CanisterId, "settlepurchase", args);
                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);

                if (result.ContainsKey("ok"))
                {
                    Debug.Log("✅ Purchase settled successfully");
                    return (true, null);
                }
                
                Debug.LogWarning($"⚠️ Settlement failed: {result["err"]}");
                return (false, result["err"].ToString());
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Settlement failed: {e.Message}");
                return (false, e.Message);
            }
        }

        public async Task TriggerBalanceNftSettlement(Principal collectionCanisterId)
        {
            try
            {
                Debug.Log("🔄 Triggering balance_nft_settelment...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId)
                );
                
                await this.Agent.CallAsynchronousAndWaitAsync(this.CanisterId, "balance_nft_settelment", args);
                Debug.Log("✅ Balance settlement triggered successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Balance settlement failed: {e.Message}");
            }
        }

        public async Task<(List<(uint, string, object, string)>, ulong, ulong)> GetAllUserActivity(string buyerId, ulong chunkSize, ulong pageNo)
        {
            try
            {
                Debug.Log("🔄 Calling alluseractivity...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Text(buyerId),
                    CandidTypedValue.Nat(chunkSize),
                    CandidTypedValue.Nat(pageNo)
                );

                QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "alluseractivity", args);
                CandidArg reply = response.ThrowOrGetReply();
                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);

                if (result.ContainsKey("err"))
                {
                    Debug.LogWarning($"⚠️ Error from canister: {result["err"]}");
                    return (new List<(uint, string, object, string)>(), 0, 0);
                }

                var okResult = (Dictionary<string, object>)result["ok"];
                var data = ((List<object>)okResult["data"])
                    .Select(item =>
                    {
                        var tuple = (List<object>)item;
                        return (
                            Convert.ToUInt32(tuple[0]), // TokenIndex
                            tuple[1].ToString()!,       // TokenIdentifier
                            tuple[2],                   // Transaction
                            tuple[3].ToString()!        // CollectionName
                        );
                    }).ToList();

                ulong currentPage = Convert.ToUInt64(okResult["current_page"]);
                ulong totalPages = Convert.ToUInt64(okResult["total_pages"]);

                Debug.Log($"✅ User activities fetched: {data.Count} on page {currentPage}/{totalPages}");
                return (data, currentPage, totalPages);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch user activities: {e.Message}");
                return (new List<(uint, string, object, string)>(), 0, 0);
            }
        }

        public async Task<(List<(string, uint, string, string, Principal, ulong?)> boughtNFTs, 
                          List<(string, uint, string, string, Principal, ulong?)> unboughtNFTs)> 
            GetUserNFTCollection(Principal collectionCanisterId, string userAccountId, ulong chunkSize, ulong pageNo)
        {
            try
            {
                Debug.Log("🔄 Calling userNFTcollection...");
                var args = CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId),
                    CandidTypedValue.Text(userAccountId),
                    CandidTypedValue.Nat(chunkSize),
                    CandidTypedValue.Nat(pageNo)
                );

                QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "userNFTcollection", args);
                CandidArg reply = response.ThrowOrGetReply();
                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);

                if (result.ContainsKey("err"))
                {
                    Debug.LogWarning($"⚠️ Error from canister: {result["err"]}");
                    return (new List<(string, uint, string, string, Principal, ulong?)>(), 
                           new List<(string, uint, string, string, Principal, ulong?)>());
                }

                var okResult = (Dictionary<string, object>)result["ok"];
                
                var boughtNFTs = ((List<object>)okResult["boughtNFTs"])
                    .Select(item => ConvertNFTTuple((List<object>)item)).ToList();
                
                var unboughtNFTs = ((List<object>)okResult["unboughtNFTs"])
                    .Select(item => ConvertNFTTuple((List<object>)item)).ToList();

                Debug.Log($"✅ Fetched {boughtNFTs.Count} bought NFTs and {unboughtNFTs.Count} unbought NFTs");
                return (boughtNFTs, unboughtNFTs);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch user NFT collection: {e.Message}");
                return (new List<(string, uint, string, string, Principal, ulong?)>(), 
                       new List<(string, uint, string, string, Principal, ulong?)>());
            }
        }

        private (string, uint, string, string, Principal, ulong?) ConvertNFTTuple(List<object> tuple)
        {
            return (
                tuple[0].ToString()!,                    // TokenIdentifier
                Convert.ToUInt32(tuple[1]),             // TokenIndex
                tuple[2].ToString()!,                    // Metadata
                tuple[3].ToString()!,                    // CollectionName
                Principal.FromText(tuple[4].ToString()!), // Principal
                tuple[5] is null ? null : Convert.ToUInt64(tuple[5]) // Optional Price
            );
        }
    }
}