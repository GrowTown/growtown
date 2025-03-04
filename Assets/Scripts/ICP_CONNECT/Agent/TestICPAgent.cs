using UnityEngine;
using UnityEngine.UI;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GrowTownGameScene";         
        public string greetFrontend = "https://s6dkc-nyaaa-aaaac-albta-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";

        Ed25519Identity mEd25519Identity = null;
        DelegationIdentity mDelegationIdentity = null;

        public Ed25519Identity TestIdentity => mEd25519Identity;

        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                mDelegationIdentity = value;
                EnableButtons();
            }
        }

        private bool isSceneLoading = false; // Track scene loading state

        void Awake()
        {
            // Ensure only one instance exists
            TestICPAgent[] agents = FindObjectsOfType<TestICPAgent>();
            if (agents.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }

        void Start()
        {
#if UNITY_WEBGL
            Debug.Log("ℹ️ Skipping Wasmtime on WebGL.");
#else
            WasmtimeLoader.LoadWasmtime();
#endif
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
                Debug.LogWarning("Scene " + sceneToLoad + " is already loaded, loading, or rendering failed. Skipping load.");
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

        public void EnableButtons()
        {
            if (mDelegationIdentity != null)
            {
                Debug.Log("✅ Delegation Identity is set. Calling AutoCreateUser...");
                AutoCreateUser();
            }
            else
            {
                Debug.LogWarning("⚠ DelegationIdentity is null. Cannot enable buttons.");
            }
        }

        private async void AutoCreateUser()
        {
            if (DelegationIdentity == null)
            {
                Debug.LogError("❌ DelegationIdentity is NULL, API call cannot proceed!");
                StartGameFun(); // Fallback, but check for rendering and recursion
                return;
            }
            var agent = new HttpAgent(DelegationIdentity);
            var canisterId = Principal.FromText(greetBackendCanister);
            var client = new GreetingClient.GreetingClient(agent, canisterId);

            // Initialize API_Manager explicitly
            API_Manager.Instance.Initialize(client);

            try
            {
                Debug.Log("🔄 Fetching user principal...");
                string userPrincipalString = await client.GetPrincipal();
                Principal userPrincipal;
                try
                {
                    userPrincipal = Principal.FromText(userPrincipalString);
                    Debug.LogError($" Principal format: {userPrincipalString}");
                    DataTransfer.UserPrincipal = userPrincipal.ToString();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ Invalid Principal format: {userPrincipalString}. Error: {ex.Message}");
                    StartGameFun(); // Fallback, but check for rendering and recursion
                    return;
                }
                string uuid = GenerateUUID();
                Debug.Log($"✅ Principal response: {userPrincipal}");
                Debug.Log($"✅ Generated UUID: {uuid}");
                CandidArg arg = CandidArg.FromCandid(
                    CandidTypedValue.Principal(userPrincipal),
                    CandidTypedValue.Text(uuid)
                );
                Debug.Log("🔄 Calling create_user on canister...");
                CandidArg reply = await agent.CallAsynchronousAndWaitAsync(canisterId, "create_user", arg);
                Debug.Log($"✅ create_user response: {reply}");
                await API_Manager.Instance.FetchCurrentUserCollections(); // Fetch collections after user creation
                StartGameFun(); // Load scene only once after success, if rendering is stable
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to create user: {e.Message}\nStackTrace: {e.StackTrace}");
                StartGameFun(); // Fallback, but check for rendering and recursion
            }
        }

        private string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }

        // Helper method to check if rendering has failed (based on logcat errors)
        private bool IsRenderingFailed()
        {
            // This is a simple check; in a real scenario, you might track errors or use Unity’s debug logs
            return Application.HasProLicense() && (Screen.currentResolution.width == 0 || Screen.currentResolution.height == 0);
        }
    }

    public static class WasmtimeLoader
    {
#if UNITY_ANDROID
        private const string WasmtimeLib = "wasmtime";
#elif UNITY_IOS
        private const string WasmtimeLib = "__Internal";
#elif UNITY_STANDALONE_OSX
        private const string WasmtimeLib = "libwasmtime";
#else
        private const string WasmtimeLib = "unsupported";
#endif

        [DllImport(WasmtimeLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr wasmtime_config_new();

        public static void LoadWasmtime()
        {
#if UNITY_WEBGL
            Debug.Log("ℹ️ Wasmtime not supported on WebGL. Consider WebAssembly alternative.");
#else
            try
            {
                IntPtr config = wasmtime_config_new();
                if (config == IntPtr.Zero)
                {
                    Debug.LogError("❌ Wasmtime loaded but initialization failed.");
                    return;
                }
                Debug.Log("✅ Wasmtime successfully loaded!");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to load Wasmtime: {e.Message}");
            }
#endif
        }
    }
}