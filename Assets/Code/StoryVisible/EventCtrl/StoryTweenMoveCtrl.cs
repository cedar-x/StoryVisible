using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--ITween移动类--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StoryTweenMoveCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public iTween.EaseType easetype;
            public Vector3 localPosition;
            public Vector3 directPosition;
            public bool bInitPosition;
        }
       
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        private Hashtable _paramHash;

        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryTweenMoveCtrl"; }
        }
        public override string ctrlName
        {
            get { return "ITweenMove:"; }
        }
        public override void initInfo()
        {
            _normalInfo.easetype = iTween.EaseType.linear;
            _paramHash = new Hashtable();
            base.initInfo();
            expList.Add("directPos");
            expList.Add("localPosition");
            expList.Add("easetype");
            expList.Add("bInitPosition");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryTweenMoveCtrl obj = new StoryTweenMoveCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            //////本类事件属性赋值
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            //Debug.Log("StoryTweenMove:Execute:" + _realInfo.bInitPosition+":" + _realInfo.localPosition.ToString() + ":" + _realInfo.directPosition.ToString());
            Transform target = _shotCtrl.actor.target.transform;
            if (_realInfo.bInitPosition == true)
            {
                target.localPosition = _realInfo.localPosition;
            }
            SmoothTarget(target, _realInfo);
        }
        public override void OnFinish()
        {
            GameObject target = _shotCtrl.actor.target;
            Debug.Log("StoryTweenMoveCtrl:OnFinish:" + target.name);
            iTween.Stop(target);
            target.transform.localPosition = _realInfo.directPosition;
        }
        public override void Stop()
        {
            GameObject target = _shotCtrl.actor.target;
            Debug.Log("StoryTweenMoveCtrl:Stop:" + target.name);
            iTween.Stop(target);
            target.transform.localPosition = _realInfo.directPosition;
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
                case "directPos":
                    _normalInfo.directPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "localPosition":
                    _normalInfo.localPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "bInitPosition":
                    _normalInfo.bInitPosition = lua.ToBoolean(-1);
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
                case "directPos":
                    LuaExport.Vector3ToStack(lua, _realInfo.directPosition);
                    break;
                case "localPosition":
                    LuaExport.Vector3ToStack(lua, _realInfo.localPosition);
                    break;
                case "bInitPosition":
                    lua.PushBoolean(_realInfo.bInitPosition);
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
            GUILayoutOption[] option = new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) };
            if (GUILayout.Button("c", option))
            {
                _normalInfo.localPosition = Selection.activeTransform.position;
            }
            if (GUILayout.Button("s", option))
            {
                _shotCtrl.actor.target.transform.localPosition = _normalInfo.localPosition;
            }
            _normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("c", option))
            {
                _normalInfo.directPosition = Selection.activeTransform.position;
            }
            if (GUILayout.Button("s", option))
            {
                _shotCtrl.actor.target.transform.localPosition = _normalInfo.directPosition;
            }
            _normalInfo.directPosition = EditorGUILayout.Vector3Field("directPosition", _normalInfo.directPosition);
            GUILayout.EndHorizontal();
            _normalInfo.easetype = (iTween.EaseType)EditorGUILayout.EnumPopup("easetype", _normalInfo.easetype);
            _normalInfo.bInitPosition = GUILayout.Toggle(_normalInfo.bInitPosition, "bInitPosition");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////
        private void SmoothTarget(Transform sTarget, paramInfo pmInfo)
        {
            _paramHash.Clear();
            _paramHash.Add("time", time);
            _paramHash.Add("position", pmInfo.directPosition);
            _paramHash.Add("easetype", pmInfo.easetype);
//             _paramHash.Add("oncomplete", "OnProxyFinish");
//             _paramHash.Add("oncompleteparams", this);
//             _paramHash.Add("oncompletetarget", _baseCtrl.objProxy.gameObject);
            
            iTween.MoveTo(sTarget.gameObject, _paramHash);
        }
    }
}