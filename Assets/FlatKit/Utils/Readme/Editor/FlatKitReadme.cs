using System;
using ExternalPropertyAttributes;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global

namespace FlatKit {
#if FLAT_KIT_DEV
[CreateAssetMenu(fileName = "Readme", menuName = "FlatKit/Internal/Readme", order = 0)]
#endif // FLAT_KIT_DEV

[ExecuteAlways]
public class FlatKitReadme : ScriptableObject {
    [NonSerialized, ShowNonSerializedField]
    public bool FlatKitInstalled = false;

    [NonSerialized, ShowNonSerializedField]
    public readonly string FlatKitVersion = "2.1.8";

    [NonSerialized, ShowNonSerializedField]
    public string UrpInstalled = "Unknown";

    [NonSerialized, ShowNonSerializedField]
    public string UrpVersionInstalled = "N/A";

    [NonSerialized, ShowNonSerializedField]
    public string UnityVersion = Application.unityVersion;

    [ReadOnly, HideIf("_"),
     InfoBox("If the `FlatKitInstalled` field below is not checked, " +
           "you first need to unpack Flat Kit depending on your " +
           "project's Render Pipeline. See buttons below.", EInfoBoxType.Warning)]
    public bool _ = true;

    [Button("Refresh data")]
    private void Refresh() {
        _urpInstalled = false;
        FlatKitInstalled = false;
        _packageManagerError = false;

        PackageCollection packages = GetPackageList();
        foreach (PackageInfo p in packages) {
            if (p.name == UrpPackageID) {
                _urpInstalled = true;
                UrpVersionInstalled = p.version;
            }
        }

        string path = AssetDatabase.GUIDToAssetPath(StylizedShaderGuid.ToString());
        var flatKitSourceAsset = AssetDatabase.LoadAssetAtPath<Shader>(path);
        FlatKitInstalled = flatKitSourceAsset != null;

        UnityVersion = Application.unityVersion;

        UrpInstalled = _packageManagerError ? "Package Manager error, please re-install URP" :
            _urpInstalled ? "yes" : "no";
    }

    [Button("Unpack Flat Kit for URP")]
    private void UnpackFlatKitUrp() {
        string path = AssetDatabase.GUIDToAssetPath(UnityPackageUrpGuid.ToString());
        if (path == null) {
            Debug.LogError("[Flat Kit] Could not find the URP package.");
        }
        else {
            AssetDatabase.ImportPackage(path, false);
        }
    }

    [Button("Unpack Flat Kit for Built-in RP")]
    private void UnpackFlatKitBuiltInRP() {
        string path = AssetDatabase.GUIDToAssetPath(UnityPackageBuiltInGuid.ToString());
        if (path == null) {
            Debug.LogError("[Flat Kit] Could not find the Built-in RP package.");
        }
        else {
            AssetDatabase.ImportPackage(path, false);
        }
    }

    [Button("Configure project for URP")]
    private void ConfigureUrp() {
        string path = AssetDatabase.GUIDToAssetPath(UrpPipelineAssetGuid.ToString());
        if (path == null) {
            Debug.LogError("[Flat Kit] Couldn't find the URP pipeline asset. Have you unpacked the URP package?");
            return;
        }

        var pipelineAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(path);
        if (pipelineAsset == null) {
            Debug.LogError("[Flat Kit] Couldn't load the URP pipeline asset.");
            return;
        }

        GraphicsSettings.renderPipelineAsset = pipelineAsset;
        QualitySettings.renderPipeline = pipelineAsset;

        Debug.Log(
            "<b>[Flat Kit]</b> Set the render pipeline asset in the Graphics and Quality settings to the " +
            "Flat Kit example.");
    }

    [Button("Configure project for Built-in RP")]
    private void ConfigureBuiltIn() {
        GraphicsSettings.renderPipelineAsset = null;
        QualitySettings.renderPipeline = null;
        Debug.Log("<b>[Flat Kit]</b> Cleared the render pipeline asset in the Graphics and Quality settings.");
    }

    [Button("Copy debug info to clipboard")]
    private void CopyDebugInfoToClipboard() {
        EditorGUIUtility.systemCopyBuffer =
            $"Flat Kit: {FlatKitVersion}, URP: {UrpVersionInstalled}, Unity: {UnityVersion}";
        Debug.Log("<b>Flat Kit</b> info copied to clipboard.");
    }

    [Button("Open support ticket")]
    private void OpenSupportTicket() {
        Application.OpenURL("https://github.com/Dustyroom/flat-kit-doc/issues/new/choose");
    }

    // ---------------------------------------------------------

    private const string UrpPackageID = "com.unity.render-pipelines.universal";

    private static readonly GUID StylizedShaderGuid = new GUID("bee44b4a58655ee4cbff107302a3e131");
    private static readonly GUID UnityPackageUrpGuid = new GUID("41e59f562b69648719f2424c438758f3");
    private static readonly GUID UnityPackageBuiltInGuid = new GUID("f4227764308e84f89a765fbf315e2945");
    private static readonly GUID UrpPipelineAssetGuid = new GUID("ecbd363870e07455ea237f5753668d30");

    private bool _urpInstalled;
    private bool _packageManagerError;

    private void OnEnable() {
        Refresh();
    }

    private PackageCollection GetPackageList() {
        var listRequest = Client.List(true);

        while (listRequest.Status == StatusCode.InProgress) continue;

        if (listRequest.Status == StatusCode.Failure) {
            _packageManagerError = true;
            Debug.LogWarning("[Flat Kit] Failed to get packages from Package Manager.");
            return null;
        }

        return listRequest.Result;
    }
}
}