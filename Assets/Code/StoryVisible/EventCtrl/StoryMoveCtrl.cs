using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Actor移动--事件的调度
/// 设计时间：2015-08-03
/// </summary>

namespace xxstory
{
    public class StoryMoveCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public Vector3 localPosition;
            public Vector3 directPosition;
            public float speed;
            public int dwMMethod;
            public bool bInitPosition;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        public override void initInfo()
        {
            _normalInfo.speed = 1;
            _normalInfo.dwMMethod = 1;
            base.initInfo();
            expList.Add("speed");
            expList.Add("localPosition");
            expList.Add("directPos");
            expList.Add("bInitPosition");
            expList.Add("dwMMethod");
        }
        public override string luaName
        {
            get
            {
                return "StoryMoveCtrl";
            }
        }
        public override string ctrlName
        {
            get
            {
                return "走动";
            }
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryMoveCtrl obj = new StoryMoveCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        /// ///////////////////////////////////////////////////////////
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
        public override void OnFinish()
        {
            Actor target = _shotCtrl.actor.target.GetComponent<Actor>();
            target.onMoveStop -= this.OnFinish;
        }
        public override void Stop()
        {
            Actor target = _shotCtrl.actor.target.GetComponent<Actor>();
            target.onMoveStop -= this.OnFinish;
            target.MoveStop();
            target.transform.localPosition = _realInfo.directPosition;
        }
        public override void Execute()
        {
            Actor target = _shotCtrl.actor.target.GetComponent<Actor>();
            if (_realInfo.bInitPosition == true)
                target.transform.localPosition = _realInfo.localPosition;
            target.onMoveStop += this.OnFinish;
            target.MoveMethod = _realInfo.dwMMethod;
            target.SetMoveTime(time);
            target.SetSpeed(_realInfo.speed);
            target.MoveTo(_realInfo.directPosition.x, _realInfo.directPosition.y, _realInfo.directPosition.z, false, 0, 0, 0);
        }
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "speed":
                    _normalInfo.speed = (float)lua.L_CheckNumber(-1);
                    break;
                case "dwMMethod":
                    _normalInfo.dwMMethod = lua.L_CheckInteger(-1);
                    break;
                case "localPosition":
                    _normalInfo.localPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "directPos":
                    _normalInfo.directPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "bInitPosition":
                    _normalInfo.bInitPosition = lua.ToBoolean(-1);
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
                case "speed":
                    lua.PushNumber(_realInfo.speed);
                    break;
                case "dwMMethod":
                    lua.PushInteger(_realInfo.dwMMethod);
                    break;
                case "bInitPosition":
                    lua.PushBoolean(_realInfo.bInitPosition);
                    break;
                case "localPosition":
                    LuaExport.Vector3ToStack(lua, _realInfo.localPosition);
                    break;
                case "directPos":
                    LuaExport.Vector3ToStack(lua, _realInfo.directPosition);
                    break;
                default:
                    return base.WidgetReadOper(lua, key);
            }
            return true;
        }
#if UNITY_EDITOR 
        public override void OnParamGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _normalInfo.localPosition = Selection.activeTransform.position;
            }
            if (GUILayout.Button("s", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _shotCtrl.actor.target.transform.localPosition = _normalInfo.localPosition;
            }
            _normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _normalInfo.directPosition = Selection.activeTransform.position;
            }
            if (GUILayout.Button("s", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _shotCtrl.actor.target.transform.localPosition = _normalInfo.directPosition;
            }
            _normalInfo.directPosition = EditorGUILayout.Vector3Field("directPosition", _normalInfo.directPosition);
            EditorGUILayout.EndHorizontal();
            _normalInfo.speed = EditorGUILayout.FloatField("speed", _normalInfo.speed);
            _normalInfo.dwMMethod = EditorGUILayout.IntField("dwMMethod", _normalInfo.dwMMethod);
            _normalInfo.bInitPosition = GUILayout.Toggle(_normalInfo.bInitPosition, "bInitPosition");
            base.OnParamGUI();
        }
#endif
    }
}
