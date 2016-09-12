namespace PlayFab.Editor
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public class TitleDataViewer : Editor {
        public List<KvpItem> items;
        public static TitleDataEditor tdEditor;
        public string displayTitle = "";
        public Vector2 scrollPos = Vector2.zero;
        private bool showSave = false;

        // this gets called after the Base draw loop
        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.displayTitle);
                if(GUILayout.Button("REFRESH",  PlayFabEditorHelper.uiStyle.GetStyle("Button")))
                {
                     RefreshRecords();

                }

            if(GUILayout.Button("+",  PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(25)))
                {
                    AddRecord();
                }

            EditorGUILayout.EndHorizontal();


            if(items.Count > 0)
            {
                    scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                    float keyInputBoxWidth = EditorGUIUtility.currentViewWidth > 200 ? 170 : (EditorGUIUtility.currentViewWidth - 100) / 2;
                    float valueInputBoxWidth = EditorGUIUtility.currentViewWidth > 200 ? EditorGUIUtility.currentViewWidth - 290 : (EditorGUIUtility.currentViewWidth - 100) / 2; 

                      for(var z = 0; z < this.items.Count; z++)
                    {
                        this.items[z].DataEditedCheck();
                        if(items[z].isDirty)
                        {
                            showSave = true;
                        }

                        if(items[z].Value != null)
                        {

                            //GUIContent c1 = new GUIContent(items[z].Value);
                            //Rect r1 = GUILayoutUtility.GetRect(c1, EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("TextArea"));

                        var keyStyle = this.items[z].isDirty ?  PlayFabEditorHelper.uiStyle.GetStyle("listKey_dirty") :PlayFabEditorHelper.uiStyle.GetStyle("listKey");
                        var valStyle = this.items[z].isDirty ?  PlayFabEditorHelper.uiStyle.GetStyle("listValue_dirty") : PlayFabEditorHelper.uiStyle.GetStyle("listValue");
                            //var h = style.CalcHeight(c1, valueInputBoxWidth);


                        EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));




                        items[z].Key = GUILayout.TextField(items[z].Key, keyStyle, GUILayout.Width(keyInputBoxWidth));

                            EditorGUILayout.LabelField(":", GUILayout.MaxWidth(10));
                            GUILayout.Label(""+items[z].Value, valStyle, GUILayout.MaxWidth(valueInputBoxWidth), GUILayout.MaxHeight(25));  

                        if(GUILayout.Button("E",  PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxHeight(19), GUILayout.MaxWidth(20)))
                            {
                                tdEditor.LoadData(items[z].Key, items[z].Value);
                                TitleDataEditor.ShowWindow(tdEditor);
                            } 
                        if(GUILayout.Button("X",  PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxHeight(19), GUILayout.MaxWidth(20)))
                            {
                                items[z].isDirty = true;
                                items[z].Value = null;
                            } 
                          
                            EditorGUILayout.EndHorizontal();
                        }
                    }


                GUILayout.EndScrollView();//EditorGUILayout.EndVertical();

                if(showSave)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("SAVE", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(200)))
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
            this.items.Add(new KvpItem("","NewValue"){isDirty = true});
        }

        public void RefreshRecords()
        {
            //BaseUiAnimationController.StartAlphaFade(1, 0, listDisplay);

            Action<PlayFab.Editor.EditorModels.GetTitleDataResult> cb = (result) => {
                
                items.Clear();
                showSave = false;
                foreach(var kvp in result.Data)
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
            foreach(var item in items)
            {
                if(item.isDirty)
                {
                    dirtyItems.Add(item.Key, item.Value);
                }

            }

            if(dirtyItems.Count > 0)
            {
                PlayFabEditorApi.SetTitleData(dirtyItems, (result) => 
                {
                    foreach(var item in items)
                    {
                        item.CleanItem();
                    }
                }, PlayFabEditorHelper.SharedErrorCallback);
            } 
        }



        public TitleDataViewer(List<KvpItem> i = null)
        {
            this.items = i ?? new List<KvpItem>();
        }

        public TitleDataViewer()
        {
            this.items = new List<KvpItem>();
        }

        public void OnEnable()
        {
            if(tdEditor == null)
            {
                tdEditor = ScriptableObject.CreateInstance<TitleDataEditor>();
            }
        }

    }


    public class KvpItem
    {
        public string Key;
        public string Value;

        public string _prvKey;
        public string _prvValue;

        public bool isDirty;

        public KvpItem(string k, string v)
        {
            this.Key = k;
            this.Value = v;

            this._prvKey = k;
            this._prvValue = v;
        }

        public void CleanItem()
        {
            _prvKey = Key;
            _prvValue = Value;
            isDirty = false;
        }

        public void DataEditedCheck()
        {
            if(Key != _prvKey || Value != _prvValue)
            {
                this.isDirty = true;
            }
            else
            {
               
            }
        }


    }
}

