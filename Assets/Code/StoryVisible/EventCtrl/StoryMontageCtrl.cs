using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--蒙太奇效果类--事件的调度
/// 设计时间：2015-08-03
/// </summary>

namespace xxstory
{
    public class StoryMontageCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string spName;
            public float distance;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        //事件相关属性
        private LuaMeshImage _meshImage;
        private int yuanCullingMask;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryMontageCtrl"; }
        }
        public override string ctrlName
        {
            get { return "Boss背景"; }
        }
        public override void initInfo()
        {
            _normalInfo.distance = 10f;
            base.initInfo();
            expList.Add("spName");
            expList.Add("distance");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryMontageCtrl obj = new StoryMontageCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            this.yuanCullingMask = objMainCamera.cullingMask;
            _meshImage = objMainCamera.gameObject.GetComponentInChildren<LuaMeshImage>();
            _meshImage.transform.localPosition = new Vector3(0f, 0f, _realInfo.distance);
            _meshImage.init(_realInfo.spName, objMainCamera.fieldOfView);
            objMainCamera.cullingMask = 544;
        }
        public override void OnFinish()
        {
            objMainCamera.cullingMask = this.yuanCullingMask;
            _meshImage.Clear();
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
                case "spName":
                    _normalInfo.spName = lua.L_CheckString(-1);
                    break;
                case "distance":
                    _normalInfo.distance = (float)lua.L_CheckNumber(-1);
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
                case "spName":
                    lua.PushString(_realInfo.spName);
                    break;
                case "distance":
                    lua.PushNumber(_realInfo.distance);
                    break;
                default:
                    return base.WidgetReadOper(lua, key);
            }
            return true;
        }
#if UNITY_EDITOR 
        public override void OnParamGUI()
        {
            _normalInfo.spName = EditorGUILayout.TextField("spName", _normalInfo.spName);
            _normalInfo.distance = EditorGUILayout.FloatField("distance", _normalInfo.distance);
            base.OnParamGUI();
        }
#endif

    }
}