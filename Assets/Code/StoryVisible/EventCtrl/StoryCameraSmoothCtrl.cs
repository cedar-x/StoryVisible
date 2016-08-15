using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UniLua;
using Hstj;

/// <summary>
/// 设计目的：用于--Camera缓动--事件的调度
/// 设计时间：2015-08-13
/// </summary>

namespace xxstory
{
    public class StoryCameraSmoothCtrl : StoryBaseCtrl
    {
        private struct paramInfo 
        {
            public normalInfo norInfo;
            public string pathName;
            public int step;
            public iTween.EaseType easetype;
            public int type;
        }
        private paramInfo _saveInfo;
        private paramInfo _realInfo;
        private paramInfo _normalInfo;

        public LuaPathCamera _pathTarget;
        /// ////////////////功能重写部分-必须实现部分/////////////////////////////////////////////
        public override string luaName
        {
            get { return "StoryCameraSmoothCtrl"; }
        }
        public override string ctrlName
        {
            get { return "Camera缓动"; }
        }
        public override void initInfo()
        {
            _normalInfo.easetype = iTween.EaseType.linear;
            base.initInfo();
            expList.Add("cameraParam");
            expList.Add("step");
            expList.Add("easetype");
            expList.Add("type");
            expList.Add("pathParam");
        }
        public override StoryBaseCtrl CopySelf()
        {
            StoryCameraSmoothCtrl obj = new StoryCameraSmoothCtrl();
            obj.bWait = bWait;
            obj.bClick = bClick;
            obj.time = time;
            obj._normalInfo = _normalInfo;
            return obj;
        }
        public override void Execute()
        {
            
            OnCameraSmooth(_realInfo);
        }
        public override void OnFinish()
        {
            if (_realInfo.type == 0)
                objMainCamera.onTweenStop -= OnFinish;
            objMainCamera.transform.localEulerAngles = Vector3.zero;
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
            //恢复视角
            objMainCamera.LRAnge -= _normalInfo.norInfo.rotationLR;
            objMainCamera.distance -= _normalInfo.norInfo.distance;
            objMainCamera.UDAngle -= _normalInfo.norInfo.rotationUD;
            objMainCamera.calculatePos(false);
        }
        protected override bool WidgetWriteOper(ILuaState lua, string key)
        {
            switch (key)
            {
                case "cameraParam":
                    _normalInfo.norInfo = LuaExport.GetNormalInfo(lua, -1);
                    break;
                case "step":
                    _normalInfo.step = lua.L_CheckInteger(-1);
                    break;
                case "easetype":
                    _normalInfo.easetype = (iTween.EaseType)lua.L_CheckInteger(-1);
                    break;
                case "type":
                    _normalInfo.type = lua.L_CheckInteger(-1);
                    break;
                case "pathParam":
//                     if (_pathTarget == null)
//                     {
//                         GameObject obj = new GameObject("New Story Path");
//                         _pathTarget = obj.AddComponent<LuaPathCamera>();
//                         _pathTarget.Init();
//                     }
//                     _pathTarget.ImportProperty(-1);
                    _normalInfo.pathName = lua.L_CheckString(-1);
                    _pathTarget = GameObject.Find("StoryPath_juqing").transform.FindChild(_normalInfo.pathName).GetComponent<LuaPathCamera>();
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
                case "cameraParam":
                    LuaExport.NormalInfoToStack(lua, _realInfo.norInfo);
                    break;
                case "step":
                    lua.PushInteger(_realInfo.step);
                    break;
                case "easetype":
                    lua.PushInteger((int)_realInfo.easetype);
                    break;
                case "type":
                    lua.PushInteger(_realInfo.type);
                    break;
                case "pathParam":
                    if (_realInfo.type == 0)
                        return false;
                    if (_pathTarget == null)
                    {
                        Debug.LogWarning("StoryCameraSmoothCtrl don't set Story Path Target.....");
                        return false;
                    }
                    //_pathTarget.ExportProperty(null);
                    lua.PushString(_pathTarget.name);
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
            _normalInfo.type = EditorGUILayout.IntField("dwType", _normalInfo.type);
            if (_normalInfo.type == 0)
            {
                StoryBaseCtrl.OnCameraInfoGUI(ref _normalInfo.norInfo);
                _normalInfo.easetype = (iTween.EaseType)EditorGUILayout.EnumPopup("easetype", _normalInfo.easetype);
                _normalInfo.step = EditorGUILayout.IntField("step", _normalInfo.step);
                if (GUILayout.Button("Smooth"))
                {
                    _saveInfo.norInfo = _normalInfo.norInfo;
                    _saveInfo.step = _normalInfo.step;
                    OnCameraSmooth(_saveInfo);
                }
            }
            else
            {
                _pathTarget = EditorGUILayout.ObjectField(_pathTarget, typeof(LuaPathCamera)) as LuaPathCamera;
            }
            base.OnParamGUI();
        }
#endif
        /// /////////////////////////////// 功能函数 ///////////////////////////////////////////////////////////////
        private void OnCameraSmooth(paramInfo pInfo)
        {
            if (pInfo.type == 0)
            {
                objMainCamera.ClearParam();
                objMainCamera.AddParam("time", time);
                objMainCamera.AddParam("oncomplete", "oncomplete");
                objMainCamera.AddParam("oncompleteparams", new LuaObjWithCallFun());
                objMainCamera.AddParam("easetype", pInfo.easetype);
                //EventDelegate.Add(objMainCamera.onTweenStop, OnFinish);
                objMainCamera.onTweenStop += OnFinish;
                if (pInfo.step > 1)
                {
                    float lrRange = (pInfo.norInfo.rotationLR) / pInfo.step;
                    float udRange = (pInfo.norInfo.rotationUD) / pInfo.step;
                    Vector3[] path = new Vector3[pInfo.step];
                    for (int i = 1; i <= pInfo.step; i++)
                    {
                        float lr = objMainCamera.LRAnge + i * lrRange;
                        float ud = objMainCamera.UDAngle + i * udRange;
                        path[i - 1] = objMainCamera.virtualPos(objMainCamera.distance, lr, ud);
                    }
                    objMainCamera.AddParam("path", path);
                }
                objMainCamera.distance += pInfo.norInfo.distance;
                objMainCamera.UDAngle += pInfo.norInfo.rotationUD;
                objMainCamera.LRAnge += pInfo.norInfo.rotationLR;
                objMainCamera.calculatePos(true);
            }
            else if(pInfo.type == 1)
            {
                objMainCamera.StopTween();
                _pathTarget.Stop();
                _pathTarget.SetAnimTarget(objMainCamera.transform);
                _pathTarget.Play();
            }
            else if (pInfo.type == 2)
            {
                objMainCamera.target = null;
                objMainCamera.StopTween();
                objMainCamera.transform.localPosition = Vector3.zero;
                objMainCamera.transform.localEulerAngles = Vector3.zero;
                _pathTarget.Stop();
                _pathTarget.SetAnimTarget(objMainCamera.proxyParent);
                _pathTarget.Play();
            }
            else if (pInfo.type == 3)
            {
                _pathTarget.Stop();
                _pathTarget.SetAnimTarget(_shotCtrl.actor.target.transform);
                _pathTarget.Play();
            }
        }
    }
}