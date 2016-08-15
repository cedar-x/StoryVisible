using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hstj;
using UniLua;

namespace xxstory
{
    //人物事件管理-即所有人物的统一管理
    public class StoryBoardCtrl
    {
        private List<StoryShotCtrl> _shots = new List<StoryShotCtrl>();
        private float _time;

        public bool bFolderOut = true;
        //----------
        public float time
        {
            get
            {
                return _time;
            }
            set
            { 
                _time = value;
            }
        }
        public float length
        {
            get
            {
                return _time;
            }
        }
        public int Count
        {
            get
            {
                return _shots.Count;
            }
        }
        public StoryShotCtrl this[int index]
        {
            get
            {
                if (index < 0)
                    Debug.LogError("StoryBoardCtrl Index can't be minus");
                if (index >= Count)
                    Debug.LogError("StoryBoardCtrl Index out of range");
                return _shots[index];
            }
        }
        public int indexOf(StoryShotCtrl shotCtrl)
        {
            return _shots.IndexOf(shotCtrl);
        }
        //增加特定人物时间事件
        public void Add(StoryShotCtrl lensCtrl)
        {
            _shots.Add(lensCtrl);
        }
        public bool Delete(int index)
        {
            if (index < 0 || index > Count)
            {
                Debug.LogError("StoryBoardCtrl Delete Index failed:"+index);
                return false;
            }
            _shots.RemoveAt(index);
            return true;
        }
        public bool Delete(StoryShotCtrl shotCtrl)
        {
            if (!_shots.Contains(shotCtrl))
            {
                Debug.LogError("StoryBoardCtrl Delete StoryShotCtrl failed:"+shotCtrl.actorName);
                return false;
            }
            _shots.Remove(shotCtrl);
            return true;
        }
        public void Clear()
        {
            _shots.Clear();
        }

        public void Execute()
        {
            for (int i = 0; i < Count; ++i)
            {
                StoryShotCtrl shotCtrl = _shots[i];
                shotCtrl.Execute();
            }
        }
        public void Stop()
        {
        }
        //强制使用下一步跳过剧情时，遗留事件处理
        public void OnForceNext()
        {
            Debug.Log("OnForceNext:------------");
            for (int i = 0; i < Count; ++i)
            {
                StoryShotCtrl shotCtrl = _shots[i];
                shotCtrl.OnForceNext();
            }
        }
        public void Update()
        {
            foreach (StoryShotCtrl shotCtrl in _shots)
            {
                shotCtrl.Update();
            }
        }
        public void CalculateTime()
        {
            float ttime = 0f;
            foreach (StoryShotCtrl shotCtrl in _shots)
            {
                if (shotCtrl.time > ttime)
                {
                    ttime = shotCtrl.time;
                }
                shotCtrl.CalculateTime();
            }
            _time = ttime;
        }
        public void ImportProperty(ILuaState lua, int index)
        {
            lua.PushValue(index);
            lua.PushNil();
            while(lua.Next(-2))
            {
                int key = lua.L_CheckInteger(-2);
                StoryShotCtrl shotctrl = _shots[key-1];
                shotctrl.ImportProperty(lua, -1);
                lua.Pop(1);
            }
            lua.Pop(1);
        }
        public string ExportProperty(string[] strProperty)
        {
            string Result = "";
            for (int i = 0; i < Count; ++i)
            {
                StoryShotCtrl shot = _shots[i];
                if (shot.Count > 0)
                {
                    Result += "[" + shot.actor.nameIndex + "] = " + shot.ExportProperty(null) + ";";
                }
            }
            return Result;
        }
    }
}

