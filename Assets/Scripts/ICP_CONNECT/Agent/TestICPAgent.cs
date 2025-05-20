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
using InternetClients.greetBackendCanister;
using InternetClients.greetBackendCanister.Models;

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

            #if UNITY_ANDROID
            // Configure Android-specific settings
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            #endif

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
                await TestGetAllUsers();
            }
            else
            {
                Debug.LogWarning("⚠ DelegationIdentity is null. Using anonymous identity...");
                await AutoCreateUserAsync(true);
                await TestGetAllUsers();
            }
        }

        private async Task AutoCreateUserAsync(bool useAnonymous = false)
        {
            Debug.Log("1️⃣ Initializing AutoCreateUserAsync...");

            // Use delegation identity if available, otherwise use Ed25519 identity
            IIdentity identity;
            if (mDelegationIdentity != null)
            {
                identity = mDelegationIdentity;
                Debug.Log("✅ Using DelegationIdentity");
            }
            else
            {
                identity = mEd25519Identity;
                Debug.Log("✅ Using Ed25519Identity");
            }

            // Create agent with proper configuration
            var agent = new HttpAgent(identity);

            var canisterId = Principal.FromText(greetBackendCanister);
            var client = new GreetBackendCanisterApiClient(agent, canisterId);
            
            try
            {
                // Get root key first and verify it
                var rootKey = await agent.GetRootKeyAsync();
                if (rootKey == null)
                {
                    throw new Exception("Failed to get root key");
                }
                Debug.Log($"✅ Root Key: {rootKey}");
                Debug.Log("✅ GreetBackendCanisterApiClient created.");

                Debug.Log("5️⃣ Fetching user principal...");
                string userPrincipalString;
                try
                {
                    var principalTask = client.GetPrincipal();
                    Debug.Log("🔄 Initiated GetPrincipal call...");
                    userPrincipalString = await Task.WhenAny(principalTask, Task.Delay(15000)) == principalTask
                        ? principalTask.Result
                        : throw new TimeoutException("GetPrincipal timed out after 15s");
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

                Debug.Log("🔄 Calling create_user on canister...");
                const int maxRetries = 5;
                bool createUserSuccess = false;

                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        // Create a new agent with the same identity for each attempt
                        var freshAgent = new HttpAgent(identity);

                        // Get fresh root key
                        var freshRootKey = await freshAgent.GetRootKeyAsync();
                        if (freshRootKey == null)
                        {
                            throw new Exception("Failed to get fresh root key");
                        }
                        Debug.Log($"✅ Refreshed Root Key for attempt {attempt}");

                        // Create new client with fresh agent
                        var freshClient = new GreetBackendCanisterApiClient(freshAgent, canisterId);
                        
                        Result13 reply = await freshClient.CreateUser(userPrincipal, uuid);
                        Debug.Log($"✅ create_user response: {reply}");
                        createUserSuccess = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"❌ Attempt {attempt}/{maxRetries} Failed to call create_user: {ex.Message}");
                        if (attempt == maxRetries)
                        {
                            Debug.LogWarning("⚠ Max retries reached. Skipping create_user...");
                        }
                        else
                        {
                            // Wait before retry
                            await Task.Delay(1000 * attempt);
                        }
                    }
                }

                if (!createUserSuccess)
                {
                    Debug.LogWarning("⚠ create_user failed, but continuing with anonymous access...");
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

        public async Task TestGetAllUsers()
        {
            try
            {
                Debug.Log("🔄 Testing GetAllUsers...");
                
                // Use delegation identity if available, otherwise use Ed25519 identity
                IIdentity identity;
                if (mDelegationIdentity != null)
                {
                    identity = mDelegationIdentity;
                    Debug.Log("✅ Using DelegationIdentity for GetAllUsers");
                }
                else
                {
                    identity = mEd25519Identity;
                    Debug.Log("✅ Using Ed25519Identity for GetAllUsers");
                }
                
                // Create fresh agent
                var agent = new HttpAgent(identity);

                var canisterId = Principal.FromText(greetBackendCanister);

                // Get fresh root key
                var rootKey = await agent.GetRootKeyAsync();
                if (rootKey == null)
                {
                    throw new Exception("Failed to get root key for GetAllUsers");
                }
                Debug.Log($"✅ Refreshed Root Key for GetAllUsers");

                // Create new client with fresh agent
                var client = new GreetBackendCanisterApiClient(agent, canisterId);

                // Using static values for testing
                var chunkSize = UnboundedUInt.FromUInt64(1); // Nat: Get 1 user per page
                var pageNo = UnboundedUInt.FromUInt64(1);    // Nat: Get first page

                var result = await client.GetAllUsers(chunkSize, pageNo);
                Debug.Log($"✅ GetAllUsers result: {result}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in TestGetAllUsers: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
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