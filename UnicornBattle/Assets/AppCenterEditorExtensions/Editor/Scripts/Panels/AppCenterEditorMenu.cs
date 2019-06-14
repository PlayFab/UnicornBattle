using UnityEngine;
using UnityEditor;

namespace AppCenterEditor
{
    public class AppCenterEditorMenu : Editor
    {
        #region panel variables
        internal enum MenuStates
        {
            Sdks = 0,
        }

        internal static MenuStates _menuState = MenuStates.Sdks;
        #endregion

        public static void DrawMenu()
        {
            if (AppCenterEditorSDKTools.IsInstalled)
                _menuState = (MenuStates)AppCenterEditorPrefsSO.Instance.curMainMenuIdx;

            var sdksButtonStyle = AppCenterEditorHelper.uiStyle.GetStyle("textButton");


            if (_menuState == MenuStates.Sdks)
                sdksButtonStyle = AppCenterEditorHelper.uiStyle.GetStyle("textButton_selected");

            using (new AppCenterGuiFieldHelper.UnityHorizontal(AppCenterEditorHelper.uiStyle.GetStyle("gpStyleGray1"), GUILayout.Height(25), GUILayout.ExpandWidth(true)))
            {
                GUILayout.Space(5);

                if (GUILayout.Button("SDK", sdksButtonStyle, GUILayout.MaxWidth(35)))
                {

                }
            }
        }
    }
}
