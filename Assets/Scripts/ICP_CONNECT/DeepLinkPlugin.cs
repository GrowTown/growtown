using UnityEngine;
using EdjCase.ICP.Agent;
using EdjCase.ICP.Agent.Identities;
using EdjCase.ICP.Agent.Models;
using EdjCase.ICP.Candid.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IC.GameKit
{
    public class DeepLinkPlugin : MonoBehaviour
    {
        TestICPAgent mTestICPAgent = null;

        private void Awake()
        {
#if UNITY_ANDROID || UNITY_IOS
            Debug.Log(":white_check_mark: Registering deep link event for Native App.");
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                Debug.Log("🔗 App opened via deep link: " + Application.absoluteURL);
                OnDeepLinkActivated(Application.absoluteURL);
            }
#endif
        }

        public void Start()
        {
            mTestICPAgent = gameObject.GetComponent<TestICPAgent>();
            if (mTestICPAgent == null)
            {
                Debug.LogError("❌ TestICPAgent component not found on GameObject.");
            }
        }

        private bool IsRunningOnMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        public void OpenBrowser()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log(":arrows_counterclockwise: WebGL: Redirecting to login...");
            UnityLogin();
#elif UNITY_ANDROID || UNITY_IOS
            if (mTestICPAgent == null || mTestICPAgent.TestIdentity == null)
            {
                Debug.LogError(":x: TestICPAgent or TestIdentity is NULL.");
                return;
            }

            string route = IsRunningOnMobile() ? "app" : "";
            string sessionKeyHex = ToHexString(mTestICPAgent.TestIdentity.PublicKey.ToDerEncoding());
            string targetUrl = $"{mTestICPAgent.greetFrontend}{route}?sessionkey={sessionKeyHex}";

            Debug.Log($"🔗 Opening browser in Native App: {targetUrl}");
            Application.OpenURL(targetUrl);
#endif
        }

        public void OnDeepLinkActivated(string url)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(":x: Deep link URL is empty.");
                return;
            }

            Debug.Log($"✅ Deep link activated: {url}");

            const string kDelegationParam = "delegation=";
            var indexOfDelegation = url.IndexOf(kDelegationParam);
            if (indexOfDelegation == -1)
            {
                Debug.LogError("❌ Cannot find delegation parameter in deep link.");
                return;
            }

            string delegationString = HttpUtility.UrlDecode(url.Substring(indexOfDelegation + kDelegationParam.Length));
            var delegationIdentity = ConvertJsonToDelegationIdentity(delegationString);

            if (delegationIdentity == null)
            {
                Debug.LogError("❌ Failed to convert JSON into DelegationIdentity.");
                return;
            }

            Debug.Log("✅ Delegation identity successfully created!");

           if (mTestICPAgent != null)
{
    mTestICPAgent.DelegationIdentity = delegationIdentity;
    mTestICPAgent.EnableButtons();
    mTestICPAgent.StartGameFun(); // ✅ Trigger game start after successful login
}
else
{
    Debug.LogError("❌ TestICPAgent is NULL; cannot assign delegation identity!");
}

#endif
        }

        internal DelegationIdentity ConvertJsonToDelegationIdentity(string jsonDelegation)
        {
            var delegationChainModel = JsonConvert.DeserializeObject<DelegationChainModel>(jsonDelegation);
            if (delegationChainModel == null || delegationChainModel.delegations.Length == 0)
            {
                Debug.LogError("❌ Invalid delegation chain.");
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
            return new DelegationIdentity(mTestICPAgent.TestIdentity, new DelegationChain(chainPublicKey, delegations));
        }

        private static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        private static byte[] FromHexString(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid hex string length.");
            }

            return Enumerable.Range(0, hex.Length / 2)
                .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                .ToArray();
        }
    }
}
