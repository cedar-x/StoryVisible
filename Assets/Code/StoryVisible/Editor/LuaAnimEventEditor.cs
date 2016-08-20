using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using xxstory;

/// <summary>
/// Inspector 显示LuaGameCamera设置参数，方便可视化配置参数
/// </summary>
[CustomEditor(typeof(LuaAnimEvent))]
public class LuaAnimEventEditor : Editor
{
    LuaAnimEvent objAnimEvent;
    private static int dwBoardIndex;
    private static bool bActorInfoFolder = true;
    private bool bActorZJCreate = false;
    private bool bActorFolder;
    private bool bBasicFolder;
    private static int dwModelID;
    private int minBoxWidth = 20, minBoxHeight = 12, minRightSpace=60;
    void OnInspectorUpdate()
    {
        this.Repaint(); // 刷新Inspector
    }
    public override void OnInspectorGUI()
    {
        objAnimEvent = target as LuaAnimEvent;
        if (!objAnimEvent.bInitMember) return;
        //
        ImportScript(objAnimEvent);
        ShowBasicInfo();
        LuaAnimEventEditor.ShowActorInfo(objAnimEvent);
        ShowBoardInfo();
    }
    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=12>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            //if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            if (GUILayout.Button(text, GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }
    //脚本导入
    private void ImportScript(LuaAnimEvent animEvent)
    {
        if (GUILayout.Button("ImportLua"))
        {
            LuaAnimEvent.OnImportFromLua(animEvent);
        }
    }
    //剧情基础信息导入
    private void ShowBasicInfo()
    {
        bBasicFolder = EditorGUILayout.Foldout(bBasicFolder, "Story Basic Info");
        if (bBasicFolder)
        {
            storyBasicInfo basic = objAnimEvent._storyBasicInfo;
            basic.szStageName = EditorGUILayout.TextField("stage name", basic.szStageName);
            //basic.dwNextTime = EditorGUILayout.FloatField("default next time", basic.dwNextTime);
            basic.szPathName = EditorGUILayout.TextField("Path Prefab Name", basic.szPathName);
            GUILayout.BeginHorizontal();
            basic.bNewSceneState = GUILayout.Toggle(basic.bNewSceneState, "new Scene");
            basic.bNewLightState = GUILayout.Toggle(basic.bNewLightState, "new light");
            GUILayout.EndHorizontal();
            EditorGUILayout.IntField("type", basic.dwType);
            EditorGUILayout.TextField("sceneName", basic.szSceneName);
        }
    }
    //人物信息展示
    public static void ShowActorInfo(LuaAnimEvent animEvent)
    {
        bActorInfoFolder = EditorGUILayout.Foldout(bActorInfoFolder, "All Actor Info");
        if (bActorInfoFolder == true)
        {
            for (int i = 0, imax = animEvent.actorCount; i < imax; ++i)
            {
                GUILayout.BeginHorizontal();
                storyActorInfo actor = animEvent._storyActor[i];
                if (animEvent.Count == 0 && i == animEvent.actorCount - 1 && GUILayout.Button("X", GUILayout.Width(20)))
                {
                    animEvent.DeleteActor(actor);
                }
                if (GUILayout.Button(">"))
                {
                    StoryShotCtrl lenCtrl = new StoryShotCtrl();
                    lenCtrl.actor = actor;
                    StoryBoardCtrl boardCtrl = animEvent._storyBoard[animEvent.Count - 1];
                    boardCtrl.Add(lenCtrl);
                }
                GUILayout.Label("Actor"+(i + 1) + ": ");
                //GUILayout.Label(actor.dwModelId + ":");
                actor.dwModelId = EditorGUILayout.IntField("dwModelID", actor.dwModelId);
                actor.name = EditorGUILayout.TextField("name", actor.name);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                if (animEvent.bSingleModelID(dwModelID) == true || (EditorUtility.DisplayDialog("已经存在此类型模型", "是否重复创建", "确认", "取消")))
                {
                    int nameIndex = animEvent.actorCount + 1;
                    animEvent.AddActor(dwModelID, nameIndex);
                }
            }
            dwModelID = EditorGUILayout.IntField("dwModelID", dwModelID);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            dwBoardIndex = EditorGUILayout.IntField("Add Board", dwBoardIndex);
            if (GUILayout.Button("Add"))
            {
                StoryBoardCtrl boardCtrl = new StoryBoardCtrl();
                animEvent.InitStoryBoard(boardCtrl);
                animEvent.AddBoard(boardCtrl, dwBoardIndex);
                dwBoardIndex = 0;
            }
            if (GUILayout.Button("Execute"))
            {
                animEvent.Stop();
                animEvent.Execute();
            }
            GUILayout.EndHorizontal();
        }
    }
    //时间周期展示
    private void ShowBoardInfo()
    {
        for (int i = 0; i < objAnimEvent.Count; ++i)
        {
            StoryBoardCtrl board = objAnimEvent[i];
            string name = "storyBoard:" + (i + 1)+"(time:"+board.time+"s)";
            GUILayout.BeginHorizontal();
            board.bFolderOut = EditorGUILayout.Foldout(board.bFolderOut, name);
            if (GUILayout.Button("<b><size=12>\u25BA</size></b> ", new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                board.Execute();
            }
            if (GUILayout.Button("X", new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                objAnimEvent.DeleteBoard(board);
            }
            GUILayout.Space(minRightSpace);
            GUILayout.EndHorizontal();
            if (board.bFolderOut)
            {
                ShowShotsInfo(board, i);
            }
        }
    }
    //人物镜头展示
    private void ShowShotsInfo(StoryBoardCtrl board, int boardIndex = 0)
    {
        for (int i = 0; i < board.Count; ++i)
        {
            StoryShotCtrl lenCtrl = board[i];
            //string actorName = lenCtrl.actorName+"(time:"+lenCtrl.time+"s)";
            string actorName = "Actor" + lenCtrl.actorIndex + "(time:" + lenCtrl.time + "s)";
            string key = "storyBoard"+boardIndex + actorName;
            GUILayout.BeginHorizontal();
            string starText = "";
            if (lenCtrl == objAnimEvent.objEditorShotCtrl)
            {
                starText = "*";
            }
            if (GUILayout.Button(starText, new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                objAnimEvent.objEditorShotCtrl = lenCtrl;
            }
            bool bFolder = false;// NGUIEditorTools.DrawHeader(actorName, key, true, false);
            if (GUILayout.Button("<b><size=12>\u25BA</size></b> ", new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                lenCtrl.Stop();
                lenCtrl.Execute();
            }
            if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                objAnimEvent.objEditorCopyCtrl = lenCtrl;
            }
            if (GUILayout.Button("v", new GUILayoutOption[] { GUILayout.Width(minBoxWidth), GUILayout.Height(minBoxHeight) }))
            {
                lenCtrl.paste(objAnimEvent.objEditorCopyCtrl);
            }
            if (GUILayout.Button("X",new GUILayoutOption[]{GUILayout.Width(minBoxWidth),GUILayout.Height(minBoxHeight)}))
            {
                bool bFlag = board.Delete(lenCtrl);
                if (bFlag == true && objAnimEvent.objEditorShotCtrl == lenCtrl)
                {
                    objAnimEvent.objEditorShotCtrl = null;
                }
            }
            GUILayout.EndHorizontal();
            if (bFolder)
            {
                ShowEventInfo(lenCtrl);
            }
        }
    }
    private void ShowEventInfo(StoryShotCtrl lenCtrl)
    {
        for (int j = 0; j < lenCtrl.Count; ++j)
        {
            StoryBaseCtrl baseCtrl = lenCtrl[j];
            GUILayout.BeginHorizontal();
            GUILayout.Space(50f);
            GUILayout.Label(j + ":("+(baseCtrl.isPlaying?"*":"")+baseCtrl.time+"s)" + baseCtrl.ctrlName, GUILayout.Width(200));
            if (GUILayout.Button("Editor"))
            {
                lenCtrl._objEditorEventCtrl = baseCtrl;
            }
            if (GUILayout.Button("Delete"))
            {
                lenCtrl.Delete(baseCtrl);
            }
            if (GUILayout.Button("Execute"))
            {
                lenCtrl.Execute(j);
            }
            GUILayout.Space(minRightSpace);
            GUILayout.EndHorizontal();
        }
    }
}
