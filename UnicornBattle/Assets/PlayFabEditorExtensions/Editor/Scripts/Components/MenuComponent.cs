namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using System.Linq;

    //[InitializeOnLoad]
    public class MenuComponent : Editor {

        Dictionary<string, MenuItemContainer> items = new Dictionary<string, MenuItemContainer>();
        GUIStyle selectedStyle;
        GUIStyle defaultStyle;
        GUIStyle bgStyle;

        public void DrawMenu()
        {
            selectedStyle = selectedStyle == null ? PlayFabEditorHelper.uiStyle.GetStyle("textButton_selected") : selectedStyle;
            defaultStyle = defaultStyle == null ? PlayFabEditorHelper.uiStyle.GetStyle("textButton") : defaultStyle;
            bgStyle = bgStyle == null ? PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"): bgStyle;
            
            GUILayout.BeginHorizontal(bgStyle, GUILayout.ExpandWidth(true));

                foreach(var item in items)
                {
                    if (GUILayout.Button(item.Value.displayName, item.Value.isSelected ? selectedStyle : defaultStyle))
                    {
                        OnMenuItemClicked(item.Key);
                    }
                }
            GUILayout.EndHorizontal();
        }

        public void RegisterMenuItem(string n, System.Action m)
        {
            if(!items.ContainsKey(n))
            {
                items.Add(n, new MenuItemContainer(){ displayName = n, method = m, isSelected = items.Count == 0 ? true : false });
            }
            else
            {
                // update the method ?
                //items[n].method = m;
            }
        }

        private void OnMenuItemClicked(string key)
        {
           // PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnSubmenuItemClicked, key);
            if(items.ContainsKey(key))
            {
                DeselectAll();
                items[key].isSelected = true;
                if(items[key].method != null)
                {
                    items[key].method.Invoke();
                }
            }
        }

        private void DeselectAll()
        {
            foreach(var item in items)
            {
                item.Value.isSelected = false;
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
