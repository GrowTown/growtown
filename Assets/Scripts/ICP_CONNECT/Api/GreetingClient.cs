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
        public async Task<string> GetPrinicpal()
        {
            CandidArg arg = CandidArg.FromCandid();
            QueryResponse response = await this.Agent.QueryAsync(this.CanisterId, "getPrincipal", arg);
            CandidArg reply = response.ThrowOrGetReply();
            return reply.ToObjects<string>(this.Converter);
        }

        public async Task<List<(Principal, List<(ulong, Principal, string, string, string)>)>> GetAllCollections()
        {
            CandidArg arg = CandidArg.FromCandid();
            try
            {
                Debug.Log("🔄 Calling getAllCollections...");

                if (this.Agent == null)
                {
                    Debug.LogError("❌ Agent is NULL, cannot proceed!");
                    return new List<(Principal, List<(ulong, Principal, string, string, string)>)>();
                }

                // Call the canister function
                CandidArg reply = await this.Agent.CallAsync(this.CanisterId, "getAllCollections", arg);

                // Log the raw response for debugging
                Debug.Log($"✅ Raw response from canister: {reply}");

                // Deserialize response
                List<(Principal, List<(ulong, Principal, string, string, string)>)>? rawResult =
                    reply.ToObjects<List<(Principal, List<(ulong, Principal, string, string, string)>)>>(this.Converter);

                if (rawResult == null)
                {
                    Debug.LogWarning("⚠️ No collections found. Returning an empty list.");
                    return new List<(Principal, List<(ulong, Principal, string, string, string)>)>();
                }

                Debug.Log($"✅ Successfully retrieved {rawResult.Count} user collections.");
                return rawResult;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch collections: {e.Message}");
                return new List<(Principal, List<(ulong, Principal, string, string, string)>)>();
            }
        }

        // Countlistings
        public async Task<(List<(ulong, string, string, string, ulong)>, ulong, ulong)> CountListings(Principal collectionCanisterId, ulong chunkSize, ulong pageNo)
        {
            try
            {
                Debug.Log("🔄 Calling countListings...");

                // Prepare the Candid arguments manually
                var args = CandidArg.FromObjects(
                    CandidValue.Principal(collectionCanisterId),
                    CandidValue.Nat(chunkSize),
                    CandidValue.Nat(pageNo)
                );

                // Call the countListings method in the canister
                CandidArg reply = await this.Agent.CallAsynchronousAndWaitAsync(this.CanisterId, "countlistings", args);

                // Log the raw response for debugging
                Debug.Log($"✅ Raw response from canister: {reply}");

                // Parse the result
                var result = reply.ToObjects<Dictionary<string, object>>(this.Converter);

                // Extract data (NFT listings)
                var data = ((List<object>)result["data"])
                    .Select(item =>
                    {
                        var tuple = (List<object>)item;
                        return (
                            (ulong)tuple[0], // TokenIndex
                            tuple[1].ToString(), // TokenIdentifier
                            tuple[2].ToString(), // Listing
                            tuple[3].ToString(), // Metadata
                            (ulong)tuple[4] // Price
                        );
                    }).ToList();

                // Extract current page and total pages
                ulong currentPage = Convert.ToUInt64(result["current_page"]);
                ulong totalPages = Convert.ToUInt64(result["total_pages"]);

                Debug.Log($"✅ Listings fetched: {data.Count} on page {currentPage}/{totalPages}");

                return (data, currentPage, totalPages);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch listings: {e.Message}");
                return (new List<(ulong, string, string, string, ulong)>(), 0, 0);
            }
        }

     /*   public async Task<(ulong, string, string, string, ulong?, bool)?> GetSingleNonFungibleTokens(Principal collectionCanisterId, ulong tokenId, string user)
        {
            var response = await Agent.CallAsynchronousAndWaitAsync(collectionCanisterId,"getSingleNonFungibleTokens",CandidArg.FromCandid(CandidTypedValue.Principal(collectionCanisterId),CandidTypedValue.Nat(tokenId),CandidTypedValue.Text(user)) );

            if (response.Values.Count == 0)
            {
                return null;
            }

            var result = response.Values[0].AsList();

            ulong tokenIndex = result[0].AsNat().ToUInt64();
            string tokenIdentifier = result[1].AsText();
            string accountIdentifier = result[2].AsText();
            string metadata = result[3].AsText();
            ulong? price = result[4].IsNull() ? (ulong?)null : result[4].AsNat().ToUInt64();
            bool isOwned = result[5].AsBool();

            return (tokenIndex, tokenIdentifier, accountIdentifier, metadata, price, isOwned);
        }*/


        // Call purchaseNft function
       /* private async Task<(bool success, string errorMessage)> PurchaseNft(Principal collectionCanisterId, string tokenId, ulong price, string buyerAccountId)
        {
            var args = new[]
            {
            CandidTypedValue.FromValueAndType(new CandidPrimitive(CandidPrimitiveType.Principal, collectionCanisterId),new CandidTypePrincipal()
            ),
            CandidTypedValue.FromValueAndType(
                new CandidPrimitive(CandidPrimitiveType.Text, tokenId),
                new CandidTypeText()
            ),
            CandidTypedValue.FromValueAndType(
                new CandidPrimitive(CandidPrimitiveType.Nat, price),
                new CandidTypeNat()
            ),
            CandidTypedValue.FromValueAndType(
                new CandidPrimitive(CandidPrimitiveType.Text, buyerAccountId),
                new CandidTypeText()
            )
        };

            var response = await Agent.CallAsync(
                collectionCanisterId,
                "purchaseNft",
                args
            );

            var result = response.Values[0].AsVariant();
            return result.Tag == "Ok"
                ? (true, null)
                : (false, result.AsVariant().AsText());
        }*/



        /// <summary>
        /// SettlePurchase
        /// </summary>
        /// <param name="collectionCanisterId"></param>
        /// <param name="paymentAddress"></param>
        /// <returns></returns>
        public async Task<CandidArg> SettlePurchase(Principal collectionCanisterId, string paymentAddress)
        {
            var response = await Agent.CallAsynchronousAndWaitAsync(collectionCanisterId,"settlepurchase",CandidArg.FromCandid(
                    CandidTypedValue.Principal(collectionCanisterId),
                    CandidTypedValue.Text(paymentAddress)
                )
            );

            return response;
        }

        //BalanceNFT_Settlement
        public async Task TriggerBalanceNftSettlement(Principal collectionCanisterId)
        {
            await Agent.CallAsynchronousAndWaitAsync(collectionCanisterId, "balance_nft_settelment", CandidArg.FromCandid(CandidTypedValue.Principal(collectionCanisterId)));
        }

        //All Users Activity
        public async Task<CandidArg> GetAllUserActivity(Principal collectionCanisterId, string buyerId, ulong chunkSize, ulong pageNo)
        {
            var response = await Agent.CallAsynchronousAndWaitAsync(collectionCanisterId, "alluseractivity", CandidArg.FromCandid(CandidTypedValue.Text(buyerId), CandidTypedValue.Nat(chunkSize), CandidTypedValue.Nat(pageNo)));

            return response;
        }

        //userNFTcollection 
        public async Task<CandidArg> GetUserNFTCollection(Principal collectionCanisterId,string userAccountId,ulong chunkSize,ulong pageNo)
        {
            var response = await Agent.CallAsynchronousAndWaitAsync(collectionCanisterId,"userNFTcollection",CandidArg.FromCandid(CandidTypedValue.Text(userAccountId),CandidTypedValue.Nat(chunkSize),CandidTypedValue.Nat(pageNo)));

            return response;
        }

    }
}
