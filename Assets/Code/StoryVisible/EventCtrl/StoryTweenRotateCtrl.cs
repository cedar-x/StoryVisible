using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--ITween旋转类--事件的调度
/// 设计时间：2015-08-14
/// </summary>

namespace xxstory
{
    public class StoryTweenRotateCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public iTween.EaseType easetype;
            public Vector3 localEulerAngles;
            public Vector3 directEulerAngles;
            public bool bInitEulerAngles;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        private Hashtable _paramHash;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryTweenRotateCtrl"; }
        }
        public override string ctrlName
        {
            get { return "ITweenRotate:"; }
        }
        public override void initInfo()
        {
            _normalInfo.easetype = iTween.EaseType.linear;
            _paramHash = new Hashtable();
            base.initInfo();
            expList.Add("directRotate");
            expList.Add("localEulerAngles");
            expList.Add("bInitRatate");
            expList.Add("easetype");
            
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryTweenRotateCtrl obj = new StoryTweenRotateCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            Transform target = _shotCtrl.actor.target.transform;
            if (_realInfo.bInitEulerAngles == true)
            {
                target.localEulerAngles = _realInfo.localEulerAngles;
            }
            SmoothTarget(target, _realInfo);
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
                case "directRotate":
                    _normalInfo.directEulerAngles = LuaExport.GetVector3(lua, -1);
                    break;
                case "localEulerAngles":
                    _normalInfo.localEulerAngles = LuaExport.GetVector3(lua, -1);
                    break;
                case "bInitRatate":
                    _normalInfo.bInitEulerAngles = lua.ToBoolean(-1);
                    break;
                case "easetype":
                    _normalInfo.easetype = (iTween.EaseType)lua.L_CheckInteger(-1);
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
                case "directRotate":
                    LuaExport.Vector3ToStack(lua, _realInfo.directEulerAngles);
                    break;
                case "localEulerAngles":
                    if (_realInfo.bInitEulerAngles == false)
                        return false;
                    LuaExport.Vector3ToStack(lua, _realInfo.localEulerAngles);
                    break;
                case "bInitRatate":
                    lua.PushBoolean(_realInfo.bInitEulerAngles);
                    break;
                case "easetype":
                    lua.PushInteger((int)_realInfo.easetype);
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
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("c", new GUILayoutOption[]{GUILayout.Height(20), GUILayout.Width(20)}))
            {
                _normalInfo.localEulerAngles = Selection.activeTransform.eulerAngles;
            }
            if (GUILayout.Button("s", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _shotCtrl.actor.target.transform.localEulerAngles = _normalInfo.directEulerAngles;
            }
            _normalInfo.localEulerAngles = EditorGUILayout.Vector3Field("localRotate", _normalInfo.localEulerAngles);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _normalInfo.directEulerAngles = Selection.activeTransform.eulerAngles;
            }
            if (GUILayout.Button("s", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _shotCtrl.actor.target.transform.localEulerAngles = _normalInfo.localEulerAngles;
            }
            _normalInfo.directEulerAngles = EditorGUILayout.Vector3Field("directRotate", _normalInfo.directEulerAngles);
            GUILayout.EndHorizontal();
            _normalInfo.easetype = (iTween.EaseType)EditorGUILayout.EnumPopup("easetype", _normalInfo.easetype);
            _normalInfo.bInitEulerAngles = GUILayout.Toggle(_normalInfo.bInitEulerAngles, "bInitRatate");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////
        private void SmoothTarget(Transform sTarget, paramInfo pmInfo)
        {
            _paramHash.Clear();
            _paramHash.Add("time", time);
            _paramHash.Add("rotation", pmInfo.directEulerAngles);
            _paramHash.Add("easetype", pmInfo.easetype);
//             _paramHash.Add("oncomplete", "OnProxyFinish");
//             _paramHash.Add("oncompleteparams", this);
//             _paramHash.Add("oncompletetarget", _baseCtrl.objProxy.gameObject);
            
            iTween.RotateTo(sTarget.gameObject, _paramHash);
        }
    }
}