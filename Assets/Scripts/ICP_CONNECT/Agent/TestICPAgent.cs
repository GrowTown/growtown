using UnityEngine;
using UnityEngine.UI;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Agents.Http;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;
using EdjCase.ICP.BLS;

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GrowTownGameScene";
        public string greetFrontend = "https://7kl52-gyaaa-aaaac-ahmgq-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";
        public string icHost = "https://icp0.io";

        private bool isSceneLoaded = false;
        private Ed25519Identity mEd25519Identity = null;
        private DelegationIdentity mDelegationIdentity = null;
        private bool isProcessingDelegation = false;

        public Ed25519Identity TestIdentity => mEd25519Identity;
        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                mDelegationIdentity = value;
                if (!isSceneLoaded && mDelegationIdentity != null)
                {
                    if (!isProcessingDelegation)
                    {
                        isProcessingDelegation = true;
                        StartCoroutine(ProcessDelegationAndEnableButtons());
                    }
                    else
                    {
                        Debug.Log("ℹ️ Delegation already being processed. Waiting for completion...");
                    }
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
            isSceneLoaded = true;
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

        private IEnumerator ProcessDelegationAndEnableButtons()
        {
            Debug.Log("🔄 Starting ProcessDelegationAndEnableButtons...");
            yield return EnableButtonsAsync().AsCoroutine();

            Debug.Log("✅ Delegation processing complete, starting game...");
            StartGameFun();

            isProcessingDelegation = false;
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

            var agent = useAnonymous ? new HttpAgent() : new HttpAgent(DelegationIdentity, null, new DefaultBlsCryptograhy());
            var canisterId = Principal.FromText(greetBackendCanister);
            var client = new GreetingClient.GreetingClient(agent, canisterId);
            Debug.Log("✅ GreetingClient created.");

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
                    userPrincipalString = "2vxsx-fae";
                }

                Principal userPrincipal = Principal.FromText(userPrincipalString);
                Debug.Log($"✅ User Principal set: {userPrincipal}");
                DataTransfer.UserPrincipal = userPrincipal.ToString();

                string uuid = Guid.NewGuid().ToString();
                Debug.Log($"✅ Generated UUID: {uuid}");

                Debug.Log("7️⃣ Preparing create_user call...");
                CandidArg arg = CandidArg.FromCandid(
                    CandidTypedValue.FromObject(userPrincipal),
                    CandidTypedValue.FromObject(uuid)
                );
                Debug.Log("🔄 Calling create_user on canister...");
                const int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        CandidArg reply = await agent.CallAsync(canisterId, "create_user", arg);
                        Debug.Log($"✅ create_user response: {reply}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Attempt {attempt}/{maxRetries} Failed to call create_user: {ex.Message}\nStackTrace: {ex.StackTrace}");
                        if (attempt == maxRetries)
                        {
                            Debug.LogWarning("⚠ Max retries reached. Skipping create_user...");
                        }
                        else if (ex.Message.Contains("Certificate signature does not match"))
                        {
                            Debug.LogWarning("⚠ Certificate mismatch detected. Retrying...");
                            await Task.Delay(1000);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                Debug.Log("8️⃣ Fetching collections...");
                await API_Manager.Instance.FetchAllCollections();
                Debug.Log("✅ Collections fetched.");

                Debug.Log("9️⃣ Calling CountListings...");
                try
                {
                    if (API_Manager.Instance._collectionsDict.Count > 0)
                    {
                        string collectionCanisterIdStr = API_Manager.Instance._collectionsDict[0].CanisterId;
                        Principal collectionCanisterId = Principal.FromText(collectionCanisterIdStr);
                        Debug.Log($"🔄 Using collection canister ID: {collectionCanisterId}");
                        var (listings, currentPage, totalPages) = await client.CountListings(collectionCanisterId, 10UL, 0UL);
                        Debug.Log($"✅ CountListings returned {listings.Count} listings, page {currentPage}/{totalPages}");
                    }
                    else
                    {
                        Debug.LogWarning("⚠ No collections available to call CountListings.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to call CountListings: {ex.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Unexpected error in AutoCreateUserAsync: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }

        private bool IsRenderingFailed()
        {
            return Application.HasProLicense() && (Screen.currentResolution.width == 0 || Screen.currentResolution.height == 0);
        }
    }

    public static class TaskExtensions
    {
        public static IEnumerator AsCoroutine(this Task task)
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