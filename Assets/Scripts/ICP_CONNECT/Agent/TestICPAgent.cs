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
using System.Linq; // Added this line to resolve 'Any' errors

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GrowTownGameScene";
        public string greetFrontend = "https://s6dkc-nyaaa-aaaac-albta-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";
        public string icHost = "https://icp0.io";

        public bool isSceneLoaded =false;
        private Ed25519Identity mEd25519Identity = null;
        private DelegationIdentity mDelegationIdentity = null;

        public Ed25519Identity TestIdentity => mEd25519Identity;

        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                mDelegationIdentity = value;
                if (!isSceneLoaded){
StartCoroutine(EnableButtonsCoroutine());
isSceneLoaded = true;
                }
            }
        }

        private bool isSceneLoading = false;

        public static TestICPAgent Instance { get; private set; }

        void Awake()
        {
            TestICPAgent[] agents = FindObjectsOfType<TestICPAgent>();
            if (agents.Length > 1)
            {
                Debug.LogWarning("⚠ Duplicate TestICPAgent found. Destroying this instance.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Debug.Log("ℹ️ Starting TestICPAgent...");
            exitButton.onClick.AddListener(ExitGame);
            mEd25519Identity = Ed25519Identity.Generate();
        }

        public void StartGameFun()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (!isSceneLoading && currentScene != sceneToLoad && !IsRenderingFailed())
            {
                isSceneLoading = true;
                SceneManager.LoadScene(sceneToLoad);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Debug.LogWarning($"Scene {sceneToLoad} is already loaded, loading, or rendering failed. Skipping load.");
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isSceneLoading = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void ExitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private System.Collections.IEnumerator EnableButtonsCoroutine()
        {
            yield return EnableButtonsAsync().AsCoroutine();
        }

        public async Task EnableButtonsAsync()
        {
            if (mDelegationIdentity != null)
            {
                Debug.Log("✅ Delegation Identity is set. Calling AutoCreateUser...");
                await AutoCreateUserAsync();
            }
            else
            {
                Debug.LogWarning("⚠ DelegationIdentity is null. Proceeding with anonymous identity...");
                await AutoCreateUserAsync(true);
            }
        }

        private async Task AutoCreateUserAsync(bool useAnonymous = false)
        {
            Debug.Log("1️⃣ Creating HttpAgent...");
            var httpClient = new HttpClient { BaseAddress = new Uri(icHost) };
            var agentHttpClient = new DefaultHttpClient(httpClient);
            IAgent agent = new HttpAgent(
                httpClient: agentHttpClient,
                identity: useAnonymous || mDelegationIdentity == null ? null : mDelegationIdentity
            );
            Debug.Log($"Agent Identity: {(agent.Identity != null ? agent.Identity.GetPrincipal().ToText() : "Anonymous")}");

            Debug.Log("2️⃣ Setting canisterId...");
            Principal canisterId;
            try
            {
                canisterId = Principal.FromText(greetBackendCanister);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to parse canister ID '{greetBackendCanister}': {ex.Message}");
                StartGameFun();
                return;
            }

            Debug.Log("3️⃣ Creating GreetingClient...");
            var client = new GreetingClient.GreetingClient(agent, canisterId);

            Debug.Log("4️⃣ Checking API_Manager.Instance...");
            if (API_Manager.Instance == null)
            {
                Debug.LogError("❌ API_Manager.Instance is null! Creating new instance...");
                GameObject apiManagerObj = new GameObject("API_Manager");
                API_Manager apiManager = apiManagerObj.AddComponent<API_Manager>();
                apiManager.Initialize(client);
            }
            else
            {
                Debug.Log("5️⃣ Initializing API_Manager...");
                API_Manager.Instance.Initialize(client);
            }

            try
            {
                Debug.Log("6️⃣ Fetching user principal...");
                string userPrincipalString;
                try
                {
                    var principalTask = client.GetPrincipal();
                    Debug.Log("6.1️⃣ Initiated GetPrincipal call...");
                    userPrincipalString = await Task.WhenAny(principalTask, Task.Delay(10000)) == principalTask
                        ? principalTask.Result
                        : throw new TimeoutException("GetPrincipal timed out after 10s");
                    Debug.Log("6.2️⃣ GetPrincipal completed.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to fetch principal: {ex.Message}");
                    Debug.LogWarning("⚠ Falling back to anonymous principal...");
                    userPrincipalString = "2vxsx-fae"; // Anonymous principal
                }

                Principal userPrincipal = Principal.FromText(userPrincipalString);
                Debug.Log($"✅ Principal: {userPrincipal}");
                DataTransfer.UserPrincipal = userPrincipal.ToString();

                string uuid = Guid.NewGuid().ToString();
                Debug.Log($"✅ Generated UUID: {uuid}");

                Debug.Log("7️⃣ Preparing create_user call...");
                CandidArg arg = CandidArg.FromCandid(
                    CandidTypedValue.FromObject(userPrincipal),
                    CandidTypedValue.FromObject(uuid)
                );

                Debug.Log("8️⃣ Calling create_user on canister...");
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

                Debug.Log("9️⃣ Waiting for collections to propagate...");
                int attempts = 0;
                const int maxAttempts = 1;
                List<(Principal, List<(long, Principal, string, string, string)>)> allCollections = null; // Explicitly typed
                while (attempts < maxAttempts)
                {
                    await Task.Delay(2000); // Wait 2 seconds per attempt
                    Debug.Log($"⏳ Attempt {attempts + 1}/{maxAttempts}: Fetching collections...");
                    await API_Manager.Instance.FetchAllCollections();

                    // if (allCollections.Any(c => c.Item1.ToText() == userPrincipalString))
                    // {
                    //     Debug.Log($"✅ Found {allCollections.Count} collections for user {userPrincipalString}");
                    //     break;
                    // }
                    attempts++;
                    Debug.Log($"collection are fetching");
                }

                // if (allCollections != null && allCollections.Any())
                // {
                //     foreach ((Principal principal, List<(long, Principal, string, string, string)> collections) in allCollections) // Explicit types added
                //     {
                //         Debug.Log($"User: {principal.ToText()}, Collections: {collections.Count}");
                //     }
                // }
                // else
                // {
                //     Debug.LogWarning($"⚠ No collections found for {userPrincipalString} after {maxAttempts} attempts.");
                // }

                // Debug.Log("🔟 Fetching user collections...");
                // try
                // {
                //     await API_Manager.Instance.FetchAllCollections();
                //     Debug.Log("✅ User collections fetched successfully.");
                // }
                // catch (Exception ex)
                // {
                //     Debug.LogError($"❌ Failed to fetch user collections: {ex.Message}");
                //     Debug.LogWarning("⚠ Proceeding without user collections...");
                // }

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