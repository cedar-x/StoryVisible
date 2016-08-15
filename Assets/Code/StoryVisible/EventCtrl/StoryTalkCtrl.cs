using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--对话类--事件的调度
/// 设计时间：2015-08-14
/// </summary>

namespace xxstory
{
    public class StoryTalkCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string talkName;
            public string talkInfo;
            public bool bHideNext;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryTalkCtrl"; }
        }
        public override string ctrlName
        {
            get { return "对话"; }
        }
        public override void initInfo()
        {
            _normalInfo.talkName = "";
            _normalInfo.talkInfo = "";
            base.initInfo();
            expList.Add("talkName");
            expList.Add("talkInfo");
            expList.Add("bHideNext");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryTalkCtrl obj = new StoryTalkCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            //////本类事件属性赋值
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            if (_realInfo.bHideNext == false)
                LuaAnimEvent.OnBtnNextState(true);
            ShowTalkInfo();
        }
        public override void OnFinish()
        {
            LuaAnimEvent.OnBtnNextState(false);
            HideTalkInfo();
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
            if (bRealInfo == true)
                _normalInfo = _realInfo;
            else
                _normalInfo = _saveInfo;
        }
        /// //////////////////属性导出导入部分-有导入导出需求时重写///////////////////////////////////////////////
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "talkInfo":
                    _normalInfo.talkInfo = lua.L_CheckString(-1);
                    break;
                case "talkName":
                    _normalInfo.talkName = lua.L_CheckString(-1);
                    break;
                case "bHideNext":
                    _normalInfo.bHideNext = lua.ToBoolean(-1);
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
                case "talkInfo":
                    lua.PushString(_realInfo.talkInfo);
                    break;
                case "talkName":
                    lua.PushString(_realInfo.talkName);
                    break;
                case "bHideNext":
                    lua.PushBoolean(_realInfo.bHideNext);
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
            _normalInfo.talkName = EditorGUILayout.TextField("talkName-名字", _normalInfo.talkName);
            _normalInfo.talkInfo = EditorGUILayout.TextField("talkInfo-内容", _normalInfo.talkInfo);
            _normalInfo.bHideNext = GUILayout.Toggle(_normalInfo.bHideNext, "bHideNext");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////
        public void ShowTalkInfo()
        {
            objStoryUI.talkName.text = _normalInfo.talkName;
            objStoryUI.talkInfo.text = _normalInfo.talkInfo;
        }
        public void HideTalkInfo()
        {
            objStoryUI.talkName.text = "";
            objStoryUI.talkInfo.text = "";
        }
    }
}