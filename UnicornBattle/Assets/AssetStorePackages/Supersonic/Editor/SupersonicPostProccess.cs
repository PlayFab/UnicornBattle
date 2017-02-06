using SupersonicJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class SupersonicPostProccesser
{
    [PostProcessBuild(1000)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target.ToString().Equals("iPhone") || target.ToString().Equals("iOS"))
        {
            var project = new Supersonic.UnityEditor.XCodeEditor.XCProject(path);

            // Find Mediation.suGlobalConfig file and run it



            var adapterFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.adapterProjmod", System.IO.SearchOption.AllDirectories);
            if (adapterFiles[0] != null)
            {
                FileInfo projectFileInfo;
                Hashtable globalDatastore = new Hashtable();
                foreach (string file in adapterFiles)
                {
                    projectFileInfo = new FileInfo(file);
                    string contents = projectFileInfo.OpenText().ReadToEnd();
                    Dictionary<string, object> dictJson = Json.Deserialize(contents) as Dictionary<string, object>;
                    Hashtable datastore = new Hashtable(dictJson);
                    globalDatastore = mergeData(globalDatastore, datastore);
                    // merge json with dedup
                    // create file
                    //get all the data and concatenate
                }
                Dictionary<string, object> dic = HashtableToDictionary(globalDatastore);
                string output = Json.Serialize(dic);
                // create file globalProjmod from json
                System.IO.File.WriteAllText(new FileInfo(adapterFiles[0]).Directory.FullName + "/Mediation.globalProjmod", output);

            }


            System.Threading.Thread.Sleep(1000);
            var files = System.IO.Directory.GetFiles(Application.dataPath, "*.globalProjmod", System.IO.SearchOption.AllDirectories);
            if (files[0] != null)
            {
                project.ApplyMod(Application.dataPath, files[0]);
            }

            project.Save();
            Debug.Log("Supersonic PostProccesser finished");
        }
    }

    private static Hashtable mergeData(Hashtable mainData, Hashtable addition)
    {
        Hashtable mergeProduct = (Hashtable)mainData.Clone();
        foreach (DictionaryEntry entry in addition)
        {
            var value = mergeProduct[entry.Key];
            // if there's already such an entry in the global config
            //System.Type type = value.GetType();
            if (value != null && value is IList
                && entry.Value is IList)
            {
                List<object> dedupOutput = new List<object>((List<object>)value);
                if (dedupOutput.Count == 0)
                {
                    dedupOutput.Add("");
                }
                foreach (var obj in (IList)(entry.Value))
                {
                    if (!dedupOutput.Contains(obj))
                    {
                        dedupOutput.Add(obj);
                    }
                }



                mergeProduct[entry.Key] = dedupOutput;
            }
            else if (value == null)
            {
                mergeProduct.Add(entry.Key, entry.Value);
            }
        }
        return mergeProduct;
    }


    public static Dictionary<string, object> HashtableToDictionary(Hashtable table)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        foreach (DictionaryEntry kvp in table)
            dict.Add((string)kvp.Key, (object)kvp.Value);
        return dict;
    }

}
