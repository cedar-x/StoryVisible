using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera目标选取--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StoryCameraLookCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public normalInfo cameraParam;
            public int type;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryCameraLookCtrl"; }
        }
        public override string ctrlName
        {
            get { return "Camera目标"; }
        }
        public override void initInfo()
        {
            _normalInfo.type = 1;
            base.initInfo();
            expList.Add("cameraParam");
            expList.Add("type");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryCameraLookCtrl obj = new StoryCameraLookCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            Transform target = _shotCtrl.actor.target.transform;
            if (target == null)
            {
                Debug.LogWarning("StroyCamerLookCtrl have not target set.......please check.");
                return;
            }
            if (_realInfo.cameraParam.distance > 0f)
            {
                objMainCamera.StopTween();
                objMainCamera.UseParam(_realInfo.cameraParam);
                objMainCamera.LookTarget(target);
            }
            else
            {
                objMainCamera.target = target;
            }
        }
        //修改事件内容可以重写-点击页签AddEvent-修改时调用
        public override void ModInfo()
        {
            SavePoint();
            _realInfo = _saveInfo;
        }
        //保存存储点时可以重写-点击页签AddEvent-存储点时调用
        public override void SavePoint()
        {
            _saveInfo = _normalInfo;
        }
        //重设存储点时可以重写-点击页签AddEvent-重设时调用
        public override void ResetPoint(bool bRealInfo)
        {
            if (bRealInfo == false)
                _normalInfo = _saveInfo;
            else
                _normalInfo = _realInfo;
        }
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "cameraParam":
                    _normalInfo.cameraParam = LuaExport.GetNormalInfo(lua, -1);
                    break;
                case "type":
                    _normalInfo.type = lua.L_CheckInteger(-1);
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
                case "cameraParam":
                    LuaExport.NormalInfoToStack(lua, _normalInfo.cameraParam);
                    break;
                case "type":
                    lua.PushInteger(_normalInfo.type);
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
            StoryBaseCtrl.OnCameraInfoGUI(ref _normalInfo.cameraParam);
            if (_shotCtrl != null && GUILayout.Button("Flush"))
            {
                objMainCamera.type = (CameraCastType)_normalInfo.type;
                objMainCamera.UseParam(_normalInfo.cameraParam);
                objMainCamera.LookTarget(_shotCtrl.actor.target.transform, false);  
            }
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}