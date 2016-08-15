using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--ITween摄像机淡入淡出类--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StoryTweenFadeCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public int type;//淡入淡出类型：0-淡入，1-淡出
            public int easetype;
            public int directNum;
            public Color directColor;
            public bool bReset;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        private float yuanMiddleGrey;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryTweenFadeCtrl"; }
        }
        public override string ctrlName
        {
            get { return "淡入淡出:"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("directColor");
            expList.Add("directNum");
            expList.Add("time");
            expList.Add("dwType");
            expList.Add("bReset");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryTweenFadeCtrl obj = new StoryTweenFadeCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            ScreenOverlay tone = objMainCamera.gameObject.GetComponent<ScreenOverlay>();
            yuanMiddleGrey = -1f;
        }
        public override void OnFinish()
        {
            if (_saveInfo.bReset == true)
                objMainCamera.gameObject.GetComponent<ScreenOverlay>().intensity = 0;
        }
        public override void Update()
        {
            if (Time.time - startTime > time)
            {
                onEnd();
                OnFinish();
                yuanMiddleGrey = 0;
            }
            else if (_realInfo.type == 0 && yuanMiddleGrey != 0)
            {
                objMainCamera.GetComponent<ScreenOverlay>().intensity = Mathf.Lerp(0, _realInfo.directNum, (Time.time - startTime) / time);
            }else if (_realInfo.type == 1 && yuanMiddleGrey != 0)
            {
                objMainCamera.GetComponent<ScreenOverlay>().intensity = Mathf.Lerp(_realInfo.directNum, 0, (Time.time - startTime) / time);
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
                case "dwType":
                    _normalInfo.type = lua.L_CheckInteger(-1);
                    break;
                case "directColor":
                    _normalInfo.directColor = LuaExport.GetColor(lua, -1);
                    break;
                case "bReset":
                    _normalInfo.bReset = lua.ToBoolean(-1);
                    break;
                case "directNum":
                    _normalInfo.directNum = lua.ToInteger(-1);
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
                case "dwType":
                    lua.PushInteger(_realInfo.type);
                    break;
                case "directColor":
                    LuaExport.ColorToStack(lua, _realInfo.directColor);
                    break;
                case "directNum":
                    lua.PushInteger(_realInfo.directNum);
                    break;
                case "bReset":
                    lua.PushBoolean(_realInfo.bReset);
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("类型 0: 正常->目标值,1: 目标值->正常");
            _normalInfo.type = EditorGUILayout.IntField(_normalInfo.type);
            EditorGUILayout.EndHorizontal();
            //_normalInfo.directColor = EditorGUILayout.ColorField("directColor", _normalInfo.directColor);
            _normalInfo.directNum = EditorGUILayout.IntField("directNum", _normalInfo.directNum);
            _normalInfo.bReset = GUILayout.Toggle(_normalInfo.bReset, "bReset");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////
    }
}