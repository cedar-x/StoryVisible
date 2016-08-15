using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using xxstory;

[CustomEditor(typeof(StoryUIMapping))]
public class StoryUIMappingInspector : Editor
{
    private StoryUIMapping objUIMap;
    private string refName;
    private string comName;
    private GameObject objAdd;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        objUIMap = target as StoryUIMapping;

        foreach (var panelInfo in objUIMap._panelInfos.Values)
        {
            GUILayout.BeginHorizontal();
            panelInfo.bFolder = EditorGUILayout.Foldout(panelInfo.bFolder, GUIContent.none);
            GUILayout.Label(panelInfo.panelRoot.name + ":" + panelInfo.panelName, GUILayout.Width(200));
            if (GUILayout.Button("Del"))
            {
                objUIMap.Delete(panelInfo.panelRoot);
                return;
            }
            GUILayout.EndHorizontal();
            if (panelInfo.bFolder)
            {
                GUILayout.BeginHorizontal();
                panelInfo.panelName = GUILayout.TextField(panelInfo.panelName);
                if (GUILayout.Button("AllText"))
                {
                    panelInfo.AddAllText();
                }
                if(GUILayout.Button("check", GUILayout.Width(50)))
                {
                    panelInfo.CheckSignName();
                }
                GUILayout.EndHorizontal();
                NGUIEditorTools.SetLabelWidth(40f);
                GUILayout.Space(6f);
                if (NGUIEditorTools.DrawHeader("Component and Function"))
                {
                    NGUIEditorTools.BeginContents();
                    Dictionary<string, storyMapData> symbols = panelInfo.mapDatas;
                    foreach (var sym in symbols.Values)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20f);
                        sym.bFloder = EditorGUILayout.Foldout(sym.bFloder, GUIContent.none);
                        sym.signName = EditorGUILayout.TextField(sym.signName);
                        GUILayout.Label(sym.refName, GUILayout.MinWidth(100f));
                        if (GUILayout.Button("X", GUILayout.Width(22f)))
                        {
                            panelInfo.DeleteSingle(sym.refName);
                            return;
                        }
                        GUILayout.EndHorizontal();
                        if (sym.bFloder)
                        {
                            for (int i = 0; i < sym.funcs.Count; ++i)
                            {
                                StoryUIMapping.funcData funcdData = sym.funcs[i];
                                string key = funcdData.key;
                                string value = funcdData.value;
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(24);
                                GUILayout.Label(key);
                                funcdData.value = EditorGUILayout.TextField(funcdData.value);
                                if (GUILayout.Button("X", GUILayout.Width(22f)))
                                {
                                    panelInfo.DeleteFunction(sym.refName, key);
                                    --i;
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(24f);
                            sym.funcName = EditorGUILayout.TextField(sym.funcName, GUILayout.Width(100));
                            sym.funcImpl = EditorGUILayout.TextField(sym.funcImpl);
                            if (GUILayout.Button("Add"))
                            {
                                panelInfo.AddFunction(sym.refName, sym.funcName, sym.funcImpl);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    refName = EditorGUILayout.TextField(refName);
                    comName = EditorGUILayout.TextField(comName);
                    if (GUILayout.Button("<"))
                    {
                        if (Selection.activeGameObject == null)
                        {
                            Debug.Log("please select one active GameObject....");
                            return;
                        }
                        comName = panelInfo.GetSubName(Selection.activeGameObject);
                    }
                    if (GUILayout.Button("Add"))
                    {
                        panelInfo.AddSingle(refName, comName);
                    }
                    GUILayout.EndHorizontal();
                    NGUIEditorTools.EndContents();
                }
            }

        }

        GUILayout.BeginHorizontal();
        objAdd = EditorGUILayout.ObjectField("PreAdd", objAdd, typeof(GameObject)) as GameObject;
        if (GUILayout.Button("Add"))
        {
            if (objAdd == null)
            {
                Debug.Log("PreAdd GameObject is null..");
                return;
            }
            objUIMap.Add(objAdd);
        }
        GUILayout.EndHorizontal();
    }
}