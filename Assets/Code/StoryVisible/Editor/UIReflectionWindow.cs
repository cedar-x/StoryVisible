using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using xxstory;

public class UIReflectionWindow : EditorWindow
{

    #region MENU
    [MenuItem("Story Export/UI Transition/Flush Hierarchy Label")]
    private static void OnFlushHierarchyLabel()
    {
        if (!checkSelectionNull()) return;
        int slength = Selection.objects.Length;
        for (int i = 0; i < slength; i++)
        {
//             foreach (var childLbl in (Selection.objects[i] as GameObject).GetComponentsInChildren<UILabel>())
//             {
//                 SetLabelParam(childLbl);
//             }
        }
        Debug.Log("Flush Hierarchy Label successed:count:" + slength);
    }
    [MenuItem("Story Export/UI Transition/Flush Project Label")]
    private static void OnFlushProjectLabel()
    {
//         foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
//         {
//             if (PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
//             {
//                 UILabel[] childlbls = GetComponentsInChildrenOfAsset<UILabel>(o as GameObject);
//                 foreach (var childlbl in childlbls)
//                 {
//                     SetLabelParam(childlbl);
//                 }
//                 Debug.Log("Flush Project Labe:" + AssetDatabase.GetAssetPath(o));
//             }
//            
    }
    [MenuItem("Story Export/UI Transition/Flush Project Button")]
    private static void OnFlushProjectButton()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
            {
//                 UIButtonScale[] childs = GetComponentsInChildrenOfAsset<UIButtonScale>(o as GameObject);
//                 foreach (var child in childs)
//                 {
//                     SetButtonParam(child);
//                 }
                Debug.Log("Flush Project Button:" + AssetDatabase.GetAssetPath(o));
            }
        }
    }
    [MenuItem("Story Export/UI Transition/Attach Hierarchy Label")]
    private static void OnAttachHierarchyLabel()
    {
        if (!checkSelectionNull()) return;
        int slength = Selection.gameObjects.Length;
        for (int i = 0; i < slength; i++)
        {
//             foreach (var childLbl in Selection.gameObjects[i].GetComponentsInChildren<UILabel>())
//             {
//                 AttachLabel(childLbl.gameObject);
//             }
        }
        Debug.Log("Attach Hierarchy Label successed:count:" + slength);
    }
    [MenuItem("Story Export/UI Transition/Attach Project Label")]
    private static void OnAttachProjectchyLabel()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (PrefabUtility.GetPrefabType(o) == PrefabType.Prefab)
            {
//                 UILabel[] childlbls = GetComponentsInChildrenOfAsset<UILabel>(o as GameObject);
//                 foreach (var childlbl in childlbls)
//                 {
//                     AttachLabel(childlbl.gameObject);
//                 }
                Debug.Log("Attach Hierarchy Label:" + AssetDatabase.GetAssetPath(o));
            }
        }
    }
    [MenuItem("Story Export/UI Transition/Export Hierarchy Label")]
    private static void OnExportHierarchyLocalLabel()
    {
        //用于ui文字本地化-Hierarchy试图
        if (!checkSelectionNull()) return;
        GameObject oRoot = Selection.activeGameObject;
        List<string> checkstr = new List<string>();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int index = 0;
        string sfName = oRoot.name;
        sb.Append("UIReflectMapping.localStringConfig['" + sfName + "']={\n");
//         foreach (var childLbl in oRoot.GetComponentsInChildren<UILabel>())
//         {
//             AttachLabel(childLbl.gameObject);
//             string subName = StoryUIMapping.GetSubName<Hstj.HUIWidget>(childLbl.gameObject, oRoot);
//             if (!checkstr.Contains(subName))
//             {
//                 checkstr.Add(subName);
//             }else{
//                 Debug.LogWarning("Two GameObject can't be the same name:" + subName);
//             }
//             string non = childLbl.text.Replace("\n", "\\n");
//             sb.Append(string.Format("\t{0}={{\n\t\tname = '{1}',\n\t\ttext = 'text_{2}',\n\t}},\n", "label" + index,subName, non.Replace("\r", "")));
//             ++index;
//         }
        sb.Append("}");
        ExportFile(sfName, sb.ToString());
        Debug.Log("Export Hierarchy Local String successed:" + sfName);
    }
    [MenuItem("Story Export/UI Transition/Export Project Label")]
    private static void OnExportProjectLocalLabel()
    {
        //用于ui文字本地化-Project试图
        System.Text.StringBuilder sbInclude = new System.Text.StringBuilder("_G.UIReflectMapping.localStringConfig = {}\n");
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
//             if (PrefabUtility.GetPrefabType(o) == PrefabType.Prefab && (o as GameObject).GetComponent<Hstj.HUIRoot>())
//             {
//                 UILabel[] childlbls = GetComponentsInChildrenOfAsset<UILabel>(o as GameObject);
//                 System.Text.StringBuilder sb = new System.Text.StringBuilder();
//                 int index = 0;
//                 List<string> checkstr = new List<string>();
//                 string assetPath = AssetDatabase.GetAssetPath(o);
//                 string folderPath = assetPath.Substring(0, assetPath.LastIndexOf('/'));
//                 folderPath = folderPath.Substring(folderPath.IndexOf("UI/") + 3);
//                 string sfName = o.name;
//                 if (folderPath != o.name)
//                     sfName = folderPath + "/" + o.name;
//                 sbInclude .Append("dofile \"Config/UIReflectConfig/UILocalString/"+o.name+".lua\"\n");
//                 sb.Append("UIReflectMapping.localStringConfig['" + sfName + "']={\n");
//                 foreach (var childlbl in childlbls)
//                 {
//                     string subName = StoryUIMapping.GetSubName<Hstj.HUIWidget>(childlbl.gameObject, (o as GameObject));
//                     if (!AttachLabel(childlbl.gameObject))
//                     {
//                         Debug.LogWarning("Export Project Label AttachLabel failed:" +o.name+":"+ subName);
//                     }
//                     if (string.IsNullOrEmpty(childlbl.text) || childlbl.gameObject.activeSelf == false)
//                         continue;
//                     if (!checkstr.Contains(subName))
//                     {
//                         checkstr.Add(subName);
//                     }
//                     else
//                     {
//                         Debug.LogWarning("Two GameObject can't be the same name:" +o.name+":"+ subName);
//                     }
//                     string non = childlbl.text.Replace("\n", "\\n");
//                     sb.Append(string.Format("\t{0}={{\n\t\tname = '{1}',\n\t\ttext = 'text_{2}',\n\t}},\n", "label" + index, subName, non.Replace("\r", "")));
//                     ++index;
//                 }
//                 sb.Append("}");
//                 ExportFile(o.name, sb.ToString());
//                //Debug.Log("Export Project Local String:" + AssetDatabase.GetAssetPath(o));
//           }
        }
        string exportFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log("Export Project Local String:" + exportFolder);
        if(exportFolder.EndsWith("UI"))
        {
            ExportFile("Include", sbInclude.ToString());
        }

    }
    #endregion

    #region SUPPORTFUNC
    private static string[] __GetFiles(string path, bool recursive = true)
    {
        var resultList = new List<string>();
        var dirArr = Directory.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        for (int i = 0; i < dirArr.Length; i++)
        {
            if (Path.GetExtension(dirArr[i]) != ".meta")
                resultList.Add(dirArr[i].Replace('\\', '/'));
        }
        return resultList.ToArray();
    }
    public static T[] GetComponentsInChildrenOfAsset<T>(GameObject go) where T : Component
    {
        List<Transform> tfs = new List<Transform>();
        CollectChildren(tfs, go.transform);
        List<T> all = new List<T>();
        for (int i = 0; i < tfs.Count; i++)
            all.AddRange(tfs[i].gameObject.GetComponents<T>());
        return all.ToArray();
    }

    private static void CollectChildren(List<Transform> transforms, Transform tf)
    {
        transforms.Add(tf);
        foreach (Transform child in tf)
        {
            CollectChildren(transforms, child);
        }
    }
    #endregion

    private static bool checkSelectionNull()
    {
        Object[] selectObjs = Selection.objects;
        int slength = selectObjs.Length;
        if (slength <= 0)
        {
            Debug.LogWarning("Select Object Failed Select GameObject Not Right.");
            return false;
        }
        return true;
    }
//     private static void SetLabelParam(UILabel objcom)
//     {
//         if (objcom == null) return;
//         objcom.effectStyle = UILabel.Effect.Shadow;
//         objcom.effectColor = new Color(0, 0, 0, 1);
//         objcom.effectDistance = new Vector2(1f, 1f);
//         objcom.spacingX = 1;
//         objcom.spacingY = 0;
//     }
//     private static void SetButtonParam(UIButtonScale objcom)
//     {
//         if (objcom == null) return;
//         objcom.hover = Vector3.one;
//         objcom.pressed = new Vector3(0.95f,0.95f,0.95f);
//         objcom.duration = 0.02f;
//     }
    private static bool AttachLabel(GameObject objLbl)
    {
//         var hstjtype = objLbl.GetComponent<Hstj.HUIWidget>();
//         if (hstjtype == null)
//         {
//             objLbl.gameObject.AddComponent<Hstj.HUILabel>();
//         }
//         else if (!(hstjtype is Hstj.HUILabel))
//         {
//             Debug.LogWarning("UILabel's LuaObject is not HUILabel. please check.." + objLbl.name);
//             return false;
//         }
         return true;
    }
    private static void ExportFile(string fileName, string content)
    {
        string savePath = Application.streamingAssetsPath + "/ScriptConfig/UIReflectConfig/UILocalString/" + fileName + ".lua";
        StreamWriter sw = new StreamWriter(savePath);
        sw.Write(content);
        sw.Close();
    }








    //         List<string> folders = new List<string>();
    //         string szSelected;
    //         for (int i = 0; i < selectObjs.Length; i++)
    //         {
    //             szSelected = AssetDatabase.GetAssetPath(selectObjs[i]);
    //             Debug.Log("select fold path:" + szSelected);
    //             folders.Add(szSelected);
    //         }
    //         var guids = AssetDatabase.FindAssets("t:Prefab", folders.ToArray());
    //         foreach (var guid in guids)
    //         {
    //             string path = AssetDatabase.GUIDToAssetPath(guid);
    //             Debug.Log("select prefab path :" + Selection.activeObject.name);
    //         }
    //         GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
    //         foreach (GameObject go in gos)
    //         {
    //             if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
    //             {
    //                 UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(go);
    //                 string path = AssetDatabase.GetAssetPath(parentObject);
    //                 Debug.Log("===sdfs="+path);
    //             }
    //         }
    //         var dirArr = Directory.GetDirectories(AssetDatabase.GetAssetPath(Selection.activeObject));
    //         for (int i = 0; i < dirArr.Length; i++)
    //         {
    //             var pathArr = __GetFiles(dirArr[i], false);
    //             for (int j = 0; j < pathArr.Length; j++)
    //             {
    //                 var filePath = pathArr[j];
    //                 Debug.Log("===sdfs2222=" + filePath);
    //             }
    //         }
}
