using UnityEngine;
using UnityEditor;

public class AppCenterSettingsContext : ScriptableObject
{
    public const string AppCenterPath = "Assets";
    private const string SettingsPath = AppCenterPath + "/AppCenter/AppCenterSettings.asset";
    private const string AdvancedSettingsPath = AppCenterPath + "/AppCenter/AppCenterSettingsAdvanced.asset";

    public static AppCenterSettings SettingsInstance
    {
        get
        {
            // No need to lock because this can only be accessed from the main thread.
            var instance = AssetDatabase.LoadAssetAtPath<AppCenterSettings>(SettingsPath);
            if (instance == null)
            {
                instance = CreateInstance<AppCenterSettings>();
                AssetDatabase.CreateAsset(instance, SettingsPath);
                AssetDatabase.SaveAssets();
            }
            return instance;
        }
    }

    public static AppCenterSettingsAdvanced SettingsInstanceAdvanced
    {
        get
        {
            // No need to lock because this can only be accessed from the main thread.
            return AssetDatabase.LoadAssetAtPath<AppCenterSettingsAdvanced>(AdvancedSettingsPath);      
        }
    }

    public static AppCenterSettingsAdvanced CreateSettingsInstanceAdvanced()
    {
        var instance = AssetDatabase.LoadAssetAtPath<AppCenterSettingsAdvanced>(AdvancedSettingsPath);
        if (instance == null)
        {
            instance = CreateInstance<AppCenterSettingsAdvanced>();
            AssetDatabase.CreateAsset(instance, AdvancedSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return instance;
    }

    public static void DeleteSettingsInstanceAdvanced()
    {
        AssetDatabase.DeleteAsset(AdvancedSettingsPath);
    }
}
