using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--阶段结束类--事件的调度
/// 设计时间：2015-09-10
/// </summary>

namespace xxstory
{
    public class StoryEndCtrl : StoryBaseCtrl
    {
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryEndCtrl"; }
        }
        public override string ctrlName
        {
            get { return "结束"; }
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryEndCtrl obj = new StoryEndCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time= time;
            return obj;
        }
        public override void Execute()
        {
        }
    }
}