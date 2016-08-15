using UnityEngine;
using System.Collections;
using UnityEditor;
using Hstj;
using xxstory;
using xxstoryEditor;

public class StoryAnimationEditorWindow : EditorWindow
{ 
#region Window
    const string TITLE = "StoryAnimator";
    const string MENU_ITEM = "Story Export/StoryAnimator";

    private StoryLayoutEditorControl storyEditorControl;
    private LuaAnimEvent luaAnimEvent;

    [MenuItem(MENU_ITEM, false, 10)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StoryAnimationEditorWindow));
    }

    public void Awake()
    {
#if UNITY_5 && !UNITY_5_0
        base.titleContent = new GUIContent(TITLE, TrackGroupControl.textures.titleImage);
#else
        base.title = TITLE;
#endif
        this.minSize = new Vector2(550, 250);
    }

    public void OnInspectorUpdate()
    {
        this.Repaint(); // 刷新Inspector
    }

    public void OnEnable()
    {
        storyEditorControl = new StoryLayoutEditorControl();
        GUISkin skin = Resources.Load("Director_LightSkin") as GUISkin;
        TrackGroupControl.InitStyles(skin);
        TimelineTrackControl.InitStyles(skin);
    }

    public void OnDisable()
    {

    }

    public void OnDestroy()
    {
    }

#endregion

    private string[] toolbarStrings = { "Cutscene List", "Inspector", "Cutscene" };
    private int toolbarInt = 1;
    private float[] gridValues = { 0.0f, 0.05f, 0.1f, 0.2f, 0.5f, 1.0f };
    private string[] gridNames = { "Off", "00:00:05", "00:00:10", "00:00:20", "00:00:50", "00:01:00" };
    private int currentGridIndex = 2;
    private bool isSnappingEnabled;
    //窗口尺寸参数
    private float rightwidth = 400f;
    private float headheight = 18f;
    private float leftnamewidth = 200f;
    private float leftclipheight = 20f;
    private float righttopheight = 20f;
    private float lefteventwidth;
    private Rect cacheSize;
    private Rect headsize;
    private Rect leftsize;
    private Rect lefteventsize;
    private Rect rightsize;
    private Rect rightbottomsize;
    private Rect timelineRect;


    private void InitParam()
    {
        if (position != this.cacheSize)
        {
            this.cacheSize = position;
            this.lefteventwidth = position.width - rightwidth - leftnamewidth;
            this.headsize = new Rect(0, 0, position.width, headheight);
            this.leftsize = new Rect(0, headheight, position.width - rightwidth, position.height - headheight);
            this.lefteventsize = new Rect(leftnamewidth, headheight + 2 * leftclipheight, leftsize.width - leftnamewidth, leftsize.height - 2 * leftclipheight);
            this.rightsize = new Rect(position.width - rightwidth, headheight, rightwidth, position.height - headheight);
            this.rightbottomsize = new Rect(rightsize.x, rightsize.y + righttopheight, rightsize.width, rightsize.height - righttopheight);
            this.timelineRect = new Rect(leftnamewidth, headheight, lefteventwidth - 14, position.height - headheight);
        }
        if (luaAnimEvent == null)
        {
            luaAnimEvent = GameObject.FindObjectOfType<LuaAnimEvent>();
        }
    }

    private void OnGUI()
    {
        InitParam();
        displayHead();
        displayLeft();
        dispachEvent();

    }

    private void displayHead()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.FlexibleSpace();
        //Snap settings
        bool tempSnapping = GUILayout.Toggle(isSnappingEnabled, TrackGroupControl.textures.snapImage, EditorStyles.toolbarButton, GUILayout.Width(24));
        if (tempSnapping != isSnappingEnabled)
        {
            isSnappingEnabled = tempSnapping;
            //this.storyEditorControl.IsSnappingEnabled = isSnappingEnabled;
        }
        currentGridIndex = EditorGUILayout.Popup(currentGridIndex, gridNames, EditorStyles.toolbarPopup, GUILayout.Width(70));
        if (GUILayout.Button(TrackGroupControl.textures.rescaleImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            this.storyEditorControl.Rescale();
        }
        if (GUILayout.Button(TrackGroupControl.textures.zoomInImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            this.storyEditorControl.ZoomIn();
        }
        if (GUILayout.Button(TrackGroupControl.textures.zoomOutImage, EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            this.storyEditorControl.ZoomOut();
        }
        GUILayout.Space(100);
        GUILayout.EndHorizontal();
    }

    private void dispachEvent()
    {
        GUI.Box(this.rightsize, GUIContent.none, "AnimationCurveEditorBackground");
        toolbarInt = GUI.Toolbar(new Rect(rightsize.x, rightsize.y, rightwidth, righttopheight), toolbarInt, toolbarStrings);
        GUILayout.BeginArea(rightbottomsize);
        if (toolbarInt == 0)
        {
             if (this.storyEditorControl != null)
                 this.storyEditorControl.updateExportInfo();
        }
        else if (toolbarInt == 1)
        {
             if (this.storyEditorControl != null)
                 this.storyEditorControl.updateInspector(this.rightbottomsize);
        }
        else if (toolbarInt == 2)
        {
            if (this.storyEditorControl != null)
                this.storyEditorControl.updateExtraFunc();
        }
        GUILayout.EndArea();
    }

    private void displayLeft()
    {

        //绘制左侧角色名称和事件显示
        this.storyEditorControl.OnGUI(this.leftsize, luaAnimEvent);
        //Dark bar at edge of work area.
        //GUI.Box(new Rect(this.rightsize.x - 14, this.headheight, 14, this.cacheSize.height), new GUIContent(TrackGroupControl.textures.curvecanvasImage));
    }










    //最后没用删除
    public static LuaAnimEvent animEvent;

    public static bool GetStoryAnimControl()
    {

           if (animEvent==null)
           {
               animEvent = FindObjectOfType(typeof(LuaAnimEvent)) as LuaAnimEvent;
               if (animEvent == null)
                   return false;
                return true;
           }
           return true;
    }
    private string FormatTime(float time)
    {
        int num = (int)Mathf.Floor(time / 60f);
        int num2 = (int)Mathf.Repeat(time, 60f);
        int num3 = (int)Mathf.Repeat(time * 100f, 100f);
        string text = string.Empty;
        if (num < 10)
        {
            text += "0";
        }
        text += num;
        text += ":";
        if (num2 < 10)
        {
            text += "0";
        }
        text += num2;
        text += ":";
        if (num3 < 10)
        {
            text += "0";
        }
        return text + num3;
    }

}
