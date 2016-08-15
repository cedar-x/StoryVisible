using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Transform坐标设置--事件的调度
/// 设计时间：2015-08-04
/// </summary>

namespace xxstory
{
    public class StoryPositionCtrl : StoryBaseCtrl
    {    
        private struct paramInfo
        {
            public Vector3 localPosition;
            public Vector3 localEulerAngles;
            public Vector3 localScale;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        public override string luaName
        {
            get
            {
                return "StoryPositionCtrl";
            }
        }
        public override string ctrlName
        {
            get
            {
                return "设置坐标";
            }
        }
        public override void initInfo()
        {
            _normalInfo.localScale = Vector3.one;
            base.initInfo();
            expList.Add("localPosition");
            expList.Add("localEulerAngles");
            expList.Add("localScale");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryPositionCtrl obj = new StoryPositionCtrl();
            obj.bClick = bClick;
            obj.bWait = bWait;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            Transform target = _shotCtrl.actor.target.transform;
            //如果是人物则停止人物移动
            Actor objActor = target.GetComponent<Actor>();
            if (objActor != null)
            {
                objActor.MoveStop(false);
            }
            target.localPosition = _realInfo.localPosition;
            target.localEulerAngles = _realInfo.localEulerAngles;
            target.localScale = _realInfo.localScale;
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
                case "localPosition":
                    _normalInfo.localPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "localEulerAngles":
                    _normalInfo.localEulerAngles = LuaExport.GetVector3(lua, -1);
                    break;
                case "localScale":
                    _normalInfo.localScale = LuaExport.GetVector3(lua, -1);
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
                case "localPosition":
                    LuaExport.Vector3ToStack(lua, _realInfo.localPosition);
                    break;
                case "localEulerAngles":
                    LuaExport.Vector3ToStack(lua, _realInfo.localEulerAngles);
                    break;
                case "localScale":
                    LuaExport.Vector3ToStack(lua, _realInfo.localScale);
                    break;
                default:
                    return base.WidgetReadOper(lua, key);
            }
            return true;
        }

#if UNITY_EDITOR 
        public override void OnParamGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
            {
                _normalInfo.localPosition = Selection.activeTransform.position;
            }
            _normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
            GUILayout.EndHorizontal();
            _normalInfo.localEulerAngles = EditorGUILayout.Vector3Field("localRotate", _normalInfo.localEulerAngles);
            _normalInfo.localScale = EditorGUILayout.Vector3Field("localScale", _normalInfo.localScale);
            base.OnParamGUI();
        }
#endif

    }
}
