using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif

/// <summary>
/// 设计目的：用于--音效类--事件的调度
/// 设计时间：2015-10-21
/// </summary>

namespace xxstory
{
    public class StoryMusicCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string musicName;
            public bool bLoop;
            public float volume;
        }
        //事件相关属性
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryMusicCtrl"; }
        }
        public override string ctrlName
        {
            get { return "音效"; }
        }
        public override void initInfo()
        {
            _normalInfo.volume = 1f;
            base.initInfo();
            expList.Add("musicName");
            expList.Add("bLoop");
            expList.Add("volume");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryMusicCtrl obj = new StoryMusicCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {

        }
        public override void OnFinish()
        {

        }
        public override void OnForceNext()
        {

        }
        public override void ModInfo()
        {
            SavePoint();
            _realInfo = _saveInfo;
        }
        public override void SavePoint()
        {
            _saveInfo = _normalInfo;
        }
        public override void ResetPoint(bool bRealInfo)
        {
            if (bRealInfo)
                _normalInfo = _realInfo;
            else
                _normalInfo = _saveInfo;
        }
        /// //////////////////属性导出导入部分-有导入导出需求时重写///////////////////////////////////////////////
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "musicName":

                    break;
                case "volume":
                    break;
                case "bLoop":
                    break;
                default:
                    return base.WidgetWriteOper(lua, key);
            }
            return true;
        }
        protected override bool WidgetReadOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "musicName":
                    break;
                case "volume":
                    break;
                case "bLoop":
                    break;
                default:
                    return base.WidgetReadOper(lua, key);
            }
            return true;
        }
#if UNITY_EDITOR 
        /// ////////////////UI显示部分-AddEvent页签中创建相应事件UI显示/////////////////////////////////////////////
        public override void OnParamGUI()
        {
            _normalInfo.musicName = EditorGUILayout.TextField("musicName", _normalInfo.musicName);
            _normalInfo.volume = EditorGUILayout.FloatField("volume", _normalInfo.volume);
            _normalInfo.bLoop = GUILayout.Toggle(_normalInfo.bLoop, "bLoop");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}