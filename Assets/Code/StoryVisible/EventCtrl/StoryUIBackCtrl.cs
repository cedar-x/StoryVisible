using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--UI背景类--事件的调度
/// 设计时间：2015-08-28
/// </summary>

namespace xxstory
{
    public class StoryUIBackCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public int type;
            public Vector3 localPosition;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryUIBackCtrl"; }
        }
        public override string ctrlName
        {
            get { return "UI背景"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("type");
            expList.Add("localPosition");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryUIBackCtrl obj = new StoryUIBackCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            //////本类事件属性赋值
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            Debug.Log("StoryUIBackCtrl:Execute:" + _realInfo.type + ":" + _realInfo.localPosition.ToString() + ":" + objStoryUI.shangBack.name);
            if (_realInfo.type == 0)
            {
                objStoryUI.btnClose.SetActive(false);
            }else if (_realInfo.type == 1)
            {
                objStoryUI.shangBack.SetActive(false);
            }
            else if (_realInfo.type == 2)
            {
                objStoryUI.xiaBack.SetActive(false);
            }
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
                case "type":
                    _normalInfo.type = lua.L_CheckInteger(-1);
                    break;
                case "localPosition":
                    _normalInfo.localPosition = LuaExport.GetVector3(lua, -1);
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
                case "type":
                    lua.PushInteger(_realInfo.type);
                    break;
                case "localPosition":
                    LuaExport.Vector3ToStack(lua, _realInfo.localPosition);
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
            _normalInfo.type = EditorGUILayout.IntField("type", _normalInfo.type);
            //_normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}