using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--主角组装类--事件的调度
/// 设计时间：2016-06-29
/// </summary>

namespace xxstory
{
    public class StoryPackActorCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string partName;
            public string skinMeshes;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryPackActorCtrl"; }
        }
        public override string ctrlName
        {
            get { return "主角组装"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("partName");
            expList.Add("skinMeshes");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryPackActorCtrl obj = new StoryPackActorCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            //////本类事件属性赋值
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {

            Actor target = _shotCtrl.actor.target.GetComponent<Actor>();
            if (target == null)
            {
                Debug.LogWarning("StoryPackActorCtrl Execute not have target");
                return;
            }
            string meshName = "";
            if (_realInfo.partName == "ride")
            {
                ILuaState lua = Game.LuaApi;
                lua.GetGlobal("StoryPackActor");
                lua.PushString(target.name);
                lua.PushString(_realInfo.partName);
                lua.PushString(_realInfo.skinMeshes);
                if (lua.PCall(3, 0, 0) != 0)
                {
                    Debug.LogWarning(lua.ToString(-1));
                    lua.Pop(-1);
                }
                return;
            }
            meshName = string.Format("Actor/{0}/{1}", _shotCtrl.actor.skeleton, _realInfo.skinMeshes);
            target.SetMeshPart(_realInfo.partName, meshName);
            target.Visible = true;

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
                case "partName":
                    _normalInfo.partName = lua.L_CheckString(-1);
                    break;
                case "skinMeshes":
                    _normalInfo.skinMeshes = lua.L_CheckString(-1);
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
                case "partName":
                    lua.PushString(_realInfo.partName);
                    break;
                case "skinMeshes":
                    lua.PushString(_realInfo.skinMeshes);
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
            _normalInfo.partName = EditorGUILayout.TextField("partName", _normalInfo.partName);
            _normalInfo.skinMeshes = EditorGUILayout.TextField("skinMeshes", _normalInfo.skinMeshes);
            GUILayout.Label("身体:body  武器:weapon 坐骑:ride");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}