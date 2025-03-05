using UnityEngine;
using UnityEngine.UI;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GrowTownGameScene";
        public string greetFrontend = "https://s6dkc-nyaaa-aaaac-albta-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";

        private Ed25519Identity mEd25519Identity = null;
        private DelegationIdentity mDelegationIdentity = null;

        public Ed25519Identity TestIdentity => mEd25519Identity;

        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                mDelegationIdentity = value;
                StartCoroutine(EnableButtonsCoroutine());
            }
        }

        private bool isSceneLoading = false;

        void Awake()
        {
            TestICPAgent[] agents = FindObjectsOfType<TestICPAgent>();
            if (agents.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
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
                Debug.LogWarning("⚠ DelegationIdentity is null. Cannot enable buttons.");
                StartGameFun();
            }
        }

        private async Task AutoCreateUserAsync()
        {
            Debug.Log("1️⃣ Creating HttpAgent...");
            // Use default configuration with mainnet host, no custom public key
var agent = new HttpAgent(DelegationIdentity);

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
                    Debug.LogError($"❌ Failed to fetch principal: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    Debug.LogWarning("⚠ Falling back to anonymous principal...");
                    userPrincipalString = "2vxsx-fae"; // Anonymous principal
                }

                Principal userPrincipal;
                try
                {
                    userPrincipal = Principal.FromText(userPrincipalString);
                    Debug.Log($"✅ Principal format: {userPrincipalString}");
                    DataTransfer.UserPrincipal = userPrincipal.ToString();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Invalid principal format: {userPrincipalString}. Error: {ex.Message}");
                    Debug.LogWarning("⚠ Using anonymous principal as fallback...");
                    userPrincipal = Principal.Anonymous();
                    DataTransfer.UserPrincipal = userPrincipal.ToString();
                }

                string uuid = Guid.NewGuid().ToString();
                Debug.Log($"✅ Principal: {userPrincipal}");
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
                    Debug.LogError($"❌ Failed to call create_user: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    Debug.LogWarning("⚠ Skipping create_user due to error...");
                }

                Debug.Log("9️⃣ Fetching user collections...");
                try
                {
                    await API_Manager.Instance.FetchCurrentUserCollections();
                    Debug.Log("✅ Collections fetched successfully.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Failed to fetch collections: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    Debug.LogWarning("⚠ Proceeding without collections...");
                }

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