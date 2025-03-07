using UnityEngine;
using UnityEngine.UI;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Agents.Http;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using UnityEditor; // Added this line to resolve 'Any' errors

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GrowTownGameScene";
        public string greetFrontend = "https://s6dkc-nyaaa-aaaac-albta-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";
        public string icHost = "https://icp0.io";

        public bool isSceneLoaded = false;
        private Ed25519Identity mEd25519Identity = null;
        private DelegationIdentity mDelegationIdentity = null;

        public Ed25519Identity TestIdentity => mEd25519Identity;
        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                mDelegationIdentity = value;
                if (!isSceneLoaded)
                {
                    StartCoroutine(EnableButtonsCoroutine());
                    isSceneLoaded = true;
                }
            }
        }

        private bool isSceneLoading = false;
        public static TestICPAgent Instance { get; private set; }

        void Awake()
        {
            Debug.Log("🔄 Initializing TestICPAgent...");
            TestICPAgent[] agents = FindObjectsOfType<TestICPAgent>();
            if (agents.Length > 1)
            {
                Debug.LogWarning("⚠ Duplicate TestICPAgent found. Destroying this instance.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ TestICPAgent singleton set.");
        }

        void Start()
        {
            Debug.Log("ℹ️ Starting TestICPAgent...");
            exitButton.onClick.AddListener(ExitGame);
            mEd25519Identity = Ed25519Identity.Generate();
            Debug.Log("✅ Generated Ed25519Identity.");
        }

        public void StartGameFun()
        {
            Debug.Log($"🔄 Attempting to load scene: {sceneToLoad}");
            string currentScene = SceneManager.GetActiveScene().name;
            if (!isSceneLoading && currentScene != sceneToLoad && !IsRenderingFailed())
            {
                isSceneLoading = true;
                SceneManager.LoadScene(sceneToLoad);
                SceneManager.sceneLoaded += OnSceneLoaded;
                Debug.Log($"✅ Loading scene: {sceneToLoad}");
            }
            else
            {
                Debug.LogWarning($"⚠ Scene {sceneToLoad} is already loaded, loading, or rendering failed. Skipping load.");
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isSceneLoading = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log($"✅ Scene loaded: {scene.name}");
        }

        void ExitGame()
        {
            Debug.Log("🔚 Exiting game...");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private System.Collections.IEnumerator EnableButtonsCoroutine()
        {
            Debug.Log("🔄 Starting EnableButtonsCoroutine...");
            yield return EnableButtonsAsync().AsCoroutine();
        }

        public async Task EnableButtonsAsync()
        {
            Debug.Log("🔄 EnableButtonsAsync started...");
            if (mDelegationIdentity != null)
            {
                Debug.Log("✅ Delegation Identity is set. Calling AutoCreateUser...");
                await AutoCreateUserAsync();
            }
            else
            {
                Debug.LogWarning("⚠ DelegationIdentity is null. Using anonymous identity...");
                await AutoCreateUserAsync(true);
            }
        }

        private async Task AutoCreateUserAsync(bool useAnonymous = false)
{
    Debug.Log("1️⃣ Initializing AutoCreateUserAsync...");

    // Step 1: Create HttpAgent
    Debug.Log("🔄 Creating HttpAgent...");
    var httpClient = new HttpClient { BaseAddress = new Uri(icHost) };
    var agentHttpClient = new DefaultHttpClient(httpClient);
    IAgent agent = new HttpAgent(
        httpClient: agentHttpClient,
        identity: useAnonymous || mDelegationIdentity == null ? null : mDelegationIdentity
    );
    Debug.Log($"✅ Agent Identity: {(agent.Identity != null ? agent.Identity.GetPrincipal().ToText() : "Anonymous")}");

    // Step 2: Parse Canister ID
    Debug.Log($"2️⃣ Parsing canister ID: {greetBackendCanister}");
    Principal canisterId;
    try
    {
        canisterId = Principal.FromText(greetBackendCanister);
        Debug.Log($"✅ Canister ID parsed: {canisterId}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"❌ Failed to parse canister ID '{greetBackendCanister}': {ex.Message}");
        StartGameFun();
        return;
    }

    // Step 3: Create GreetingClient
    Debug.Log("3️⃣ Creating GreetingClient...");
    var client = new GreetingClient.GreetingClient(agent, canisterId);
    Debug.Log("✅ GreetingClient created.");

    // Step 4: Initialize API_Manager
    Debug.Log("4️⃣ Checking API_Manager.Instance...");
    if (API_Manager.Instance == null)
    {
        Debug.LogWarning("⚠ API_Manager.Instance is null! Creating new instance...");
        GameObject apiManagerObj = new GameObject("API_Manager");
        API_Manager apiManager = apiManagerObj.AddComponent<API_Manager>();
        apiManager.Initialize(client);
    }
    else
    {
        Debug.Log("✅ Initializing existing API_Manager...");
        API_Manager.Instance.Initialize(client);
    }

    try
    {
        // Step 5: Fetch Principal
        Debug.Log("5️⃣ Fetching user principal...");
        string userPrincipalString;
        try
        {
            var principalTask = client.GetPrincipal();
            Debug.Log("🔄 Initiated GetPrincipal call...");
            userPrincipalString = await Task.WhenAny(principalTask, Task.Delay(10000)) == principalTask
                ? principalTask.Result
                : throw new TimeoutException("GetPrincipal timed out after 10s");
            Debug.Log($"✅ Principal fetched: {userPrincipalString}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to fetch principal: {ex.Message}");
            Debug.LogWarning("⚠ Falling back to anonymous principal...");
            userPrincipalString = "2vxsx-fae"; // Anonymous principal
        }

        Principal userPrincipal = Principal.FromText(userPrincipalString);
        Debug.Log($"✅ User Principal set: {userPrincipal}");
        DataTransfer.UserPrincipal = userPrincipal.ToString();

        // Step 6: Generate UUID
        string uuid = Guid.NewGuid().ToString();
        Debug.Log($"✅ Generated UUID: {uuid}");

        // Step 7: Create User
        Debug.Log("7️⃣ Preparing create_user call...");
        CandidArg arg = CandidArg.FromCandid(
            CandidTypedValue.FromObject(userPrincipal),
            CandidTypedValue.FromObject(uuid)
        );
        Debug.Log("🔄 Calling create_user on canister...");
        try
        {
            CandidArg reply = await agent.CallAsync(canisterId, "create_user", arg);
            Debug.Log($"✅ create_user response: {reply}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to call create_user: {ex.Message}");
            Debug.LogWarning("⚠ Skipping create_user due to error...");
        }

        // Step 8: Fetch Collections
        Debug.Log("8️⃣ Fetching collections...");
        await API_Manager.Instance.FetchAllCollections();
        Debug.Log("✅ Collections fetched.");

        // Step 9: Call CountListings with a specific collectionCanisterId
        Debug.Log("9️⃣ Calling CountListings...");
        try
        {
                // Use the first collection's CanisterId as an example (you can modify this logic)
                string collectionCanisterIdStr =API_Manager.Instance._collectionsDict[0].CanisterId;
                Principal collectionCanisterId = Principal.FromText(collectionCanisterIdStr);
                Debug.Log($"🔄 Using collection canister ID: {collectionCanisterId}");

                var (listings, currentPage, totalPages) = await client.CountListings(collectionCanisterId, 10UL, 0UL);
                Debug.Log($"✅ CountListings returned {listings.Count} listings, page {currentPage}/{totalPages}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to call CountListings: {ex.Message}");
        }

        // Step 10: Start Game
        Debug.Log("🔚 Starting game...");
        StartGameFun();
    }
    catch (Exception e)
    {
        Debug.LogError($"❌ Unexpected error in AutoCreateUserAsync: {e.Message}\nStackTrace: {e.StackTrace}");
        StartGameFun();
    }
}
        private bool IsRenderingFailed()
        {
            return Application.HasProLicense() && (Screen.currentResolution.width == 0 || Screen.currentResolution.height == 0);
        }
    }

    public static class TaskExtensions
    {
        public static System.Collections.IEnumerator AsCoroutine(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        }
    }
}