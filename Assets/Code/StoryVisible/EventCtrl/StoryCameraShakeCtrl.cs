using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera震动--事件的调度
/// 设计时间：2015-10-16
/// </summary>

namespace xxstory
{
    public class StoryCameraShakeCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public int type;
            public float intensity;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryCameraShakeCtrl"; }
        }
        public override string ctrlName
        {
            get { return "震屏"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("type");
            expList.Add("intensity");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryCameraShakeCtrl obj = new StoryCameraShakeCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            if (time == 0f)
                objMainCamera.DoShake(_realInfo.type, 0, 0);
            else
                objMainCamera.DoShake(_realInfo.intensity, time);
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
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "type":
                    _normalInfo.type = lua.L_CheckInteger(-1);
                    break;
                case "intensity":
                    _normalInfo.intensity = (float)lua.L_CheckNumber(-1);
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
                case "intensity":
                    lua.PushNumber(_realInfo.intensity);
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
            _normalInfo.intensity = EditorGUILayout.FloatField("intensity", _normalInfo.intensity);
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}