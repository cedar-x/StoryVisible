using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR 
    using UnityEditor;
#endif


/// <summary>
/// 设计目的：时间管理基类-用于事件的调度
/// 设计时间：2015-08-03
/// 作者：
/// </summary>
/// 
namespace xxstory
{
    public class ILuaState
    {

    }
    public class StoryBaseCtrl
    {

        private static Camera _gameCamera;

        protected float startTime = float.MaxValue;
        public float time = 0f;
        public bool bWait = false;
        public bool bClick = false;
        protected List<string> expList;
        public StoryShotCtrl _shotCtrl;
        protected bool _isPlaying = false;
        //////////////////虚函数////////////////////////
        public StoryBaseCtrl()
        {
            expList = new List<string>();
            bWait = false;
            bClick = false;
            initInfo();
        }
        public void onStart()
        {
            startTime = Time.time;
            _isPlaying = true;
        }
        public void onEnd()
        {
            startTime = float.MaxValue;
            _isPlaying = false;
        }
        public virtual void OnForceNext()
        {
            //Debug.Log("StoryBase:OnForceNext:" + luaName + ":" + isPlaying);
            if (_isPlaying == false) return;

            onEnd();
            Stop();
        }
        public virtual void initInfo()
        {
            expList.Clear();
            expList.Add("bWait");
            expList.Add("time");
            expList.Add("event");
            expList.Add("bClick");
        }
        /// //////////////////路由lua中对应功能的类名称-导出lua生成类实例或者标识类名
        public virtual string luaName
        {
            get
            {
                return "StoryEventCtrl";
            }
        }
        /// //////////////////用于可视化调节窗口名字显示-OnGUI窗口标识事件名称，易于理解
        public virtual string ctrlName
        {
            get
            {
                return "baseCtrl";
            }
        }
        public virtual int index
        {
            get
            {
                return _shotCtrl.indexOf(this);
            }
        }
        public virtual bool isPlaying
        {
            get
            {
                return _isPlaying;
            }
        }
        public virtual void Execute()
        {
        }
        public virtual void Update()
        {
            if (_isPlaying == true && (Time.time - startTime > time))
            {
                onEnd();
                OnFinish();
            }
        }
        public virtual void OnFinish()
        {
        }
        public virtual void Stop()
        {
            this.OnFinish();
        }
        public virtual void ModInfo()
        {

        }
        public virtual void SavePoint()
        {

        }
        public virtual void ResetPoint(bool bRealInfo)
        {

        }
        public virtual StoryBaseCtrl CopySelf()
        {
            StoryBaseCtrl obj = new StoryBaseCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            return obj;
        }
        /// //////////////////属性导出导入与访问相关///////////////////////////////////////////////
        protected virtual bool WidgetReadOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "bWait":
                    break;
                case "event":
                    break;
                case "time":
                    break;
                case "bClick":
                    break;
                default:
                    return false;
            }
            return true;
        }
        protected virtual bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "event":
                    break;
                case "time":
                    break;
                case "bWait":
                    break;
                case "bClick":
                    break;
                default:
                    return false;
            }
            return true;
        }
        public virtual void ExportProperty(ILuaState lua, int index)
        {
            bool bSet = false;
            foreach (string key in expList)
            {
                bSet = WidgetReadOper(lua, key);
                if (bSet == true)
                {
                    //lua.SetField(-2, key);
                }
            }
        }
        public virtual void ImportProperty(ILuaState lua, int index)
        {
//             bool bFlag = false;
//             lua.PushValue(index);
//             lua.PushNil();
//             while (lua.Next(-2))
//             {
//                 string key = lua.L_CheckString(-2);
//                 bFlag = WidgetWriteOper(lua, key);
//                 if (bFlag == false)
//                     Debug.LogWarning(luaName + " can't write key:" + key + " please check....");
//                 lua.Pop(1);
//             }
//             lua.Pop(1);
        }
#if UNITY_EDITOR
        public virtual void OnParamGUI()
        {
            time = EditorGUILayout.FloatField("time", time);
            bWait = GUILayout.Toggle(bWait, "bWait");
            //bClick = GUILayout.Toggle(bClick, "bClick");
        }
#endif
        public static Camera objMainCamera
        {
            get
            {
                return _gameCamera;
            }
        }
        public static storyUI objStoryUI
        {
            get
            {
                return LuaAnimEvent.AnimStoryUI;
            }
        }
    }
}
