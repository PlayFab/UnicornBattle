using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    //[InitializeOnLoad]
    public class SubMenuComponent : UnityEditor.Editor
    {

        Dictionary<string, MenuItemContainer> items = new Dictionary<string, MenuItemContainer>();
        GUIStyle selectedStyle;
        GUIStyle defaultStyle;
        GUIStyle bgStyle;

        public void DrawMenu()
        {
            selectedStyle = selectedStyle ?? PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected");
            defaultStyle = defaultStyle ?? PlayFabEditorHelper.uiStyle.GetStyle("textButton");
            bgStyle = bgStyle ?? PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1");

            GUILayout.BeginHorizontal(bgStyle, GUILayout.ExpandWidth(true));

            foreach (var item in items)
            {
                var styleToUse = item.Value.isSelected ? selectedStyle : defaultStyle;
                var content = new GUIContent(item.Value.displayName);
                var size = styleToUse.CalcSize(content);

                if (GUILayout.Button(item.Value.displayName, styleToUse, GUILayout.Width(size.x + 1)))
                {
                    OnMenuItemClicked(item.Key);
                }
            }
            GUILayout.EndHorizontal();
        }

        public void RegisterMenuItem(string n, System.Action m)
        {
            if (!items.ContainsKey(n))
            {
                bool selectState = false;
                int activeSubmenu = PlayFabEditorDataService.editorSettings.currentSubMenu;
                if (items.Count == 0 && activeSubmenu == 0)
                {
                    selectState = true;
                }
                else if (activeSubmenu == items.Count)
                {
                    // this is the menu being redrawn while also not being on the first menu tab
                    selectState = true;
                }

                items.Add(n, new MenuItemContainer() { displayName = n, method = m, isSelected = selectState });
            }
            else
            {
                // update the method ?
                //items[n].method = m;
            }
        }

        private void OnMenuItemClicked(string key)
        {
            if (items.ContainsKey(key))
            {
                DeselectAll();
                items[key].isSelected = true;
                if (items[key].method != null)
                {
                    items[key].method.Invoke();
                }
            }
        }

        private void DeselectAll()
        {
            foreach (var item in items)
            {
                item.Value.isSelected = false;
            }
        }

        public SubMenuComponent()
        {
            if (!PlayFabEditor.IsEventHandlerRegistered(StateUpdateHandler))
            {
                PlayFabEditor.EdExStateUpdate += StateUpdateHandler;
            }
        }

        void StateUpdateHandler(PlayFabEditor.EdExStates state, string status, string json)
        {
            switch (state)
            {
                case PlayFabEditor.EdExStates.OnMenuItemClicked:
                    DeselectAll();
                    if (items != null)
                        foreach (var each in items)
                        {
                            each.Value.isSelected = true;
                            break;
                        }
                    break;
            }
        }
    }

    public class MenuItemContainer
    {
        public string displayName { get; set; }
        public System.Action method { get; set; }
        public bool isSelected { get; set; }
    }
}
