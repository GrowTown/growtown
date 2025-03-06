#nullable enable
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Agent.Responses;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IC.GameKit
{
    public class API_Manager : MonoBehaviour
    {
        public static API_Manager Instance { get; private set; }

        private GreetingClient.GreetingClient? _greetingClient;
        private string _currentUserPrincipal = "";
        private bool _isInitialized = false;

        private Dictionary<string, List<NFTCollection>> _collectionsDict = new();
        private Dictionary<string, List<NFTCollection>> _userCollections = new();
        private Dictionary<string, List<NFTListing>> _marketplaceListings = new();

        public event Action<List<NFTCollection>>? OnCollectionsUpdated;
        public event Action<List<NFTListing>>? OnUserNFTListingsUpdated;

        public string CurrentUserPrincipal
        {
            get => _currentUserPrincipal;
            set => _currentUserPrincipal = value;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null); // Ensure root GameObject
                DontDestroyOnLoad(gameObject);
                Debug.Log("✅ API_Manager instance created and set as singleton.");
            }
            else
            {
                Debug.LogWarning("⚠ Duplicate API_Manager found. Destroying this instance.");
                Destroy(gameObject);
            }
        }

        public void Initialize(GreetingClient.GreetingClient greetingClient)
        {
            if (greetingClient == null)
            {
                Debug.LogError("❌ Cannot initialize API_Manager with null GreetingClient!");
                return;
            }
            _greetingClient = greetingClient;
            _isInitialized = true;
            Debug.Log("✅ API_Manager initialized successfully with greeting client.");
        }

        private void Start()
        {
            Debug.Log("🔄 API_Manager starting, waiting for initialization...");
        }

        public async Task<List<(Principal, List<(long, Principal, string, string, string)>)>> GetAllCollections()
        {
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager is not initialized. Cannot fetch collections.");
                return new List<(Principal, List<(long, Principal, string, string, string)>)>();
            }

            try
            {
                Debug.Log("🔄 Initiating getAllCollections call to canister...");
                CandidArg arg = CandidArg.FromCandid();
                QueryResponse response = await _greetingClient.Agent.QueryAsync(_greetingClient.CanisterId, "getAllCollections", arg);
                Debug.Log($"🔍 Query response received: {response}");
                CandidArg reply = response.ThrowOrGetReply();
                Debug.Log($"🔍 Raw CandidArg reply: {reply}");
                var rawResult = reply.ToObjects<List<(Principal, List<(long, Principal, string, string, string)>)>>(_greetingClient.Converter)
                    ?? new List<(Principal, List<(long, Principal, string, string, string)>)>();
                Debug.Log($"✅ Successfully retrieved {rawResult.Count} user collections from canister.");
                return rawResult;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch collections from getAllCollections: {e.Message}\nStackTrace: {e.StackTrace}");
                return new List<(Principal, List<(long, Principal, string, string, string)>)>();
            }
        }

        public async Task FetchAllCollections()
        {
            Debug.Log("🔄 Starting FetchAllCollections...");
            var rawCollections = await GetAllCollections();

            _collectionsDict.Clear();
            Debug.Log("🔍 Processing fetched collections...");

            foreach (var (userPrincipal, nftList) in rawCollections)
            {
                string principalStr = userPrincipal.ToText();

                if (!_collectionsDict.ContainsKey(principalStr))
                {
                    _collectionsDict[principalStr] = new List<NFTCollection>();
                }

                foreach (var (timestamp, canisterId, name, symbol, metadata) in nftList)
                {
                    NFTCollection collection = new NFTCollection
                    {
                        OwnerPrincipal = principalStr,
                        Timestamp = timestamp,
                        CanisterId = canisterId.ToText(),
                        Name = name,
                        Symbol = symbol,
                        Metadata = metadata
                    };

                    _collectionsDict[principalStr].Add(collection);
                }
            }

            Debug.Log($"✅ Loaded {_collectionsDict.Count} unique users with NFT Collections.");
            OnCollectionsUpdated?.Invoke(GetAllCollectionsList());
        }

        public List<NFTCollection> GetAllCollectionsList()
        {
            return _collectionsDict.Values.SelectMany(c => c).ToList();
        }

        public async Task FetchCurrentUserCollections(string userPrincipal = null)
        {
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager is not initialized. Call Initialize() first.");
                return;
            }

            Debug.Log("🔄 Fetching Current User's NFT Collections...");
            CurrentUserPrincipal = userPrincipal ?? await _greetingClient.GetPrincipal();
            Debug.Log($"🔹 Current User Principal: {CurrentUserPrincipal}");

            Debug.Log("🔍 Fetching all collections for user filtering...");
            var rawCollections = await GetAllCollections();

            _userCollections.Clear();
            Debug.Log("🔍 Processing collections for current user...");

            foreach (var (userPrincipalFromCanister, nftList) in rawCollections)
            {
                string userId = userPrincipalFromCanister.ToText();
                _userCollections[userId] = new List<NFTCollection>();

                foreach (var (timestamp, canisterId, name, symbol, metadata) in nftList)
                {
                    NFTCollection collection = new NFTCollection
                    {
                        OwnerPrincipal = userId,
                        Timestamp = timestamp,
                        CanisterId = canisterId.ToText(),
                        Name = name,
                        Symbol = symbol,
                        Metadata = metadata
                    };

                    _userCollections[userId].Add(collection);
                }
            }

            if (_userCollections.TryGetValue(CurrentUserPrincipal, out var userCollections))
            {
                Debug.Log($"✅ Found {userCollections.Count} collections for the current user ({CurrentUserPrincipal}).");
                OnCollectionsUpdated?.Invoke(userCollections);
            }
            else
            {
                Debug.LogWarning($"⚠️ No NFT collections found for the current user ({CurrentUserPrincipal}).");
                OnCollectionsUpdated?.Invoke(new List<NFTCollection>());
            }
        }

        public async Task FetchUserNFTListings()
        {
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager is not initialized. Call Initialize() first.");
                return;
            }

            if (string.IsNullOrEmpty(CurrentUserPrincipal) || !_userCollections.TryGetValue(CurrentUserPrincipal, out var collections))
            {
                Debug.LogWarning("⚠ No collections found for the user. Fetch collections first.");
                return;
            }

            Debug.Log("🔄 Fetching NFT listings for user's collections...");

            List<NFTListing> allUserNFTs = new();

            foreach (var collection in collections)
            {
                Principal collectionCanisterId = Principal.FromText(collection.CanisterId);
                ulong chunkSize = 10UL;
                ulong pageNo = 0UL;

                var (listings, currentPage, totalPages) = await _greetingClient.CountListings(collectionCanisterId, chunkSize, pageNo);

                foreach (var (tokenIndex, tokenIdentifier, listing, metadata, price) in listings)
                {
                    NFTListing nftListing = new NFTListing
                    {
                        TokenIndex = tokenIndex,
                        TokenIdentifier = tokenIdentifier,
                        ListingDetails = listing.ToString() ?? "Unknown",
                        Metadata = metadata,
                        Price = price
                    };

                    allUserNFTs.Add(nftListing);
                }

                Debug.Log($"✅ Fetched {listings.Count} listings from collection {collection.Name}, page {currentPage}/{totalPages}");
            }

            Debug.Log($"✅ Loaded {allUserNFTs.Count} NFTs for the current user.");
            OnUserNFTListingsUpdated?.Invoke(allUserNFTs);
        }

        public List<NFTCollection> GetCollectionsByUser(string principal)
        {
            return _collectionsDict.TryGetValue(principal, out var userCollections) 
                ? userCollections 
                : new List<NFTCollection>();
        }
    }

    public class NFTCollection
    {
        public string OwnerPrincipal { get; set; } = "";
        public long Timestamp { get; set; }
        public string CanisterId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Symbol { get; set; } = "";
        public string Metadata { get; set; } = "";
    }

    public class NFTListing
    {
        public uint TokenIndex { get; set; }
        public string TokenIdentifier { get; set; } = "";
        public string ListingDetails { get; set; } = "";
        public string Metadata { get; set; } = "";
        public ulong Price { get; set; }
    }
}