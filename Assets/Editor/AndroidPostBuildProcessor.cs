#if UNITY_ANDROID
using System;
using System.IO;
using System.Xml;
using UnityEditor.Android;

namespace IC.GameKit
{
    public class AndroidPostProcessor : IPostGenerateGradleAndroidProject
    {
        const string kAndroidNamespaceURI = "http://schemas.android.com/apk/res/android";

        // Android URL Scheme for your canister
        const string kURLScheme = "https";
        const string kURLHost = "7nk3o-laaaa-aaaac-ahmga-cai.icp0.io";
        const string kURLPath = "/authorize";

        public int callbackOrder { get { return 0; } }

        public void OnPostGenerateGradleAndroidProject(string projectPath)
        {
            InjectAndroidManifest(projectPath);
            UpdateGradleProperties(projectPath);
        }

        private void InjectAndroidManifest(string projectPath)
        {
            var manifestPath = projectPath + "/src/main/AndroidManifest.xml";
            if (!File.Exists(manifestPath))
                throw new FileNotFoundException(manifestPath + " doesn't exist.");

            var manifestXmlDoc = new XmlDocument();
            manifestXmlDoc.Load(manifestPath);

            // Check if the URL host already exists to avoid duplicates
            if (manifestXmlDoc.InnerXml.Contains(kURLHost))
                return;

            // Add required permissions
            AddRequiredPermissions(manifestXmlDoc);

            // Add intent filter for App Links
            AppendAndroidIntentFilter(manifestPath, manifestXmlDoc);

            manifestXmlDoc.Save(manifestPath);
        }

        private void AddRequiredPermissions(XmlDocument xmlDoc)
        {
            var manifestNode = xmlDoc.SelectSingleNode("manifest");
            if (manifestNode == null)
                throw new ArgumentException("Missing 'manifest' node in AndroidManifest.xml");

            // Add Internet permission
            var internetPermission = xmlDoc.CreateElement("uses-permission");
            internetPermission.SetAttribute("name", kAndroidNamespaceURI, "android.permission.INTERNET");
            manifestNode.AppendChild(internetPermission);

            // Add Network State permission
            var networkStatePermission = xmlDoc.CreateElement("uses-permission");
            networkStatePermission.SetAttribute("name", kAndroidNamespaceURI, "android.permission.ACCESS_NETWORK_STATE");
            manifestNode.AppendChild(networkStatePermission);
        }

        private void UpdateGradleProperties(string projectPath)
        {
            var gradlePropertiesPath = projectPath + "/gradle.properties";
            if (!File.Exists(gradlePropertiesPath))
            {
                File.WriteAllText(gradlePropertiesPath, "");
            }

            var gradleProperties = File.ReadAllText(gradlePropertiesPath);

            // Add required properties for Android builds
            if (!gradleProperties.Contains("android.useAndroidX=true"))
            {
                gradleProperties += "\nandroid.useAndroidX=true";
            }
            if (!gradleProperties.Contains("android.enableJetifier=true"))
            {
                gradleProperties += "\nandroid.enableJetifier=true";
            }
            if (!gradleProperties.Contains("org.gradle.jvmargs=-Xmx4096M"))
            {
                gradleProperties += "\norg.gradle.jvmargs=-Xmx4096M";
            }

            File.WriteAllText(gradlePropertiesPath, gradleProperties);
        }

        internal static void AppendAndroidIntentFilter(string manifestPath, XmlDocument xmlDoc)
        {
            var activityNode = xmlDoc.SelectSingleNode("manifest/application/activity");
            if (activityNode == null)
                throw new ArgumentException(string.Format("Missing 'activity' node in '{0}'.", manifestPath));

            // Add intent filter for the canister's frontend URL
            var intentFilterNode = xmlDoc.CreateElement("intent-filter");
            intentFilterNode.SetAttribute("autoVerify", kAndroidNamespaceURI, "true");

            var actionNode = xmlDoc.CreateElement("action");
            actionNode.SetAttribute("name", kAndroidNamespaceURI, "android.intent.action.VIEW");
            intentFilterNode.AppendChild(actionNode);

            var categoryNode1 = xmlDoc.CreateElement("category");
            categoryNode1.SetAttribute("name", kAndroidNamespaceURI, "android.intent.category.DEFAULT");
            intentFilterNode.AppendChild(categoryNode1);

            var categoryNode2 = xmlDoc.CreateElement("category");
            categoryNode2.SetAttribute("name", kAndroidNamespaceURI, "android.intent.category.BROWSABLE");
            intentFilterNode.AppendChild(categoryNode2);

            var dataNode = xmlDoc.CreateElement("data");
            dataNode.SetAttribute("scheme", kAndroidNamespaceURI, kURLScheme);
            dataNode.SetAttribute("host", kAndroidNamespaceURI, kURLHost);
            dataNode.SetAttribute("path", kAndroidNamespaceURI, kURLPath);
            intentFilterNode.AppendChild(dataNode);

            activityNode.AppendChild(intentFilterNode);
        }
    }
}
#endif