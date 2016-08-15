using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera合并类--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StoryCombineCtrl : StoryBaseCtrl
    {
        public bool bRecalculate; 
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryCombineCtrl"; }
        }
        public override string ctrlName
        {
            get { return "Camera合并"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("bRecalculate");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryCombineCtrl obj = new StoryCombineCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj.bRecalculate = bRecalculate;
            return obj;
        }
        public override void Execute()
        {
            if (bRecalculate == false)
                objMainCamera.ResetParam(_shotCtrl.actor.target.transform);
            objMainCamera.CombineParent();
        }
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "bRecalculate":
                    bRecalculate = lua.ToBoolean(-1);;
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
                case "bRecalculate":
                    lua.PushBoolean(bRecalculate);
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
            bRecalculate = GUILayout.Toggle(bRecalculate, "bRecalculate");
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}