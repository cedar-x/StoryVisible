using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--选项类--事件的调度
/// 设计时间：2015-09-21
/// </summary>

namespace xxstory
{
    public class StoryOptionCtrl : StoryBaseCtrl
    {
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryOptionCtrl"; }
        }
        public override string ctrlName
        {
            get { return "选项"; }
        }
        public override void initInfo()
        {
            base.initInfo();
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryOptionCtrl obj = new StoryOptionCtrl();
            obj.bClick = bClick;
            obj.bWait = bWait;
            obj.time = time;
            return obj;
        }
        public override void Execute()
        {
            ILuaState lua = Game.LuaApi;
            lua.GetGlobal("StoryShowOption");
            if (lua.PCall(0, 0, 0) != 0)
            {
                Debug.LogWarning(lua.ToString(-1));
                lua.Pop(-1);
            }
        }
        /// //////////////////属性导出导入部分-有导入导出需求时重写///////////////////////////////////////////////
//         protected override bool WidgetWriteOper(ILuaState lua, string key)
//         {
//             switch (key)
//             {
//                 default:
//                     return base.WidgetWriteOper(lua, key);
//             }
//             return true;
//         }
//         protected override bool WidgetReadOper(ILuaState lua, string key)
//         {
//             switch (key)
//             {
//                 default:
//                     return base.WidgetReadOper(lua, key);
//             }
//             return true;
//         }
#if UNITY_EDITOR 
        /// ////////////////UI显示部分-AddEvent页签中创建相应事件UI显示/////////////////////////////////////////////
        public override void OnParamGUI()
        {
            GUILayout.Label("StoryOptionCtrl");
            base.OnParamGUI();
        }
#endif

    }
}