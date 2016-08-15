using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera分离类--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StorySeparateCtrl : StoryBaseCtrl
    {
        private bool _bClear;//是否清除摄像机目标
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StorySeparateCtrl"; }
        }
        public override string ctrlName
        {
            get { return "Camera分离"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("bClear");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StorySeparateCtrl obj = new StorySeparateCtrl();
            obj.bClick = bClick;
            obj.bWait = bWait;
            obj.time = time;
            obj._bClear = _bClear;
            return obj;
        }
        public override void Execute()
        {
            if (_bClear == true)
                objMainCamera.target = null;
            objMainCamera.SeparateParent();
        }
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "bClear":
                    _bClear = lua.ToBoolean(-1);
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
                case "bClear":
                    lua.PushBoolean(_bClear);
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
            _bClear = GUILayout.Toggle(_bClear, "clear target");
            base.OnParamGUI();
        }
#endif

    }
}