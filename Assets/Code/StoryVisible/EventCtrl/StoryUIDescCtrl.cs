using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--UI文字描述--事件的调度
/// 设计时间：2015-10-10
/// </summary>

namespace xxstory
{
    public class StoryUIDescCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string descName;
            public string childName;
            public Vector3 localPosition;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        private HUIShowPicture mPicture;
        public List<Texture2D> _textures = new List<Texture2D>();

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryUIDescCtrl"; }
        }
        public override string ctrlName
        {
            get { return "描述"; }
        }
        public override void initInfo()
        {
            _normalInfo.childName = "grid";
            base.initInfo();
            expList.Add("descName");
            expList.Add("childName");
            expList.Add("localPosition");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryUIDescCtrl obj = new StoryUIDescCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            if (mPicture == null)
            {
                GameObject objRes = ResManager.LoadResource("UI/" + _realInfo.descName) as GameObject;
                if (objRes == null)
                {
                    Debug.LogWarning("can't find HUIShowPicture in path:" + _realInfo.descName);
                    return;
                }
                GameObject pChild = NGUITools.AddChild(objStoryUI.backGround.gameObject, objRes);
                pChild.gameObject.name = "StoryDesc";
                pChild.transform.localPosition = _realInfo.localPosition;
                mPicture = pChild.transform.FindChild(_realInfo.childName).GetComponent<HUIShowPicture>();
                mPicture.InitMember();
            }
            mPicture.showPicture();
        }
        public override void OnFinish()
        {
            if (mPicture != null)
                GameObject.DestroyObject(mPicture.transform.parent.gameObject);
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
                case "descName":
                    _normalInfo.descName = lua.L_CheckString(-1);
                    break;
                case "childName":
                    _normalInfo.childName = lua.L_CheckString(-1);
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
                case "descName":
                    lua.PushString(_realInfo.descName);
                    break;
                case "childName":
                    lua.PushString(_realInfo.childName);
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
            _normalInfo.descName = EditorGUILayout.TextField("descName", _normalInfo.descName);
            _normalInfo.childName = EditorGUILayout.TextField("childName", _normalInfo.childName);
            _normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
            base.OnParamGUI();
        }
#endif
    }
}