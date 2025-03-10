using UnityEngine;
using System.Runtime.InteropServices;
using EdjCase.ICP.Agent;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Agent.Models;
using EdjCase.ICP.Candid.Models;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace IC.GameKit
{
    public class DeepLinkPlugin : MonoBehaviour
    {
        private TestICPAgent mTestICPAgent = null;
        private static DeepLinkPlugin instance;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SendMessageToJS(string eventName);

        [DllImport("__Internal")]
        private static extern void UnityLogin(string sessionKey);

        [DllImport("__Internal")]
        private static extern void SendSessionKey(string sessionKey);
#endif

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("⚠ Duplicate DeepLinkPlugin found. Destroying this instance.");
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            mTestICPAgent = FindObjectOfType<TestICPAgent>();
            if (mTestICPAgent == null)
            {
                Debug.LogError("❌ TestICPAgent not found in scene during Awake!");
            }
            Debug.Log($"✅ DeepLinkPlugin Awake on GameObject: {gameObject.name}");

#if UNITY_ANDROID || UNITY_IOS
            Debug.Log("✅ Registering deep link event for Native App.");
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                Debug.Log("🔗 App opened via deep link: " + Application.absoluteURL);
                OnDeepLinkActivated(Application.absoluteURL);
            }
#endif
        }

        public void OpenBrowser()
        {
            if (mTestICPAgent == null || mTestICPAgent.TestIdentity == null)
            {
                Debug.LogError("❌ TestICPAgent or TestIdentity is NULL. Cannot initiate login.");
                return;
            }

            string sessionKeyHex = ToHexString(mTestICPAgent.TestIdentity.PublicKey.ToDerEncoding());
            string route = IsRunningOnMobile() ? "/app" : "/";
            string maxTimeToLive = "604800000000000"; // 1 week in nanoseconds
            string targetUrl = $"{mTestICPAgent.greetFrontend}{route}?sessionkey={sessionKeyHex}&maxTimeToLive={maxTimeToLive}";

#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log($"🔄 WebGL: Initiating login flow with session key: {sessionKeyHex} at route: {route}");
            UnityLogin(sessionKeyHex);
            SendSessionKey(sessionKeyHex);
            Debug.Log($"🔄 WebGL: Session key {sessionKeyHex} sent to React.");
#elif UNITY_ANDROID || UNITY_IOS
            Debug.Log($"🔗 Opening browser for Native App login: {targetUrl}");
            Application.OpenURL(targetUrl);
#endif
        }

        public void OnDeepLinkActivated(string url)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("❌ Deep link URL is empty.");
                return;
            }

            Debug.Log($"✅ Deep link activated: {url}");
            string delegationString = ExtractDelegationFromUrl(url);
            if (string.IsNullOrEmpty(delegationString))
            {
                Debug.LogError("❌ Failed to extract delegation from deep link.");
                return;
            }

            var delegationIdentity = ConvertJsonToDelegationIdentity(delegationString);
            if (delegationIdentity == null)
            {
                Debug.LogError("❌ Failed to convert JSON into DelegationIdentity.");
                return;
            }

            Debug.Log("✅ Delegation identity successfully created from deep link!");
            StartCoroutine(ProcessDelegationCoroutine(delegationIdentity));
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        public void ReceiveWebGLDelegation(string delegationJson)
        {
            Debug.Log($"✅ WebGL Delegation Received on GameObject {gameObject.name}: {delegationJson}");
            var delegationIdentity = ConvertJsonToDelegationIdentity(delegationJson);
            if (delegationIdentity != null)
            {
                Debug.Log("✅ WebGL Delegation identity successfully created!");
                StartCoroutine(ProcessDelegationCoroutine(delegationIdentity));
            }
            else
            {
                Debug.LogError("❌ Failed to process WebGL delegation. Invalid JSON or delegation data.");
            }
        }

        public static void ReceiveWebGLDelegationStatic(string delegationJson)
        {
            if (instance != null)
            {
                instance.ReceiveWebGLDelegation(delegationJson);
            }
            else
            {
                Debug.LogError("❌ No active DeepLinkPlugin instance found in scene! Cannot process delegation.");
                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene != "GrowTownGameScene" && !IsRenderingFailed())
                {
                    SceneManager.LoadScene("GrowTownGameScene");
                }
            }
        }
#endif

        private System.Collections.IEnumerator ProcessDelegationCoroutine(DelegationIdentity delegationIdentity)
        {
            ProcessDelegation(delegationIdentity);
            yield return null; // Yield to allow TestICPAgent coroutine to start
        }

        private void ProcessDelegation(DelegationIdentity delegationIdentity)
        {
            Debug.Log("🔄 Processing delegation...");
            if (mTestICPAgent == null)
            {
                mTestICPAgent = FindObjectOfType<TestICPAgent>();
            }

            if (mTestICPAgent != null)
            {
                mTestICPAgent.DelegationIdentity = delegationIdentity; // Triggers processing in TestICPAgent
            }
            else
            {
                Debug.LogError("❌ TestICPAgent is NULL; cannot assign delegation identity! Forcing scene load...");
                string currentScene = SceneManager.GetActiveScene().name;
                if (currentScene != "GrowTownGameScene" && !IsRenderingFailed())
                {
                    SceneManager.LoadScene("GrowTownGameScene");
                }
            }
        }

        private static bool IsRenderingFailed()
        {
            return Application.HasProLicense() && (Screen.currentResolution.width == 0 || Screen.currentResolution.height == 0);
        }

        private string ExtractDelegationFromUrl(string url)
        {
            const string kDelegationParam = "delegation=";
            var indexOfDelegation = url.IndexOf(kDelegationParam);
            if (indexOfDelegation == -1)
            {
                Debug.LogError("❌ No 'delegation=' parameter found in URL: " + url);
                return null;
            }

            string delegation = UnityWebRequest.UnEscapeURL(url.Substring(indexOfDelegation + kDelegationParam.Length));
            Debug.Log("✅ Extracted delegation: " + delegation);
            return delegation;
        }

        internal DelegationIdentity ConvertJsonToDelegationIdentity(string jsonDelegation)
        {
            try
            {
                var delegationChainModel = JsonConvert.DeserializeObject<DelegationChainModel>(jsonDelegation);
                if (delegationChainModel == null || delegationChainModel.delegations.Length == 0)
                {
                    Debug.LogError("❌ Invalid delegation chain: null or empty delegations.");
                    return null;
                }

                var delegations = new List<SignedDelegation>();
                foreach (var signedDelegationModel in delegationChainModel.delegations)
                {
                    var pubKey = SubjectPublicKeyInfo.FromDerEncoding(FromHexString(signedDelegationModel.delegation.pubkey));
                    var expiration = ICTimestamp.FromNanoSeconds(Convert.ToUInt64(signedDelegationModel.delegation.expiration, 16));

                    if (expiration.NanoSeconds < ICTimestamp.Now().NanoSeconds)
                    {
                        Debug.LogError("❌ Delegation has expired.");
                        return null;
                    }

                    var delegation = new Delegation(pubKey, expiration);
                    var signature = FromHexString(signedDelegationModel.signature);
                    delegations.Add(new SignedDelegation(delegation, signature));
                }

                var chainPublicKey = SubjectPublicKeyInfo.FromDerEncoding(FromHexString(delegationChainModel.publicKey));
                Debug.Log($"🔍 Delegation chain public key: {ToHexString(chainPublicKey.ToDerEncoding())}");
                return new DelegationIdentity(mTestICPAgent.TestIdentity, new DelegationChain(chainPublicKey, delegations));
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to convert JSON to DelegationIdentity: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return null;
            }
        }

        private static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private static byte[] FromHexString(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hex string length or null.");
            }

            return Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();
        }

        private bool IsRunningOnMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }
    }
}