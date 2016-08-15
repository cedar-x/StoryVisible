using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera广角变换--事件的调度
/// 设计时间：2015-10-16
/// </summary>

namespace xxstory
{
    public class StoryCameraFovCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public int type;
            public float fieldOfView;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;

        private float yuanFoV;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryCameraFovCtrl"; }
        }
        public override string ctrlName
        {
            get { return "广角设置"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("fieldOfView");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryCameraFovCtrl obj = new StoryCameraFovCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            if (time == 0)
                objMainCamera.fieldOfView = _realInfo.fieldOfView;
            else
                yuanFoV = objMainCamera.fieldOfView;
        }
        public override void Update()
        {
            if (Time.time - startTime > time)
            {
                onEnd();
                yuanFoV = 0;
                OnFinish();
            }else if (yuanFoV != 0)
            {
                objMainCamera.fieldOfView = Mathf.Lerp(yuanFoV, _realInfo.fieldOfView, (Time.time - startTime) / time);
                if (objMainCamera.fieldOfView == _realInfo.fieldOfView)
                {
                    yuanFoV = 0;
                }
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
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "fieldOfView":
                    _normalInfo.fieldOfView = (float)lua.L_CheckNumber(-1);
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
                case "fieldOfView":
                    lua.PushNumber(_realInfo.fieldOfView);
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
            _normalInfo.fieldOfView = EditorGUILayout.FloatField("fieldOfView", _normalInfo.fieldOfView);
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}