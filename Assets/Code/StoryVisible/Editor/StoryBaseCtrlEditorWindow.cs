using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniLua;
using xxstory;
using Hstj;

/// <summary>
/// 剧情事件添加界面管理接口
/// </summary>
/// 
public class StoryBaseCtrlEditorWindow : EditorWindow
{
    /// <summary>
    /// 打开剧情动画编辑器 用于以某个时间轴为基准的事件窗口显示
    /// </summary>
    /// <param name="bsCtrl">时间轴实例，对bsCtrl进行的事件添加</param>
    [MenuItem("Story Export/StoryWindow")]
    private static void OnStoryWindow()
    {
        StoryBaseCtrlEditorWindow window = (StoryBaseCtrlEditorWindow)EditorWindow.GetWindow(typeof(StoryBaseCtrlEditorWindow), false, "AddEvent");
        window.ShowTab();
    }
    
    //当前时间轴Ctrl和当前选中事件Ctrl
    private static StoryBaseCtrl _selectCtrl;
    private Hstj.LuaAnimEvent _animEvent;

    private int _dwScriptID;
    private bool mbCameraFolderOut;
    private bool mbExportFolderOut;
    private bool _bHaveOption;//---------------------
    private int _insertIndex = -1;//使用此参数代表当前时间添加到哪个位置
    private string szEditorState = "实例参数"; //使用此参数代表当前是待添加事件还是修改事件
    //初始化一个时间轴、目前单个时间轴
    private void initAnimEvent()
    {
        if (_animEvent != null)
        {
            Debug.LogWarning("StoryBaseCtrlEditorWindow: already have a AnimEvent....");
            return;
        }
        GameObject obj = new GameObject("New Anim Event");
        obj.AddComponent<Hstj.LuaAnimEvent>().InitMemeber();
    }
    private void drawLine()
    {
        GUILayout.Label("-------------------------------------------------------------------------------------");
    }
    ////////////////////////////////////////////////////////
    void OnInspectorUpdate()
    {
        this.Repaint(); // 刷新Inspector
    }
    void OnGUI()
    {
        OnEventWindow();
    }
    private void OnEventWindow()
    {
        OnExportSetting();
        if (!Application.isPlaying) return;
        if (!StoryAnimationEditorWindow.GetStoryAnimControl())
        {
            if (GUILayout.Button("Create Time Ctrl"))
            {
                initAnimEvent();
            }
        }
        else
        {
            _animEvent = StoryAnimationEditorWindow.animEvent;
            CameraSetting();
            if (_animEvent.objEditorShotCtrl == null)
            {
                GUILayout.Label("please choose storyShot in LuaAnimEvent.");
                return;
            }
            GUILayout.Label("Event Target:"+_animEvent.objEditorShotCtrl.actorName);
            drawLine();
            SingleSetting();
            EventSettting();
        }
    }
    
    public void OnExportSetting()
    {
        mbExportFolderOut = EditorGUILayout.Foldout(mbExportFolderOut, "导出剧情至lua脚本");
        if (mbExportFolderOut == true)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("剧情ID：");
            _dwScriptID = EditorGUILayout.IntField(_dwScriptID);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("导出"))
            {
                if (_animEvent == null)
                {
                    Debug.LogWarning("there is no Create Time Ctrl...");
                    return;
                }
                LuaAnimEvent.ExportToScriptFile(_animEvent, _dwScriptID);
            }
        }
    }
    //摄像机参数设置
    private void CameraSetting()
    {
        LuaAnimEvent.CameraSetting(_animEvent);
        drawLine();
    }
    //时间编辑参数
    private void SingleSetting()
    {
        GUILayout.Label("----"+szEditorState+"----");
        if (_animEvent.objEditorShotCtrl._objEditorEventCtrl != null)
        {
            
            _selectCtrl = _animEvent.objEditorShotCtrl._objEditorEventCtrl;
            szEditorState = "修改事件:"+_animEvent.objEditorShotCtrl.actorName+":"+_animEvent.objEditorShotCtrl.indexOf(_selectCtrl);
            //_animEvent.objEditorShotCtrl._objEditorEventCtrl = null;
            _selectCtrl.OnParamGUI();
        }
        else if (_selectCtrl != null)
        {
            szEditorState = "待添加事件";
            _selectCtrl.OnParamGUI();
        }
        drawLine();
        GUILayout.BeginHorizontal();
        _insertIndex = EditorGUILayout.IntField(_insertIndex, GUILayout.Width(30));
        if (GUILayout.Button("添加"))
        {
            if (_selectCtrl == null) return;
            StoryBaseCtrl objCtrl = _selectCtrl.CopySelf();
            objCtrl.ModInfo();
            _animEvent.objEditorShotCtrl.Add(objCtrl, _insertIndex);
            NGUITools.SetDirty(_animEvent);
            _insertIndex = -1;
        }
        if (GUILayout.Button("修改"))
        {
            if (_selectCtrl == null) return;
            _selectCtrl.ModInfo();
            NGUITools.SetDirty(_animEvent);
        }
        if (GUILayout.Button("存储点"))
        {
            if (_selectCtrl == null) return;
            _selectCtrl.SavePoint();
        }
        if (GUILayout.Button("重设"))
        {
            if (_selectCtrl == null) return;
            _selectCtrl.ResetPoint(false);
        }
        if (GUILayout.Button("放弃"))
        {
            if (_selectCtrl != null)
                _selectCtrl.ResetPoint(false);
            _selectCtrl = null;
            if (_animEvent.objEditorShotCtrl != null)
                _animEvent.objEditorShotCtrl._objEditorEventCtrl = null;
        }
        GUILayout.EndHorizontal();
        drawLine();
    }
    //事件列表区
    private void EventSettting()
    {
        int btnWidth = 200;
        GUILayout.Label("人物相关");
        if (GUILayout.Button("位置", GUILayout.Width(btnWidth)))
        {
            StoryPositionCtrl objCtrl = new StoryPositionCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("走动", GUILayout.Width(btnWidth)))
        {
            StoryMoveCtrl objCtrl = new StoryMoveCtrl();
            _selectCtrl = objCtrl;

        }
        if (GUILayout.Button("动作", GUILayout.Width(btnWidth)))
        {
            StoryAnimCtrl objCtrl = new StoryAnimCtrl();
            _selectCtrl = objCtrl;

        }
        if (GUILayout.Button("主角组装", GUILayout.Width(btnWidth)))
        {
            StoryPackActorCtrl objCtrl = new StoryPackActorCtrl();
            _selectCtrl = objCtrl;
        }
        GUILayout.Label("界面");
        if (GUILayout.Button("对话", GUILayout.Width(btnWidth)))
        {
            StoryTalkCtrl objCtrl = new StoryTalkCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("图片", GUILayout.Width(btnWidth)))
        {
            StoryPictureCtrl objCtrl = new StoryPictureCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("描述", GUILayout.Width(btnWidth)))
        {
            StoryUIDescCtrl objCtrl = new StoryUIDescCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("背景控制", GUILayout.Width(btnWidth)))
        {
            StoryUIBackCtrl objCtrl = new StoryUIBackCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("视频", GUILayout.Width(btnWidth)))
        {
            StoryVideoCtrl objCtrl = new StoryVideoCtrl();
            _selectCtrl = objCtrl;
        }
        
        
//         if (GUILayout.Button("选项", GUILayout.Width(btnWidth)))
//         {
//             StoryOptionCtrl objCtrl = new StoryOptionCtrl();
//             _selectCtrl = objCtrl;
//         }
        GUILayout.Label("效果");
        if (GUILayout.Button("特效", GUILayout.Width(btnWidth)))
        {
            StoryEffectCtrl objCtrl = new StoryEffectCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("音效", GUILayout.Width(btnWidth)))
        {
            StoryMusicCtrl objCtrl = new StoryMusicCtrl();
            _selectCtrl = objCtrl;
        }

        GUILayout.Label("摄像机");
        if (GUILayout.Button("分离", GUILayout.Width(btnWidth)))
        {
            StorySeparateCtrl objCtrl = new StorySeparateCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("合并", GUILayout.Width(btnWidth)))
        {
            StoryCombineCtrl objCtrl = new StoryCombineCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("目标", GUILayout.Width(btnWidth)))
        {
            StoryCameraLookCtrl objCtrl = new StoryCameraLookCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("缓动", GUILayout.Width(btnWidth)))
        {
            StoryCameraSmoothCtrl objCtrl = new StoryCameraSmoothCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("震屏", GUILayout.Width(btnWidth)))
        {
            StoryCameraShakeCtrl objCtrl = new StoryCameraShakeCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("广角设置", GUILayout.Width(btnWidth)))
        {
            StoryCameraFovCtrl objCtrl = new StoryCameraFovCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("淡入淡出", GUILayout.Width(btnWidth)))
        {
            StoryTweenFadeCtrl objCtrl = new StoryTweenFadeCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("画面去色", GUILayout.Width(btnWidth)))
        {
            StoryGrayscaleCtrl objCtrl = new StoryGrayscaleCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("Boss展示背景", GUILayout.Width(btnWidth)))
        {
            StoryMontageCtrl objCtrl = new StoryMontageCtrl();
            _selectCtrl = objCtrl;
        }
        GUILayout.Label("缓动变换相关");
        if (GUILayout.Button("直线变换", GUILayout.Width(btnWidth)))
        {
            StoryTweenMoveCtrl objCtrl = new StoryTweenMoveCtrl();
            _selectCtrl = objCtrl;
        }
        if (GUILayout.Button("旋转变换", GUILayout.Width(btnWidth)))
        {
            StoryTweenRotateCtrl objCtrl = new StoryTweenRotateCtrl();
            _selectCtrl = objCtrl;
        }
        GUILayout.Label("时间");
        if (GUILayout.Button("等待", GUILayout.Width(btnWidth)))
        {
            _selectCtrl = new StoryTimeCtrl();
        }
    }
    /*
    //更新
    void Update()
    {

    }

    void OnFocus()
    {
        Debug.Log("当窗口获得焦点时调用一次");
    }

    void OnLostFocus()
    {
        Debug.Log("当窗口丢失焦点时调用一次");
    }

    void OnHierarchyChange()
    {
        Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        Debug.Log("当Project视图中的资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        foreach (Transform t in Selection.transforms)
        {
            //有可能是多选，这里开启一个循环打印选中游戏对象的名称
            Debug.Log("OnSelectionChange" + t.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log("当窗口关闭时调用");
    }
     * */
}
