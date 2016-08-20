using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace xxstory
{
    //镜头人物事件管理
    public class StoryShotCtrl
    {
        //
        private List<StoryBaseCtrl> _listCtrl = new List<StoryBaseCtrl>();
        private GameObject target;
        private storyActorInfo _actorInfo;
        private int _ctrlIndex = 0;
        private float _time;
        private bool isPlaying = false;
        private float startTime;
        public StoryBaseCtrl _objEditorEventCtrl;
        //
        public bool bFloderOut;
        //
        public string actorName
        {
            get
            {
                return _actorInfo.target.name;
            }
        }
        public int actorIndex
        {
            get
            {
                return _actorInfo.nameIndex;
            }
        }
        public storyActorInfo actor
        {
            get
            {
                return _actorInfo;
            }
            set
            {
                _actorInfo = value;
                target = _actorInfo.target;
            }
        }
        public int Count
        {
            get
            {
                return _listCtrl.Count;
            }
        }
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
        public StoryBaseCtrl this[int index]
        {
            get
            {
                if (index < 0)
                    Debug.LogError("StoryShotCtrl Index can't be minus");
                if (index >= Count)
                    Debug.LogError("StoryShotCtrl Index out of range");
                return _listCtrl[index];
            }
        }
        public int indexOf(StoryBaseCtrl baseCtrl)
        {
            return _listCtrl.IndexOf(baseCtrl);
        }
        public void Replace(StoryBaseCtrl baseCtrl, int dwIndex)
        {
            int yuanIndex = indexOf(baseCtrl);
            if (yuanIndex == -1)
            {
                Debug.Log("StoryShotCtrl can't replace StorybaseCtrl:"+baseCtrl.ctrlName);
                return;
            }
            if (dwIndex < 0 || dwIndex >= Count)
            {
                Debug.LogError("StoryShotCtrl Index min zero or out of range:"+dwIndex);
                return;
            }
            var temp = _listCtrl[yuanIndex];
            _listCtrl[yuanIndex] = _listCtrl[dwIndex];
            _listCtrl[dwIndex] = temp; 
        }
        public void Add(StoryBaseCtrl bsCtrl, int index = -1)
        {
            //Debug.Log("StoryShotCtrl Add StoryBaseCtrl:" + bsCtrl.luaName + ":" + _listCtrl.Count);
            if (index == -1)
            {
                bsCtrl._shotCtrl = this;
                _listCtrl.Add(bsCtrl);
            }
            else
            {
                if (index < 0 || index > _listCtrl.Count)
                {
                    Debug.LogWarning("Add StoryBaseCtrl index is out of range:count=" + Count + " index=" + index);
                    return;
                }
                bsCtrl._shotCtrl = this;
                _listCtrl.Insert(index, bsCtrl);
            }
        }
        public void paste(StoryShotCtrl shotCtrl)
        {
            if (shotCtrl == null)return;
            for (int i = 0; i < shotCtrl.Count; ++i)
            {
                StoryBaseCtrl bsCtrl = shotCtrl[i].CopySelf();
                bsCtrl.ModInfo();
                _listCtrl.Add(bsCtrl);
            }
        }
        public bool Delete(int index)
        {
            if (index < 0 || index > Count)
            {
                Debug.LogError("StoryShotCtrl Delete Index failed:" + index);
                return false;
            }
            _listCtrl.RemoveAt(index);
            return true;
        }
        public void Clear()
        {
            _listCtrl.Clear();
        }
        public bool Delete(StoryBaseCtrl baseCtrl)
        {
            if (!_listCtrl.Contains(baseCtrl))
            {
                Debug.LogError("StoryShotCtrl Delete StoryShotCtrl failed:" + baseCtrl.luaName);
                return false;
            }
            _listCtrl.Remove(baseCtrl);
            return true;
        }
        public void Execute()
        {
            while (_ctrlIndex < Count)
            {
                startTime = Time.time;
                isPlaying = true;
                StoryBaseCtrl baseCtrl = _listCtrl[_ctrlIndex];
                //Debug.Log("=====StoryShotCtrl:Execute:" + _ctrlIndex+":"+_objCurCtrl.bWait+":"+_objCurCtrl.time);
                baseCtrl.onStart();
                baseCtrl.Execute();
                if (baseCtrl.bWait == true) return;
                ++_ctrlIndex;
            }
            Stop();
        }
        public void Stop()
        {
            //Debug.Log("==StoryShotCtrl:Stop" + _actorInfo.target.name);
            _ctrlIndex =0;
            isPlaying = false;
        }
        public void OnForceNext()
        {
            for (int i = 0; i < Count; ++i)
            {
                StoryBaseCtrl baseCtrl = _listCtrl[i];
                baseCtrl.OnForceNext();
            }
        }
        public void Execute(int index)
        {
            if (index < 0 || index >= Count)
            {
                Debug.LogError("StoryShotCtrl Execute Index expection:"+index+"("+Count+")");
                return;
            }
            _listCtrl[index].onStart();
            _listCtrl[index].Execute();
        }
        public void CalculateTime()
        {
            float ttime = 0f;
            foreach (StoryBaseCtrl baseCtrl in _listCtrl)
            {
                if (baseCtrl.bWait == true)
                {
                    ttime += baseCtrl.time;
                }
            }
            _time = ttime;
        }
        public void Update()
        {
            foreach(StoryBaseCtrl baseCtrl in _listCtrl)
            {
                baseCtrl.Update();
            }
            if (isPlaying == false || _listCtrl[_ctrlIndex].bWait == false) return;
            if (Time.time - startTime > _listCtrl[_ctrlIndex].time)
            {
                ++_ctrlIndex;
                Execute();
            }
        }
        public void OnFinish()
        {

        }
        public static StoryBaseCtrl InstanceEventCtrl(string key)
        {
            switch (key)
            {
                case "StoryPositionCtrl":
                    return new StoryPositionCtrl();
                case "StoryTalkCtrl":
                    return new StoryTalkCtrl();
                case "StoryTimeCtrl":
                    return new StoryTimeCtrl();
            }
            Debug.Log("there is no EventCtrl:" + key);
            return null;
        }
        public void ImportProperty(ILuaState lua, int dwIndex)
        {
//             _listCtrl.Clear();
//             lua.PushValue(dwIndex);
//             int len = lua.L_Len(-1);
//             for (int i = 1; i <= len; i++)
//             {
//                 lua.PushNumber(i);
//                 lua.GetTable(-2);
//                 lua.PushString("event");
//                 lua.GetTable(-2);
//                 string eventName = lua.L_CheckString(-1);
//                 lua.Pop(1);
//                 StoryBaseCtrl objCtrl = InstanceEventCtrl(eventName);
//                 if (objCtrl != null)
//                 {
//                     objCtrl.ImportProperty(lua, -1);
//                     objCtrl.ModInfo();
//                 }
//                 else
//                 {
//                     Debug.LogWarning("InstanceEventCtrl objCtrl is null " + eventName);
//                 }
//                 lua.Pop(1);
//                 Add(objCtrl);
//             }
//             lua.Pop(1);
        }
        public string ExportProperty(string[] strProperty)
        {
//             ILuaState _fileLua = Game.LuaApi;
//             _fileLua.NewTable();
//             for (int i = 0; i < _listCtrl.Count; i++)
//             {
//                 _fileLua.PushNumber(i + 1);
//                 StoryBaseCtrl objCtrl = _listCtrl[i];
//                 objCtrl.ExportProperty(_fileLua, -1);
//                 _fileLua.SetTable(-3);
//             }
//             string strResult = SerializeTable(_fileLua, -1);
//             _fileLua.Pop(1);
//             return strResul   
            return "";
        }
        /// <summary>
        /// 将Table序列化成为字符串写入文件
        /// </summary>
        public string SerializeTable(ILuaState _fileLua, int index)
        {
//             //获取LibCore中_SerializeTable函数，然后串行化table， 以后要重写，从而不依赖LibCore（因为有在非运行状态下获取内容）
//             if (_fileLua.Type(index) != LuaType.LUA_TTABLE)
//             {
//                 Debug.LogWarning("LuaExport:SerializeTable param must a table.. please check");
//                 return "";
//             }
//             int dwStackIndex = _fileLua.GetTop();
//             _fileLua.PushValue(index);
//             _fileLua.GetGlobal("_SerializeTable");
//             _fileLua.Insert(-2);
//             _fileLua.PushBoolean(false);
//             if (_fileLua.PCall(2, 1, 0) != 0)
//             {
//                 Debug.LogWarning(_fileLua.ToString(-1) + " in SerializeTable");
//                 _fileLua.Pop(-1);
//                 return "";
//             }
//             string szResult = _fileLua.ToString(-1);
//             //Debug.Log(m_szResult);
//             _fileLua.Pop(1);
//             if (dwStackIndex != _fileLua.GetTop())
//                 Debug.LogWarning("LuaExport:SerializeTable stack Exception:start=" + dwStackIndex + " end=" + _fileLua.GetTop());
//             return szResult;
            return "";
        }
    }
}


