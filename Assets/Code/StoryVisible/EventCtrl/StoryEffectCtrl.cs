using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--特效类--事件的调度
/// 设计时间：2015-08-03
/// </summary>

namespace xxstory
{
    public class StoryEffectCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public int type;
            public string effect;
            public Vector3 localPosition;
            public Vector3 localEulerAngles;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;

        private LuaEffect _objEffect;
        private HUIParticle _objPEffect;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryEffectCtrl"; }
        }
        public override string ctrlName
        {
            get { return "特效"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("effect");
            expList.Add("type");
            expList.Add("localPosition");
            expList.Add("localEulerAngles");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryEffectCtrl obj = new StoryEffectCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            if (_normalInfo.type == 0)
            {
                _objEffect = ResManager.CreateEffect(ref _realInfo.effect);
                _objEffect.transform.localPosition = _realInfo.localPosition;
                _objEffect.transform.localEulerAngles = _realInfo.localEulerAngles;
                _objEffect.Play(0);
            }
            else
            {
                _objPEffect = ResManager.CreateParticleEffect(ref _realInfo.effect);
                _objPEffect.transform.localPosition = _realInfo.localPosition;
                _objPEffect.transform.localScale = _realInfo.localEulerAngles;
                _objPEffect.PlayEffect(0, 0);
            }
        }
        public override void Update()
        {
            base.Update();
            if (_objPEffect != null)
            {
                _objPEffect.transform.localPosition = _realInfo.localPosition;
                _objPEffect.transform.localScale = _realInfo.localEulerAngles;
            }
        }
        public override void OnFinish()
        {
            if (_objEffect != null)
                ResManager.DestroyEffect(_objEffect);
            if (_objPEffect != null)
            {
                ResManager.DestroyObject(_objPEffect);
            }
        }
        public override void OnForceNext()
        {

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
                case "effect":
                    _normalInfo.effect = lua.L_CheckString(-1);
                    break;
                case "type":
                    _normalInfo.type = lua.L_CheckInteger(-1);
                    break;
                case "localPosition":
                    _normalInfo.localPosition = LuaExport.GetVector3(lua, -1);
                    break;
                case "localEulerAngles":
                    _normalInfo.localEulerAngles = LuaExport.GetVector3(lua, -1);
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
                case "effect":
                    lua.PushString(_realInfo.effect);
                    break;
                case "type":
                    lua.PushInteger(_realInfo.type);
                    break;
                case "localPosition":
                    LuaExport.Vector3ToStack(lua, _realInfo.localPosition);
                    break;
                case "localEulerAngles":
                    LuaExport.Vector3ToStack(lua, _realInfo.localEulerAngles);
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
            _normalInfo.effect = EditorGUILayout.TextField("effect", _normalInfo.effect);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
                {
                    _normalInfo.localPosition = Selection.activeTransform.position;
                }
                _normalInfo.localPosition = EditorGUILayout.Vector3Field("localPosition", _normalInfo.localPosition);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("c", new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) }))
                {
                    _normalInfo.localEulerAngles = Selection.activeTransform.localEulerAngles;
                }
                _normalInfo.localEulerAngles = EditorGUILayout.Vector3Field("localEulerAngles", _normalInfo.localEulerAngles);
                GUILayout.EndHorizontal();
            base.OnParamGUI();
        }
#endif

    }
}