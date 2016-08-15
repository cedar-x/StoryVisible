using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--画面去色--事件的调度
/// 设计时间：2015-08-18
/// </summary>

namespace xxstory {

    public class StoryGrayscaleCtrl : StoryBaseCtrl {

        /// ////////////////////////////////////////////////////////
        private struct paramInfo
        {
            public bool bTransition;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;
        private float yuanSaturation;
        public override string luaName
        {
            get { return "StoryGrayscaleCtrl"; }
        }
        public override string ctrlName
        {  
            get { return "画面去色"; }
        }
        public override void initInfo()
        {
            base.initInfo();
            expList.Add("bTransition");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryGrayscaleCtrl obj = new StoryGrayscaleCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            ColorCorrectionCurves colorCurves = objMainCamera.gameObject.GetComponent<ColorCorrectionCurves>();
            yuanSaturation = colorCurves.saturation;
            Debug.Log("GrayscaleCtrl:" + yuanSaturation + ":" + _realInfo.bTransition);
            if (_realInfo.bTransition == false)
                colorCurves.saturation = 0;
        }
        public override void OnFinish()
        {
            Debug.Log("StoryGaryScaleCtrl:OnFinish:" + yuanSaturation);
            objMainCamera.gameObject.GetComponent<ColorCorrectionCurves>().saturation = yuanSaturation;
        }
        public override void Update()
        {
            if (Time.time - startTime > time)
            {
                onEnd();
                OnFinish();
                yuanSaturation = 0;
            }
            else if (_realInfo.bTransition == true && yuanSaturation !=0)
            {
                objMainCamera.GetComponent<ColorCorrectionCurves>().saturation = Mathf.Lerp(yuanSaturation, 0, (Time.time - startTime) / time);
            }
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
                case "bTransition":
                    _normalInfo.bTransition = lua.ToBoolean(-1);
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
                case "bTransition":
                    lua.PushBoolean(_realInfo.bTransition);
                    break;
                default:
                    return base.WidgetReadOper(lua, key);
            }
            return true;
        }
#if UNITY_EDITOR
        public override void OnParamGUI(){
            _normalInfo.bTransition = GUILayout.Toggle(_normalInfo.bTransition, "bTranstion");
            base.OnParamGUI();
        }
#endif
    
    }
}