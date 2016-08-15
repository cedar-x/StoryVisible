using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;


/// <summary>
/// 设计目的：用于--时间等待--事件的调度
/// 设计时间：2015-08-03
/// </summary>

namespace xxstory
{
    public class StoryTimeCtrl : StoryBaseCtrl
    {
        public override void initInfo()
        {
            bWait = true;
            base.initInfo();
        }
        public override string luaName
        {
            get
            {
                return "StoryTimeCtrl";
            }
        }
        public override string ctrlName
        {
            get
            {
                return "等待";
            }
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryTimeCtrl obj = new StoryTimeCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            return obj;
        }
#if UNITY_EDITOR 
        public override void OnParamGUI()
        {
            time = EditorGUILayout.FloatField("time", time);
            GUILayout.Toggle(true, "bWait");
        }
#endif
    }
}
