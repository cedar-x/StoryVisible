using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using xxstory;

namespace xxstoryEditor
{
    public class TrackGroupControl : SidebarControl
    {
        public class TrackGroupStyles
        {
            public GUIStyle addIcon;

            public GUIStyle InspectorIcon;

            public GUIStyle trackGroupArea;

            public GUIStyle pickerStyle;

            public GUIStyle backgroundSelected;

            public GUIStyle backgroundContentSelected;

            public GUIStyle DirectorGroupIcon;

            public GUIStyle ActorGroupIcon;

            public GUIStyle MultiActorGroupIcon;

            public GUIStyle CharacterGroupIcon;

            public GUIStyle UpArrowIcon;

            public GUIStyle DownArrowIcon;

            public GUIStyle BoxSelect;

            public GUIStyle PlayIcon;

            public TrackGroupStyles(GUISkin skin)
            {
                this.addIcon = skin.FindStyle("Add");
                this.InspectorIcon = skin.FindStyle("InspectorIcon");
                this.trackGroupArea = skin.FindStyle("Track Group Area");
                this.DirectorGroupIcon = skin.FindStyle("DirectorGroupIcon");
                this.ActorGroupIcon = skin.FindStyle("ActorGroupIcon");
                this.MultiActorGroupIcon = skin.FindStyle("MultiActorGroupIcon");
                this.CharacterGroupIcon = skin.FindStyle("CharacterGroupIcon");
                this.pickerStyle = skin.FindStyle("Picker");
                this.backgroundSelected = skin.FindStyle("TrackGroupFocused");
                this.backgroundContentSelected = skin.FindStyle("TrackGroupContentFocused");
                this.UpArrowIcon = skin.FindStyle("UpArrowIcon");
                this.DownArrowIcon = skin.FindStyle("DownArrowIcon");
                this.BoxSelect = skin.FindStyle("BoxSelect");
                this.PlayIcon = skin.FindStyle("PlayIcon");
                if (this.addIcon == null || this.InspectorIcon == null || this.trackGroupArea == null
                    || this.DirectorGroupIcon == null || this.ActorGroupIcon == null || this.MultiActorGroupIcon == null
                    || this.CharacterGroupIcon == null || this.pickerStyle == null || this.backgroundSelected == null
                    || this.backgroundContentSelected == null || this.UpArrowIcon == null
                    || this.DownArrowIcon == null || this.BoxSelect == null||this.PlayIcon == null)
                {
                    Debug.Log("StoryAnimator GUI Skin not loaded properly. Please check the guiskin file in the Resources folder.");
                }
            }
        }
        public class TrackGroupTextures
        {
            public Texture playImage = null;
            public Texture pauseImage = null;
            public Texture stopImage = null;
            public Texture settingsImage = null;
            public Texture rescaleImage = null;
            public Texture zoomInImage = null;
            public Texture zoomOutImage = null;
            public Texture snapImage = null;
            public Texture rollingEditImage = null;
            public Texture rippleEditImage = null;
            public Texture pickerImage = null;
            public Texture refreshImage = null;
            public Texture titleImage = null;
            public Texture cropImage = null;
            public Texture scaleImage = null;
            public Texture curvecanvasImage = null;

            private const string PRO_SKIN = "Director_LightSkin";
            private const string FREE_SKIN = "Director_DarkSkin";
            private const string PLAY_ICON = "Director_PlayIcon";
            private const string PAUSE_ICON = "Director_PauseIcon";
            private const string STOP_ICON = "Director_StopIcon";
            private const string SETTINGS_ICON = "Director_SettingsIcon";
            private const string HORIZONTAL_RESCALE_ICON = "Director_HorizontalRescaleIcon";
            private const string PICKER_ICON = "Director_PickerIcon";
            private const string REFRESH_ICON = "Director_RefreshIcon";
            private const string MAGNET_ICON = "Director_Magnet";
            private const string ZOOMIN_ICON = "Director_ZoomInIcon";
            private const string ZOOMOUT_ICON = "Director_ZoomOutIcon";
            private const string TITLE_ICON = "Director_Icon";
            private const string CURVECANVAS = "Director_CurveCanvas";
            
            public TrackGroupTextures(){
                string suffix = "_Light";//EditorGUIUtility.isProSkin ? "_Light" : "_Dark";
                string missing = " is missing from Resources folder.";

                playImage = Resources.Load<Texture>(PLAY_ICON);
                if (playImage == null)
                {
                    Debug.Log(PLAY_ICON + suffix + missing);
                }

                pauseImage = Resources.Load<Texture>(PAUSE_ICON);
                if (pauseImage == null)
                {
                    Debug.Log(PAUSE_ICON + suffix + missing);
                }

                stopImage = Resources.Load<Texture>(STOP_ICON);
                if (stopImage == null)
                {
                    Debug.Log(STOP_ICON + suffix + missing);
                }

                refreshImage = Resources.Load<Texture>(REFRESH_ICON + suffix);
                if (refreshImage == null)
                {
                    Debug.Log(REFRESH_ICON + suffix + missing);
                }

                snapImage = Resources.Load<Texture>(MAGNET_ICON + suffix);
                if (snapImage == null)
                {
                    Debug.Log(MAGNET_ICON + suffix + missing);
                }

                rescaleImage = Resources.Load<Texture>(HORIZONTAL_RESCALE_ICON + suffix);
                if (rescaleImage == null)
                {
                    Debug.Log(HORIZONTAL_RESCALE_ICON + suffix + missing);
                }

                zoomInImage = Resources.Load<Texture>(ZOOMIN_ICON + suffix);
                if (zoomInImage == null)
                {
                    Debug.Log(ZOOMIN_ICON + suffix + missing);
                }

                zoomOutImage = Resources.Load<Texture>(ZOOMOUT_ICON + suffix);
                if (zoomOutImage == null)
                {
                    Debug.Log(ZOOMOUT_ICON + suffix + missing);
                }
                curvecanvasImage = Resources.Load<Texture>(CURVECANVAS + suffix);
                if (curvecanvasImage == null)
                {
                    Debug.Log(CURVECANVAS + suffix + missing);
                }
            }
        }

        public static TrackGroupControl.TrackGroupStyles styles;
        public static TrackGroupControl.TrackGroupTextures textures;
        public static string[,] EventItemName = { 
                                                    {"人物相关","位置",         "StoryPositionCtrl"}, 
                                                    {"人物相关","走动",         "StoryMoveCtrl"}, 
                                                    {"人物相关","动作",         "StoryAnimCtrl"}, 
                                                    {"人物相关","主角组装",     "StoryPackActorCtrl"}, 
                                                    {"界面","对话",             "StoryTalkCtrl"}, 
                                                    {"界面","图片",             "StoryPictureCtrl"}, 
                                                    {"界面","描述",             "StoryUIDescCtrl"}, 
                                                    {"界面","背景控制",         "StoryUIBackCtrl"}, 
                                                    {"界面","视频",             "StoryVideoCtrl"}, 
                                                    {"效果","特效",             "StroyEffectCtrl"}, 
                                                    {"效果","音效",             "StoryMusicCtrl"}, 
                                                    {"摄像机","分离",           "StorySeparateCtrl"}, 
                                                    {"摄像机","合并",           "StoryCombineCtrl"}, 
                                                    {"摄像机","目标",           "StoryCameraLookCtrl"}, 
                                                    {"摄像机","缓动",           "StoryCameraSmoothCtrl"}, 
                                                    {"摄像机","震屏",           "StoryCameraShakeCtrl"}, 
                                                    {"摄像机","广角设置",       "StoryCameraFovCtrl"}, 
                                                    {"摄像机","淡入淡出",       "StoryTweenFadeCtrl"}, 
                                                    {"摄像机","画面去色",       "StoryGrayscaleCtrl"}, 
                                                    {"摄像机","Boss背景",       "StoryMontageCtrl"}, 
                                                    {"缓动变换","直线",         "StoryTweenMoveCtrl"}, 
                                                    {"缓动变换","旋转",         "StoryTweenRotateCtrl"}, 
                                                    {"时间","等待",             "StoryTimeCtrl"}, 
                                                };
        public static void InitStyles(GUISkin skin)
        {
            if (TrackGroupControl.styles == null)
            {
                TrackGroupControl.styles = new TrackGroupControl.TrackGroupStyles(skin);
                TrackGroupControl.textures = new TrackGroupControl.TrackGroupTextures();
            }
        }

        public static void ShowAddTrackContextMenu()
        {
            GenericMenu createMenu = new GenericMenu();
            for (int i = 0; i < EventItemName.GetLength(0); ++i)
            {
                string baseCtrlName = string.Format("{0}/{1}", EventItemName[i, 0], EventItemName[i, 1]);
                createMenu.AddItem(new GUIContent(baseCtrlName), false, AddAnimEventItem, EventItemName[i, 2]);
            }
            createMenu.ShowAsContext();
        }

        public static void AddAnimEventItem(object userData)
        {
            string data = userData as string;
            Debug.Log("AddAnimEventItem:" + data);
        }
    }
}
