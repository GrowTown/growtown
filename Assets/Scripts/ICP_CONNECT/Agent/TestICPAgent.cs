using UnityEngine;
using UnityEngine.UI;
using EdjCase.ICP.Agent;
using EdjCase.ICP.Agent.Agents;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Candid.Models;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering;

namespace IC.GameKit
{
    public class TestICPAgent : MonoBehaviour
    {
        public Button exitButton;
        public string sceneToLoad = "GameScene";         
        public string greetFrontend = "https://7kl52-gyaaa-aaaac-ahmgq-cai.icp0.io/";
        public string greetBackendCanister = "7nk3o-laaaa-aaaac-ahmga-cai";
        Ed25519Identity mEd25519Identity = null;
        DelegationIdentity mDelegationIdentity = null;
        private bool sceneLoaded = false;
        private bool userCreated = false;
        private int debugCounter = 0;

        public Ed25519Identity TestIdentity => mEd25519Identity;

        internal DelegationIdentity DelegationIdentity
        {
            get => mDelegationIdentity;
            set
            {
                if (mDelegationIdentity == null)
                {
                    mDelegationIdentity = value;
                    EnableButtons();
                }
            }
        }

        void Start()
        {
            try
            {
                WasmtimeLoader.LoadWasmtime();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Error loading Wasmtime: {e.Message}");
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(ExitGame);
            }
            else
            {
                Debug.LogWarning("⚠️ Exit button is not assigned.");
            }

            mEd25519Identity = Ed25519Identity.Generate();
        }

        public void StartGameFun()
        {
            if (!SceneManager.GetActiveScene().name.Equals(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }

        void ExitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        internal async void EnableButtons()
        {
            if (mDelegationIdentity != null && !sceneLoaded)
            {
                sceneLoaded = true;  // Prevent re-entry
                DebugWithCounter("✅ Delegation Identity is set. Calling AutoCreateUser...");
                await AutoCreateUser();

                if (!SceneManager.GetActiveScene().name.Equals(sceneToLoad))
                {
                    if (!CreateSafeRenderTexture(Screen.width, Screen.height))
                    {
                        DebugWithCounter("⚠️ RenderTexture creation failed. Proceeding without it.");
                    }
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }

        private async Task AutoCreateUser()
        {
            if (DelegationIdentity == null)
            {
                DebugWithCounter("❌ DelegationIdentity is NULL, API call cannot proceed!");
                return;
            }

            if (userCreated)
            {
                DebugWithCounter("ℹ️ User already created. Skipping AutoCreateUser.");
                return;
            }

            try
            {
                var agent = new HttpAgent(DelegationIdentity);
                var canisterId = Principal.FromText(greetBackendCanister);
                var client = new GreetingClient.GreetingClient(agent, canisterId);
                API_Manager.Instance.Initialize(client);

                DebugWithCounter("🔄 Fetching user principal from Greet()...");
                var getPrincipalTask = client.GetPrinicpal();
                if (await Task.WhenAny(getPrincipalTask, Task.Delay(5000)) != getPrincipalTask)
                {
                    DebugWithCounter("❌ Timeout while fetching principal.");
                    return;
                }

                string userPrincipalString = await getPrincipalTask;
                if (string.IsNullOrEmpty(userPrincipalString))
                {
                    DebugWithCounter("❌ Received null or empty principal string.");
                    return;
                }

                Principal userPrincipal = Principal.FromText(userPrincipalString);
                DebugWithCounter($"✅ Principal response: {userPrincipal}");
                DataTransfer.UserPrincipal = userPrincipal.ToString();

                string uuid = GenerateUUID();
                DebugWithCounter($"✅ Generated UUID: {uuid}");

                CandidArg arg = CandidArg.FromCandid(
                    CandidTypedValue.Principal(userPrincipal),
                    CandidTypedValue.Text(uuid)
                );

                var createUserTask = agent.CallAndWaitAsync(canisterId, "create_user", arg);
                if (await Task.WhenAny(createUserTask, Task.Delay(5000)) != createUserTask)
                {
                    DebugWithCounter("❌ Timeout while creating user.");
                    return;
                }

                CandidArg reply = await createUserTask;
                DebugWithCounter($"✅ create_user response: {reply}");

                userCreated = true;  // Prevent re-creating the user
            }
            catch (Exception e)
            {
                DebugWithCounter($"❌ Failed to create user: {e.Message}");
            }
        }

        private string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static class WasmtimeLoader
        {
            public static void LoadWasmtime()
            {
                try
                {
                    string pluginPath = Application.dataPath + "/Plugins/libwasmtime.dylib";
                    IntPtr handle = dlopen(pluginPath, RTLD_NOW);

                    if (handle == IntPtr.Zero)
                    {
                        throw new Exception(Marshal.PtrToStringAnsi(dlerror()));
                    }

                    Debug.Log("✅ Wasmtime successfully loaded!");
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ Failed to load Wasmtime: {e.Message}");
                }
            }

            private const int RTLD_NOW = 2;

            [DllImport("libdl")]
            private static extern IntPtr dlopen(string path, int flag);

            [DllImport("libdl")]
            private static extern IntPtr dlerror();
        }

        private bool CreateSafeRenderTexture(int width, int height)
        {
            try
            {
                GraphicsFormat preferredFormat = GraphicsFormat.R16G16B16A16_SFloat;

                if (!SystemInfo.IsFormatSupported(preferredFormat, FormatUsage.Render))
                {
                    DebugWithCounter("⚠️ R16G16B16A16_SFloat not supported, falling back to R8G8B8A8_UNorm.");
                    preferredFormat = GraphicsFormat.R8G8B8A8_UNorm;
                }

                RenderTextureDescriptor descriptor = new RenderTextureDescriptor(width, height)
                {
                    graphicsFormat = preferredFormat,
                    depthBufferBits = 24,
                    msaaSamples = 1,
                    sRGB = true,
                    useMipMap = false,
                    autoGenerateMips = false
                };

#if UNITY_ANDROID || UNITY_WEBGL
                descriptor.msaaSamples = SystemInfo.supportsMultisampleAutoResolve ? 2 : 1;
                descriptor.sRGB = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
#endif

                RenderTexture renderTexture = new RenderTexture(descriptor);
                if (renderTexture != null)
                {
                    renderTexture.Create();
                    if (renderTexture.IsCreated())
                    {
                        DebugWithCounter("✅ RenderTexture created successfully.");
                        return true;
                    }
                    else
                    {
                        DebugWithCounter("❌ Failed to create RenderTexture.");
                    }
                }
                else
                {
                    DebugWithCounter("❌ RenderTexture is null after creation attempt.");
                }
            }
            catch (Exception ex)
            {
                DebugWithCounter($"❌ Exception during RenderTexture creation: {ex.Message}");
            }

            return false;
        }

        private void DebugWithCounter(string message)
        {
            debugCounter++;
            Debug.Log($"[{debugCounter}] {message}");
        }
    }
}
