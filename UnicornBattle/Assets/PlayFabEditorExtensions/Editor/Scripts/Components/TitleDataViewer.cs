using PlayFab.PfEditor.EditorModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayFab.PfEditor
{
    public class TitleDataViewer : UnityEditor.Editor
    {
        public List<KvpItem> items;
        public static TitleDataEditor tdEditor;
        public Vector2 scrollPos = Vector2.zero;
        private bool showSave = false;

        // this gets called after the Base draw loop
        public void Draw()
        {
            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            GUILayout.Label("TitleData provides Key-Value storage available to all API sets. TitleData is designed to store game-wide configuration data.", PlayFabEditorHelper.uiStyle.GetStyle("genTxt"));
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("REFRESH", PlayFabEditorHelper.uiStyle.GetStyle("Button")))
            {
                RefreshRecords();

            }

            if (GUILayout.Button("+", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(25)))
            {
                AddRecord();
            }

            EditorGUILayout.EndHorizontal();

            if (items.Count > 0)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                float keyInputBoxWidth = EditorGUIUtility.currentViewWidth > 200 ? 170 : (EditorGUIUtility.currentViewWidth - 100) / 2;
                float valueInputBoxWidth = EditorGUIUtility.currentViewWidth > 200 ? EditorGUIUtility.currentViewWidth - 290 : (EditorGUIUtility.currentViewWidth - 100) / 2;

                for (var z = 0; z < this.items.Count; z++)
                {
                    this.items[z].DataEditedCheck();
                    if (items[z].isDirty)
                    {
                        showSave = true;
                    }

                    if (items[z].Value != null)
                    {

                        //GUIContent c1 = new GUIContent(items[z].Value);
                        //Rect r1 = GUILayoutUtility.GetRect(c1, EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("TextArea"));

                        var keyStyle = this.items[z].isDirty ? PlayFabEditorHelper.uiStyle.GetStyle("listKey_dirty") : PlayFabEditorHelper.uiStyle.GetStyle("listKey");
                        var valStyle = this.items[z].isDirty ? PlayFabEditorHelper.uiStyle.GetStyle("listValue_dirty") : PlayFabEditorHelper.uiStyle.GetStyle("listValue");
                        //var h = style.CalcHeight(c1, valueInputBoxWidth);


                        EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));




                        items[z].Key = GUILayout.TextField(items[z].Key, keyStyle, GUILayout.Width(keyInputBoxWidth));

                        EditorGUILayout.LabelField(":", GUILayout.MaxWidth(10));
                        GUILayout.Label("" + items[z].Value, valStyle, GUILayout.MaxWidth(valueInputBoxWidth), GUILayout.MaxHeight(25));

                        if (GUILayout.Button("EDIT", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxHeight(19), GUILayout.MinWidth(35)))
                        {
                            if (tdEditor == null)
                            {
                                tdEditor = EditorWindow.GetWindow<TitleDataEditor>();
                                tdEditor.titleContent = new GUIContent("Title Data");
                                tdEditor.minSize = new Vector2(300, 400);
                            }

                            tdEditor.LoadData(items[z].Key, items[z].Value);
                            tdEditor.Show();
                        }
                        if (GUILayout.Button("X", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxHeight(19), GUILayout.MinWidth(20)))
                        {
                            items[z].isDirty = true;
                            items[z].Value = null;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }


                GUILayout.EndScrollView();//EditorGUILayout.EndVertical();

                if (showSave)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("SAVE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(200)))
                    {
                        //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);
                        SaveRecords();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }


            // draw code here.
            // base.PostDraw();
        }


        public void AddRecord()
        {
            this.items.Add(new KvpItem("", "NewValue") { isDirty = true });
        }

        public void RefreshRecords()
        {
            //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);

            Action<PlayFab.PfEditor.EditorModels.GetTitleDataResult> cb = (result) =>
            {

                items.Clear();
                showSave = false;
                foreach (var kvp in result.Data)
                {
                    items.Add(new KvpItem(kvp.Key, kvp.Value));
                }

                PlayFabEditorDataService.envDetails.titleData = result.Data;
                PlayFabEditorDataService.SaveEnvDetails();

            };

            PlayFabEditorApi.GetTitleData(cb, PlayFabEditorHelper.SharedErrorCallback);
        }

        public void SaveRecords()
        {
            //reset dirty status.
            showSave = false;
            Dictionary<string, string> dirtyItems = new Dictionary<string, string>();
            foreach (var item in items)
            {
                if (item.isDirty)
                {
                    dirtyItems.Add(item.Key, item.Value);
                }

            }

            if (dirtyItems.Count > 0)
            {
                float nextSeconds = 1f;
                foreach (var di in dirtyItems)
                {
                    EditorCoroutine.start(SaveItem(di, nextSeconds));
                    nextSeconds += 1f;
                }

















                /*
                PlayFabEditorApi.SetTitleData(dirtyItems, (result) => 
                {
                    foreach(var item in items)
                    {
                        item.CleanItem();
                    }
                }, PlayFabEditorHelper.SharedErrorCallback);
                */

                foreach (var item in items)
                {
                    item.CleanItem();
                }

            }
        }



        private IEnumerator SaveItem(KeyValuePair<string, string> dirtyItem, float seconds)
        {
            yield return new EditorCoroutine.EditorWaitForSeconds(seconds);
            //Debug.LogFormat("{0} - Co-Start: {1}", dirtyItem.Key, seconds);
            var itemToUpdateDic = new Dictionary<string, string> { { dirtyItem.Key, dirtyItem.Value } };
            PlayFabEditorApi.SetTitleData(itemToUpdateDic, (result) =>
            {
                //Do Nothing with the result.
            }, PlayFabEditorHelper.SharedErrorCallback);
        }

        public TitleDataViewer(List<KvpItem> i = null)
        {
            this.items = i ?? new List<KvpItem>();
        }

        public TitleDataViewer()
        {
            this.items = new List<KvpItem>();
        }

        public void OnDestroy()
        {
            tdEditor = null;
        }
    }



}

