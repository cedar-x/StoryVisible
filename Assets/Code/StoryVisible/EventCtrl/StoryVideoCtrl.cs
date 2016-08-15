using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--视频类--事件的调度
/// 设计时间：2016-06-29
/// </summary>

namespace xxstory
{
    public class StoryVideoCtrl : StoryBaseCtrl
    {
        private struct paramInfo
        {
            public string videoPath;
            public float speed;
            public int controlMode;
            public int scalingMode;
        }
        private paramInfo _realInfo;
        private paramInfo _saveInfo;
        private paramInfo _normalInfo;

        //事件相关属性
        private LuaMeshImage _meshImage;
#if (UNITY_ANDROID || UNITY_IPHONE)
#else
        private MovieTexture movTexture;
#endif
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryVideoCtrl"; }
        }
        public override string ctrlName
        {
            get { return "视频"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("videoPath");
            expList.Add("speed");
            expList.Add("controlMode");
            expList.Add("scalingMode");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryVideoCtrl obj = new StoryVideoCtrl();
            obj.time = time;
            obj.bWait = bWait;
            obj.bClick = bClick;
            //////本类事件属性赋值
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            Debug.Log("videoPath:" + _realInfo.videoPath + ":" + _realInfo.controlMode + ":" + _realInfo.scalingMode);
            
#if (UNITY_ANDROID || UNITY_IPHONE)
            if (_realInfo.scalingMode == 0)
            {
                Handheld.PlayFullScreenMovie(_realInfo.videoPath, Color.black, (FullScreenMovieControlMode)_realInfo.controlMode);
            }
            else
            {
                Handheld.PlayFullScreenMovie(_realInfo.videoPath, Color.black, (FullScreenMovieControlMode)_realInfo.controlMode, (FullScreenMovieScalingMode)_realInfo.scalingMode);
            }
#else
            movTexture = Resources.Load(_realInfo.videoPath) as MovieTexture;
            _meshImage = objMainCamera.gameObject.GetComponentInChildren<LuaMeshImage>();
            _meshImage.transform.localPosition = new Vector3(0f, 0f, 1);
            _meshImage.init("", objMainCamera.fieldOfView);
            _meshImage.SetTexture(movTexture);
            movTexture.Play();
#endif
        }
        public override void OnFinish()
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
#else
            _meshImage.Clear();
#endif
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
                case "videoPath":
                    _normalInfo.videoPath = lua.L_CheckString(-1);
                    break;
                case "speed":
                    _normalInfo.speed = (float)lua.L_CheckNumber(-1);
                    break;
                case "controlMode":
                    _normalInfo.controlMode = lua.L_CheckInteger(-1);
                    break;
                case "scalingMode":
                    _normalInfo.scalingMode = lua.L_CheckInteger(-1);
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
                case "videoPath":
                    lua.PushString(_realInfo.videoPath);
                    break;
                case "speed":
                    lua.PushNumber(_realInfo.speed);
                    break;
                case "controlMode":
                    lua.PushInteger(_realInfo.controlMode);
                    break;
                case "scalingMode":
                    lua.PushInteger(_realInfo.scalingMode);
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
            _normalInfo.videoPath = EditorGUILayout.TextField("videoPath", _normalInfo.videoPath);
            _normalInfo.speed = EditorGUILayout.FloatField("Speed", _normalInfo.speed);
            _normalInfo.controlMode = EditorGUILayout.IntField("controlMode", _normalInfo.controlMode);
            _normalInfo.scalingMode = EditorGUILayout.IntField("scalingMode", _normalInfo.scalingMode);
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////

    }
}