#nullable enable
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Responses;
using EdjCase.ICP.Candid.Models;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IC.GameKit
{
    public class API_Manager : MonoBehaviour
    {
        public static API_Manager Instance { get; private set; }
        private GreetingClient.GreetingClient? _greetingClient;
        private string _currentUserPrincipal = "";
        private bool _isInitialized = false;

        internal List<NFTCollection> _collectionsDict = new();
        private Dictionary<string, List<NFTCollection>> _userCollections = new();
        private Dictionary<string, List<NFTListing>> _marketplaceListings = new();

        public event Action<List<NFTCollection>>? OnCollectionsUpdated;
        public event Action<List<NFTListing>>? OnUserNFTListingsUpdated;

        public string CurrentUserPrincipal
        {
            get => _currentUserPrincipal;
            set
            {
                Debug.Log($"🔄 Setting CurrentUserPrincipal to: {value}");
                _currentUserPrincipal = value;
            }
        }

        private void Awake()
        {
            Debug.Log("🔄 Initializing API_Manager...");
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                Debug.Log("✅ API_Manager singleton set.");
            }
            else
            {
                Debug.LogWarning("⚠ Duplicate API_Manager found. Destroying this instance.");
                Destroy(gameObject);
            }
        }

        public void Initialize(GreetingClient.GreetingClient greetingClient)
        {
            Debug.Log("🔄 Initializing API_Manager with GreetingClient...");
            if (greetingClient == null)
            {
                Debug.LogError("❌ Cannot initialize with null GreetingClient!");
                return;
            }
            _greetingClient = greetingClient;
            _isInitialized = true;
            Debug.Log("✅ API_Manager initialized.");
        }

        private void Start()
        {
            Debug.Log("🔄 API_Manager starting, waiting for initialization...");
        }

        public async Task<List<(Principal, List<(long, Principal, string, string, string)>)>> GetAllCollections()
        {
            Debug.Log("🔄 Fetching all collections...");
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager not initialized.");
                return new List<(Principal, List<(long, Principal, string, string, string)>)>();
            }

            try
            {
                var result = await _greetingClient.GetAllCollections();
                Debug.Log($"✅ Retrieved {result.Count} collections.");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to fetch collections: {e.Message}");
                return new List<(Principal, List<(long, Principal, string, string, string)>)>();
            }
        }

        public async Task FetchAllCollections()
        {
            Debug.Log("🔄 Starting FetchAllCollections...");
            var rawCollections = await GetAllCollections();

            _collectionsDict.Clear();
            Debug.Log("🔍 Processing collections...");

            foreach (var (userPrincipal, nftList) in rawCollections)
            {
                string principalStr = userPrincipal.ToText();

                foreach (var (timestamp, canisterId, name, symbol, metadata) in nftList)
                {
                    _collectionsDict.Add(new NFTCollection
                    {
                        OwnerPrincipal = principalStr,
                        Timestamp = timestamp,
                        CanisterId = canisterId.ToText(),
                        Name = name,
                        Symbol = symbol,
                        Metadata = metadata
                    });
                }
            }

            Debug.Log($"✅ Loaded {_collectionsDict.Count} users with collections.");
        }

        public async Task FetchCurrentUserCollections(string? userPrincipal = null)
        {
            Debug.Log("🔄 Fetching current user's NFT collections...");
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager not initialized.");
                return;
            }

            CurrentUserPrincipal = userPrincipal ?? await _greetingClient.GetPrincipal();
            Debug.Log($"🔹 Current User Principal: {CurrentUserPrincipal}");

            var rawCollections = await GetAllCollections();
            _userCollections.Clear();
            Debug.Log("🔍 Processing user collections...");

            foreach (var (userPrincipalFromCanister, nftList) in rawCollections)
            {
                string userId = userPrincipalFromCanister.ToText();
                _userCollections[userId] = new List<NFTCollection>();

                foreach (var (timestamp, canisterId, name, symbol, metadata) in nftList)
                {
                    _userCollections[userId].Add(new NFTCollection
                    {
                        OwnerPrincipal = userId,
                        Timestamp = timestamp,
                        CanisterId = canisterId.ToText(),
                        Name = name,
                        Symbol = symbol,
                        Metadata = metadata
                    });
                }
            }

            if (_userCollections.TryGetValue(CurrentUserPrincipal, out var userCollections))
            {
                Debug.Log($"✅ Found {userCollections.Count} collections for user {CurrentUserPrincipal}.");
                OnCollectionsUpdated?.Invoke(userCollections);
            }
            else
            {
                Debug.LogWarning($"⚠ No collections found for user {CurrentUserPrincipal}.");
                OnCollectionsUpdated?.Invoke(new List<NFTCollection>());
            }
        }

        public async Task FetchUserNFTListings()
        {
            Debug.Log("🔄 Fetching NFT listings for user's collections...");
            if (!_isInitialized || _greetingClient == null)
            {
                Debug.LogError("❌ API_Manager not initialized.");
                return;
            }

            if (string.IsNullOrEmpty(CurrentUserPrincipal) || !_userCollections.TryGetValue(CurrentUserPrincipal, out var collections))
            {
                Debug.LogWarning("⚠ No collections found for the user. Fetch collections first.");
                return;
            }

            List<NFTListing> allUserNFTs = new();

            foreach (var collection in collections)
            {
                Principal collectionCanisterId = Principal.FromText(collection.CanisterId);
                ulong chunkSize = 10UL;
                ulong pageNo = 0UL;

                Debug.Log($"🔄 Fetching listings for collection {collection.Name}...");
                var (listings, currentPage, totalPages) = await _greetingClient.CountListings(collectionCanisterId, chunkSize, pageNo);

                foreach (var (tokenIndex, tokenIdentifier, listing, metadata, price) in listings)
                {
                    allUserNFTs.Add(new NFTListing
                    {
                        TokenIndex = tokenIndex,
                        TokenIdentifier = tokenIdentifier,
                        ListingDetails = listing.ToString() ?? "Unknown",
                        Metadata = metadata,
                        Price = price
                    });
                }
                Debug.Log($"✅ Fetched {listings.Count} listings from {collection.Name}, page {currentPage}/{totalPages}");
            }

            Debug.Log($"✅ Loaded {allUserNFTs.Count} NFTs for the current user.");
            OnUserNFTListingsUpdated?.Invoke(allUserNFTs);
        }

        // public List<NFTCollection> GetAllCollectionsList()
        // {
        //     var collections = _collectionsDict.Values.SelectMany(c => c).ToList();
        //     Debug.Log($"🔍 Retrieved {collections.Count} total collections.");
        //     return collections;
        // }

        // public List<NFTCollection> GetCollectionsByUser(string principal)
        // {
        //     Debug.Log($"🔍 Fetching collections for principal: {principal}");
        //     if (_collectionsDict.TryGetValue(principal, out var userCollections))
        //     {
        //         Debug.Log($"✅ Found {userCollections.Count} collections for {principal}.");
        //         return userCollections;
        //     }
        //     Debug.LogWarning($"⚠ No collections found for {principal}.");
        //     return new List<NFTCollection>();
        // }
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