#nullable enable
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Candid.Models;
using EdjCase.ICP.Candid;
using EdjCase.ICP.Agent.Responses;
using EdjCase.ICP.Agent.Identities;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class API_Manager : MonoBehaviour
{

    public static API_Manager Instance { get; private set; }

    private GreetingClient.GreetingClient? _greetingClient;
    string _currentuUserprincipalStr;

    private Dictionary<string, List<NFTCollection>> _collectionsDict = new Dictionary<string, List<NFTCollection>>();
    private Dictionary<string, List<NFTCollection>> _userCollections = new Dictionary<string, List<NFTCollection>>();
    private Dictionary<string, List<NFTListing>> _marketplaceListings = new Dictionary<string, List<NFTListing>>();
    private string _currentUserPrincipal;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes API_Manager with the ICP agent and target canister.
    /// This must be called before making any API requests.
    /// </summary>
    public void Initialize(GreetingClient.GreetingClient greetingClient)
    {
        _greetingClient = greetingClient;
    }
    private void Start()
    {
        InitializeNFTData();
    }

    private async void InitializeNFTData()
    {
        await FetchCurrentUserCollections();
        //await FetchUserNFTListings();
    }


    /// <summary>
    /// Fetches all NFT collections from the backend.
    /// Stores the results in a dictionary for fast lookups.
    /// </summary>
    public async Task FetchAllCollections()
    {
        if (_greetingClient == null)
        {
            Debug.LogError("❌ API_Manager is not initialized. Call Initialize() first.");
            return;
        }

        Debug.Log("🔄 Fetching all NFT Collections...");
        var rawCollections = await _greetingClient.GetAllCollections();

        _collectionsDict.Clear(); // Clear existing data before reloading

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

    /// <summary>
    /// Returns all NFT collections as a flat list.
    /// </summary>
    public List<NFTCollection> GetAllCollectionsList()
    {
        return _collectionsDict.Values.SelectMany(c => c).ToList();
    }

    /// <summary>
    /// Fetches the current user's NFTs and updates the UI.
    /// </summary>
    public async Task FetchCurrentUserCollections()
    {
        if (_greetingClient == null)
        {
            Debug.LogError("❌ API_Manager is not initialized. Call Initialize() first.");
            return;
        }

        Debug.Log("🔄 Fetching Current User's NFT Collections...");

        // Get the Current User's Principal ID
        CurrentUserPrincipal = await _greetingClient.GetPrinicpal();
        Debug.Log($"🔹 Current User Principal: {CurrentUserPrincipal}");

        // Fetch all collections
        var rawCollections = await _greetingClient.GetAllCollections();

        _userCollections.Clear();

        foreach (var (userPrincipal, nftList) in rawCollections)
        {
            string userId = userPrincipal.ToText();
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

        if (_userCollections.ContainsKey(CurrentUserPrincipal))
        {
            Debug.Log($"✅ Found {_userCollections[CurrentUserPrincipal].Count} collections for the current user.");
            OnCollectionsUpdated?.Invoke(_userCollections[CurrentUserPrincipal]); // Update UI
        }
        else
        {
            Debug.LogWarning("⚠️ No NFT collections found for the current user.");
            OnCollectionsUpdated?.Invoke(new List<NFTCollection>()); // Empty UI
        }
    }

    /*/// <summary>
    /// Fetches all NFTs from the current user's collections.
    /// </summary>
    public async Task FetchUserNFTListings()
    {
        if (string.IsNullOrEmpty(CurrentUserPrincipal) || !_userCollections.ContainsKey(CurrentUserPrincipal))
        {
            Debug.LogWarning("⚠ No collections found for the user. Fetch collections first.");
            return;
        }

        Debug.Log("🔄 Fetching NFT listings for user's collections...");

        List<NFTListing> allUserNFTs = new List<NFTListing>();

        foreach (var collection in _userCollections[CurrentUserPrincipal])
        {
            string collectionCanisterId = collection.CanisterId;

            // Fetch NFT listings for each collection
            var result = await _greetingClient.CountListings(collectionCanisterId,10,);

            if (result.IsOk)
            {
                foreach (var (tokenIndex, tokenIdentifier, listing, metadata, price) in result.Ok.data)
                {
                    NFTListing nftListing = new NFTListing
                    {
                        TokenIndex = tokenIndex,
                        TokenIdentifier = tokenIdentifier,
                        ListingDetails = listing,
                        Metadata = metadata,
                        Price = price
                    };

                    allUserNFTs.Add(nftListing);
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ No NFTs found in collection: {collectionCanisterId}");
            }
        }

        Debug.Log($"✅ Loaded {allUserNFTs.Count} NFTs for the current user.");
        OnUserNFTListingsUpdated?.Invoke(allUserNFTs); // Update UI
    }*/

    /// <summary>
    /// Returns NFT collections owned by a specific user (Principal).
    /// </summary>
    public List<NFTCollection> GetCollectionsByUser(string principal)
    {
        if (_collectionsDict.TryGetValue(principal, out var userCollections))
        {
            return userCollections;
        }
        return new List<NFTCollection>();
    }


}

/// <summary>
/// Represents an NFT collection.
/// </summary>
public class NFTCollection
{
    public string OwnerPrincipal { get; set; } = "";
    public ulong Timestamp { get; set; }
    public string CanisterId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Metadata { get; set; } = "";
}

/// <summary>
/// Represents for NFT Listings.
/// </summary>
public class NFTListing
{
    public ulong TokenIndex;
    public string TokenIdentifier;
    public string ListingDetails;
    public string Metadata;
    public ulong Price;
}


