using PlayFab.PfEditor.EditorModels;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    [InitializeOnLoad]
    public class PlayFabEditorSettings : UnityEditor.Editor
    {
        #region panel variables
        public enum SubMenuStates
        {
            StandardSettings,
            TitleSettings,
            ApiSettings,
            Packages
        }

        public enum WebRequestType
        {
            UnityWww, // High compatability Unity api calls
            HttpWebRequest // High performance multi-threaded api calls
        }

        private static readonly List<string> BuildTargets = new List<string>();

        private static SubMenuComponent _menu = null;

        private static Dictionary<string, StudioDisplaySet> studioFoldOutStates = new Dictionary<string, StudioDisplaySet>();
        private static Vector2 _titleScrollPos = Vector2.zero;
        private static Vector2 _packagesScrollPos = Vector2.zero;
        #endregion

        #region draw calls

        private static void DrawApiSubPanel()
        {
            float labelWidth = 160;

            using (new UnityVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                using (var fwl = new FixedWidthLabel("ENABLE CLIENT API: "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.EnvDetails.isClientApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isClientApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }

                using (var fwl = new FixedWidthLabel("ENABLE ADMIN API:  "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.EnvDetails.isAdminApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isAdminApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }

                using (var fwl = new FixedWidthLabel("ENABLE SERVER API: "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.EnvDetails.isServerApiEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isServerApiEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }

                using (var fwl = new FixedWidthLabel("ENABLE REQUEST TIMES: "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled = EditorGUILayout.Toggle(PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }
            }
        }

        public static void DrawSettingsPanel()
        {
            if (PlayFabEditorDataService.IsDataLoaded)
            {
                if (_menu != null)
                {
                    _menu.DrawMenu();
                    switch ((SubMenuStates)PlayFabEditorDataService.EditorView.currentSubMenu)
                    {
                        case SubMenuStates.StandardSettings:
                            DrawStandardSettingsSubPanel();
                            break;
                        case SubMenuStates.ApiSettings:
                            DrawApiSubPanel();
                            break;
                        case SubMenuStates.TitleSettings:
                            DrawTitleSettingsSubPanel();
                            break;
                        case SubMenuStates.Packages:
                            DrawPackagesSubPanel();
                            break;
                    }
                }
                else
                {
                    RegisterMenu();
                }
            }
        }

        private static void DrawTitleSettingsSubPanel()
        {
            float labelWidth = 100;

            if (PlayFabEditorDataService.AccountDetails.studios != null && PlayFabEditorDataService.AccountDetails.studios.Count != studioFoldOutStates.Count + 1)
            {
                studioFoldOutStates.Clear();
                foreach (var studio in PlayFabEditorDataService.AccountDetails.studios)
                {
                    if (string.IsNullOrEmpty(studio.Id))
                        continue;
                    if (!studioFoldOutStates.ContainsKey(studio.Id))
                        studioFoldOutStates.Add(studio.Id, new StudioDisplaySet { Studio = studio });
                    foreach (var title in studio.Titles)
                        if (!studioFoldOutStates[studio.Id].titleFoldOutStates.ContainsKey(title.Id))
                            studioFoldOutStates[studio.Id].titleFoldOutStates.Add(title.Id, new TitleDisplaySet { Title = title });
                }
            }

            _titleScrollPos = GUILayout.BeginScrollView(_titleScrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));

            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
            {
                EditorGUILayout.LabelField("STUDIOS:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("REFRESH", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                    PlayFabEditorDataService.RefreshStudiosList();
            }

            foreach (var studio in studioFoldOutStates)
            {
                var style = new GUIStyle(EditorStyles.foldout);
                if (studio.Value.isCollapsed)
                    style.fontStyle = FontStyle.Normal;

                studio.Value.isCollapsed = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), studio.Value.isCollapsed, string.Format("{0} ({1})", studio.Value.Studio.Name, studio.Value.Studio.Titles.Length), true, PlayFabEditorHelper.uiStyle.GetStyle("foldOut_std"));
                if (studio.Value.isCollapsed)
                    continue;

                EditorGUI.indentLevel = 2;

                using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    EditorGUILayout.LabelField("TITLES:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                }
                GUILayout.Space(5);

                // draw title foldouts
                foreach (var title in studio.Value.titleFoldOutStates)
                {
                    title.Value.isCollapsed = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), title.Value.isCollapsed, string.Format("{0} [{1}]", title.Value.Title.Name, title.Value.Title.Id), true, PlayFabEditorHelper.uiStyle.GetStyle("foldOut_std"));
                    if (title.Value.isCollapsed)
                        continue;

                    EditorGUI.indentLevel = 3;
                    using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        EditorGUILayout.LabelField("SECRET KEY:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                        EditorGUILayout.TextField("" + title.Value.Title.SecretKey);
                    }

                    using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        EditorGUILayout.LabelField("URL:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("VIEW IN GAME MANAGER", PlayFabEditorHelper.uiStyle.GetStyle("textButton")))
                            Application.OpenURL(title.Value.Title.GameManagerUrl);
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUI.indentLevel = 2;
                }

                EditorGUI.indentLevel = 0;
            }
            GUILayout.EndScrollView();
        }

        private static Studio GetStudioForTitleId(string titleId)
        {
            if (PlayFabEditorDataService.AccountDetails.studios == null)
                return Studio.OVERRIDE;
            foreach (var eachStudio in PlayFabEditorDataService.AccountDetails.studios)
                if (eachStudio.Titles != null)
                    foreach (var eachTitle in eachStudio.Titles)
                        if (eachTitle.Id == titleId)
                            return eachStudio;
            return Studio.OVERRIDE;
        }

        private static void DrawStandardSettingsSubPanel()
        {
            float labelWidth = 160;

            using (new UnityVertical(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.ExpandWidth(true)))
            {
                var studio = GetStudioForTitleId(PlayFabEditorDataService.SharedSettings.TitleId);
                if (string.IsNullOrEmpty(studio.Id))
                    using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                        GUILayout.Label("You are using a TitleId to which you are not a memeber. A title administrator can approve access for your account.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));

                PlayFabGuiFieldHelper.SuperFancyDropdown(labelWidth, "STUDIO: ", studio, PlayFabEditorDataService.AccountDetails.studios, eachStudio => eachStudio.Name, OnStudioChange, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                studio = GetStudioForTitleId(PlayFabEditorDataService.SharedSettings.TitleId); // This might have changed above, so refresh it

                if (string.IsNullOrEmpty(studio.Id))
                {
                    // Override studio lets you set your own titleId
                    using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                    {
                        EditorGUILayout.LabelField("TITLE ID: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));

                        var newTitleId = EditorGUILayout.TextField(PlayFabEditorDataService.SharedSettings.TitleId, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                        if (newTitleId != PlayFabEditorDataService.SharedSettings.TitleId)
                            OnTitleIdChange(newTitleId);
                    }
                }
                else
                {
                    PlayFabGuiFieldHelper.SuperFancyDropdown(labelWidth, "TITLE ID: ", studio.GetTitle(PlayFabEditorDataService.SharedSettings.TitleId), studio.Titles, GetTitleDisplayString, OnTitleChange, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                }

                DrawPfSharedSettingsOptions(labelWidth);
            }
        }

        private static string GetTitleDisplayString(Title title)
        {
            return string.Format("[{0}] {1}", title.Id, title.Name);
        }

        private static void DrawPfSharedSettingsOptions(float labelWidth)
        {
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
            // Set the title secret key, if we're using the dropdown
            var studio = GetStudioForTitleId(PlayFabEditorDataService.SharedSettings.TitleId);
            var correctKey = studio.GetTitleSecretKey(PlayFabEditorDataService.SharedSettings.TitleId);
            var setKey = !string.IsNullOrEmpty(studio.Id) && !string.IsNullOrEmpty(correctKey);
            if (setKey)
                PlayFabEditorDataService.SharedSettings.DeveloperSecretKey = correctKey;

            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
            {
                EditorGUILayout.LabelField("DEVELOPER SECRET KEY: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.Width(labelWidth));
                using (new UnityGuiToggler(!setKey))
                    PlayFabEditorDataService.SharedSettings.DeveloperSecretKey = EditorGUILayout.TextField(PlayFabEditorDataService.SharedSettings.DeveloperSecretKey, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
            }
#endif
            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
            {
                EditorGUILayout.LabelField("REQUEST TYPE: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
                PlayFabEditorDataService.SharedSettings.WebRequestType = (WebRequestType)EditorGUILayout.EnumPopup(PlayFabEditorDataService.SharedSettings.WebRequestType, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.Height(25));
            }

            if (PlayFabEditorDataService.SharedSettings.WebRequestType == WebRequestType.HttpWebRequest)
            {
                using (var fwl = new FixedWidthLabel(new GUIContent("REQUEST TIMEOUT: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.SharedSettings.TimeOut = EditorGUILayout.IntField(PlayFabEditorDataService.SharedSettings.TimeOut, PlayFabEditorHelper.uiStyle.GetStyle("TextField"), GUILayout.MinHeight(25));
                }

                using (var fwl = new FixedWidthLabel(new GUIContent("KEEP ALIVE: "), PlayFabEditorHelper.uiStyle.GetStyle("labelStyle")))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorDataService.SharedSettings.KeepAlive = EditorGUILayout.Toggle(PlayFabEditorDataService.SharedSettings.KeepAlive, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
                }
            }

            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
            {
                EditorGUILayout.LabelField("COMPRESS API DATA: ", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"), GUILayout.MaxWidth(labelWidth));
                PlayFabEditorDataService.SharedSettings.CompressApiData = EditorGUILayout.Toggle(PlayFabEditorDataService.SharedSettings.CompressApiData, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"), GUILayout.MinHeight(25));
            }
        }

        private static void DrawPackagesSubPanel()
        {
            using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1")))
            {
                GUILayout.Label("Packages are additional PlayFab features that can be installed. Enabling a package will install the AsssetPackage; disabling will remove the package.", PlayFabEditorHelper.uiStyle.GetStyle("genTxt"));
            }

            if (PlayFabEditorSDKTools.IsInstalled && PlayFabEditorSDKTools.isSdkSupported)
            {
                float labelWidth = 245;
                _packagesScrollPos = GUILayout.BeginScrollView(_packagesScrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                using (var fwl = new FixedWidthLabel("Push Notification Plugin (Android): "))
                {
                    GUILayout.Space(labelWidth - fwl.fieldWidth);
                    PlayFabEditorPackageManager.AndroidPushPlugin = EditorGUILayout.Toggle(PlayFabEditorPackageManager.AndroidPushPlugin, PlayFabEditorHelper.uiStyle.GetStyle("Toggle"));
                }
                GUILayout.Space(5);
                using (new UnityHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear")))
                {
                    if (GUILayout.Button("VIEW GUIDE", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                    {
                        Application.OpenURL("https://github.com/PlayFab/UnitySDK/tree/master/PluginsSource/UnityAndroidPluginSource#playfab-push-notification-plugin");
                    }
                }
                GUILayout.EndScrollView();
            }
        }
        #endregion

        #region unity-like loops

        public static void Update()
        {
            BuildTargets.Clear();
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            foreach (var each in symbols)
                BuildTargets.Add(each);
        }

        public static void After()
        {
            if (PlayFabEditorDataService.EnvDetails.isAdminApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.ADMIN_API))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.ADMIN_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isAdminApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.ADMIN_API))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.ADMIN_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (PlayFabEditorDataService.EnvDetails.isServerApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.SERVER_API))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.SERVER_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isServerApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.SERVER_API))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.SERVER_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled && !BuildTargets.Contains(PlayFabEditorHelper.DEBUG_REQUEST_TIMING))
            {
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.DEBUG_REQUEST_TIMING);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (!PlayFabEditorDataService.EnvDetails.isDebugRequestTimesEnabled && BuildTargets.Contains(PlayFabEditorHelper.DEBUG_REQUEST_TIMING))
            {
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.DEBUG_REQUEST_TIMING);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

            if (!PlayFabEditorDataService.EnvDetails.isClientApiEnabled && !BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API))
            {
                Debug.Log(PlayFabEditorHelper.CLIENT_API + ":" + BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API));
                var str = AddToBuildTarget(BuildTargets, PlayFabEditorHelper.CLIENT_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }
            else if (PlayFabEditorDataService.EnvDetails.isClientApiEnabled && BuildTargets.Contains(PlayFabEditorHelper.CLIENT_API))
            {
                Debug.Log(PlayFabEditorHelper.CLIENT_API + "- Removed");
                var str = RemoveToBuildTarget(BuildTargets, PlayFabEditorHelper.CLIENT_API);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);
                PlayFabEditorDataService.SaveEnvDetails();
            }

        }

        #endregion

        #region menu and helper methods
        private static void RegisterMenu()
        {
            if (_menu != null)
                return;

            _menu = CreateInstance<SubMenuComponent>();
            _menu.RegisterMenuItem("PROJECT", OnStandardSetttingsClicked);
            _menu.RegisterMenuItem("STUDIOS", OnTitleSettingsClicked);
            _menu.RegisterMenuItem("API", OnApiSettingsClicked);
            _menu.RegisterMenuItem("PACKAGES", OnPackagesClicked);
        }

        private static void OnPackagesClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.Packages.ToString(), "" + (int)SubMenuStates.Packages);
        }

        private static void OnApiSettingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.ApiSettings.ToString(), "" + (int)SubMenuStates.ApiSettings);
        }

        private static void OnStandardSetttingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.StandardSettings.ToString(), "" + (int)SubMenuStates.StandardSettings);
        }

        private static void OnTitleSettingsClicked()
        {
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, SubMenuStates.TitleSettings.ToString(), "" + (int)SubMenuStates.TitleSettings);
        }

        private static void OnStudioChange(Studio newStudio)
        {
            var newTitleId = newStudio.Titles == null ? "" : newStudio.Titles[0].Id;
            OnTitleIdChange(newTitleId);
        }

        private static void OnTitleChange(Title newTitle)
        {
            OnTitleIdChange(newTitle.Id);
        }

        private static void OnTitleIdChange(string newTitleId)
        {
            var studio = GetStudioForTitleId(newTitleId);
            PlayFabEditorDataService.EnvDetails.selectedStudio = studio.Name;
            PlayFabEditorDataService.SharedSettings.TitleId = newTitleId;
#if ENABLE_PLAYFABADMIN_API || ENABLE_PLAYFABSERVER_API
            PlayFabEditorDataService.SharedSettings.DeveloperSecretKey = studio.GetTitleSecretKey(newTitleId);
#endif
            PlayFabEditorDataService.EnvDetails.titleData.Clear();
            if (PlayFabEditorDataMenu.tdViewer != null)
                PlayFabEditorDataMenu.tdViewer.items.Clear();
            PlayFabEditorDataService.SaveEnvDetails();
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSuccess);
        }

        private static string AddToBuildTarget(List<string> targets, string define)
        {
            targets.Add(define);
            return string.Join(";", targets.ToArray());
        }

        private static string RemoveToBuildTarget(List<string> targets, string define)
        {
            targets.Remove(define);
            return string.Join(";", targets.ToArray());
        }

        private static string GetSelectedTitleIdFromOptions(string titleEntry)
        {
            return titleEntry.Substring(1, titleEntry.IndexOf(']') - 1);
        }
        #endregion
    }
}
