using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UniLua;
using xxstory;
#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// 设计目的：剧情时间轴实例、可以挂载各种事件
/// 设计时间：2015-08-03
/// 作者：
/// </summary>

namespace Hstj
{
    public class storyActorInfo
    {
        public GameObject target;
        public int type;
        public int dwModelId;
        public int nameIndex;
        public string skeleton;
        public string name;
        public bool bFolderOut;
    }
    public class storyBasicInfo
    {
        public string szStageName;
        public float dwNextTime = 5f;
        public string szPathName;
        public bool bNewSceneState = true;
        public bool bNewLightState = true;
        public int dwType;
        public string szSceneName;

        public string OnBeforeInitStory;
        public string OnAfterInitStory;
        public string OnBeforeEndStory;

        public storyBasicInfo()
        {
            this.OnBeforeEndStory = "";
            this.OnAfterInitStory = "";
            this.OnBeforeInitStory = "";
        }
    }
    public class storyUI
    {
        public UILabel talkName;
        public UILabel talkInfo;
        public GameObject backGround;
        public GameObject shangBack;
        public GameObject xiaBack;
        public GameObject btnClose;
        public UIButton btnNext;
    }
        

    public class LuaAnimEvent : LuaExport
    {
        //时间周期管理-即分镜管理
        public List<StoryBoardCtrl> _storyBoard;
        //人物对象管理
        public List<storyActorInfo> _storyActor;
        public storyBasicInfo _storyBasicInfo;
        //控制数据
        private bool mbInitMember = false;

        //Editor数据
        public StoryShotCtrl objEditorShotCtrl;
        public StoryShotCtrl objEditorCopyCtrl;

        private float _totalTime = 0f;
        private float startTime = float.MaxValue;
        private float durtime = 0f;
        private int _boardIndex = 0;
        private bool isPlaying = false;
        //UI相关调节
        private static storyUI _storyUI = null;

        public static storyUI AnimStoryUI
        {
            get
            {
                return _storyUI;
            }
        }

        public static void InitStoryUI()
        {
            if (_storyUI != null) return;
            ILuaState lua = Game.LuaApi;
            lua.GetGlobal("StoryInitUI");
            if (lua.PCall(0, 0, 0) != 0)
            {
                Debug.LogWarning(lua.ToString(-1));
                lua.Pop(-1);
            }  
            if (_storyUI == null)
            {
                _storyUI = new storyUI();
                _storyUI.talkName = GameObject.Find("szStoryName").GetComponent<UILabel>();
                _storyUI.talkInfo = GameObject.Find("szStoryTalkInfo").GetComponent<UILabel>();
                _storyUI.backGround = GameObject.Find("storyBkGround");
                _storyUI.btnNext = GameObject.Find("btnStoryNext").GetComponent<UIButton>();
                _storyUI.btnClose = GameObject.Find("btnStoryClose");
                _storyUI.shangBack= GameObject.Find("bkStoryShangBk");
                _storyUI.xiaBack = GameObject.Find("bkStoryXiaBk");
            }
            lua.GetGlobal("StoryResetUI");
            if (lua.PCall(0, 0, 0) != 0)
            {
                Debug.LogWarning(lua.ToString(-1));
                lua.Pop(-1);
            }  
        }

        private void onBtnAnimNext()
        {
            Debug.Log("====onBtnAnimNext:" + _boardIndex);
            OnBtnNextState(false);
            //结束上一个StoryBoard镜头事件
            if (OnNextStep())
            {
                StoryBoardCtrl board = _storyBoard[_boardIndex-1];
                if (board!=null)
                    board.OnForceNext();
                Execute();
            }
        }

        public static void OnBtnNextState(bool bVisible)
        {
            AnimStoryUI.btnNext.gameObject.SetActive(bVisible);
        }

        /// /////////////////////////////////////////////////////////////
        void Start()
        {
            //InitMemeber();
        }

        public void InitMemeber()
        {
            _storyActor = new List<storyActorInfo>();
            _storyBoard = new List<StoryBoardCtrl>();
            _storyBasicInfo = new storyBasicInfo();
            AddEmpty();
            mbInitMember = true;
            InitStoryUI();
            EventDelegate.Add(AnimStoryUI.btnNext.onClick, onBtnAnimNext);
        }

        public float totalTime
        {
            get
            {
                float num = 0;
                int i = 0;
                while (i < _storyBoard.Count)
                {
                    num += _storyBoard[i].time;
                    i++;
                }
                _totalTime = num;
                return _totalTime;
            }
            set
            {
                _totalTime = value;
            }
        }

        public int actorCount
        {
            get
            {
                return _storyActor.Count;
            }
        }

        public int Count
        {
            get
            {
                return _storyBoard.Count;
            }
        }

        public Scene storyScene
        {
            get
            {
                return Game.Instance.GetStoryScene();
            }
        }

        public bool bInitMember
        {
            get
            {
                return mbInitMember;
            }
        }

        public StoryBoardCtrl this[int index]
        {
            get
            {
                if (index < 0)
                        Debug.LogError("LuaAnimEvent Index can't be minus");
                if (index >= Count)
                    Debug.LogError("LuaAnimEvent Index out of range");
                return _storyBoard[index];
            }
        }

       //是否已经有相同模型
        public bool bSingleModelID(int dwModwlID)
        {
            foreach(storyActorInfo actor in _storyActor)
            {
                if (actor.dwModelId == dwModwlID)
                    return false;
            }
            return true;
        }

        //增加时间周期
        public void AddBoard(StoryBoardCtrl boardCtrl, int index = 0)
        {
            if (index == 0)
            {
                _storyBoard.Add(boardCtrl);
            }
            else
            {
                if (index <= 0 || index > Count)
                {
                    Debug.LogWarning("Add StoryBaseCtrl index is out of range:count=" + Count + " index=" + index);
                    return;
                }
                _storyBoard.Insert(index-1, boardCtrl);
            }
            
        }

        public bool DeleteBoard(StoryBoardCtrl boardCtrl)
        {
            if (!_storyBoard.Contains(boardCtrl))
            {
                Debug.LogWarning("LuaAnimEvent don't contain StoryBoardCtrl:");
                return false;
            }
            _storyBoard.Remove(boardCtrl);
            return true;
        }

        //增加角色对象
        public void AddActor(int dwModelID,int nameIndex, int dwType = 0, string gameName = "")
        {
            string actorName = "Actor" + nameIndex;
            int dwID = actorName.GetHashCode();
            //ResManager.UseActorConfig("story");
            //Actor objActor = Game.Instance.GetStoryScene().CreateActor("M_BaiYinJiaBing", dwID);
            //ResManager.CreateActor("M_BaiYinJiaBing", objActor);
            //ResManager.UseActorConfig("default");
            ILuaState lua = Game.LuaApi;
            lua.GetGlobal("StoryCreateActor");
            lua.PushInteger(dwID);
            lua.PushInteger(dwModelID);
            if (lua.PCall(2, 2, 0) != 0)
            {
                Debug.LogWarning(lua.ToString(-1));
                lua.Pop(-1);
                return;
            }
            Actor objActor = GetLuaObject(lua, -2) as Actor;
            string skeleton = lua.L_CheckString(-1);
            if (objActor == null)
            {
                return;
            }
            objActor.SetLayer(9);
            storyActorInfo clsActor = new storyActorInfo();
            clsActor.skeleton = skeleton;
            clsActor.target = objActor.gameObject;
            clsActor.dwModelId = dwModelID;
            clsActor.nameIndex = nameIndex;
            clsActor.name = gameName;
            clsActor.type = dwType;
            clsActor.bFolderOut = false;
            //InitActor(ref clsActor);
            objActor.name = actorName;
            _storyActor.Add(clsActor);
            lua.Pop(2);
        }

        public void InitActor(ref storyActorInfo storyActor)
        {
            int dwModelId = storyActor.dwModelId;
            Actor actor = storyActor.target.GetComponent<Actor>();
            if (dwModelId == 1)
            {
                //actor.SetMeshPart("head", "Actor/ZJ_jianxia/ZJ_jianxia_07");
                actor.SetMeshPart("body", "Actor/ZJ_jianxia/ZJ_jianxia_07");
                actor.SetMeshPart("weapon", "Actor/ZJ_jianxia/ZJ_jianxia_wq_07");
                storyActor.type = 1;
                actor.GetAnimator().cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
            else if (dwModelId == 2)
            {
                //actor.SetMeshPart("head", "Actor/ZJ_jingang/ZJ_jingang_07");
                actor.SetMeshPart("body", "Actor/ZJ_jingang/ZJ_jingang_07");
                actor.SetMeshPart("weapon", "Actor/ZJ_jingang/ZJ_jingang_wq_07");
                storyActor.type = 1;
                actor.GetAnimator().cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
            else if (dwModelId == 3)
            {
                //actor.SetMeshPart("head", "Actor/ZJ_lingnv/ZJ_lingnv_07");
                actor.SetMeshPart("body", "Actor/ZJ_lingnv/ZJ_lingnv_07");
                storyActor.type = 1;
                actor.GetAnimator().cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
            else if (dwModelId == 4)
            {
                //actor.SetMeshPart("head", "Actor/ZJ_luocha/ZJ_luocha_07");
                actor.SetMeshPart("body", "Actor/ZJ_luocha/ZJ_luocha_07");
                actor.SetMeshPart("weapon", "Actor/ZJ_luocha/ZJ_luocha_wq_07");
                storyActor.type = 1;
                actor.GetAnimator().cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            actor.Visible = true;


        }

        public void DeleteActor(storyActorInfo actor)
        {
            if (!_storyActor.Contains(actor))
            {
                Debug.LogWarning("can't find actor in _storyActor list..");
                return;
            }
            GameObject.Destroy(actor.target);
            _storyActor.Remove(actor);
        }

        //增加空对象
        public storyActorInfo AddEmpty()
        {
            string name = "Actor1";
            int dwID = name.GetHashCode();
            Entity entity = Game.Instance.GetStoryScene().CreateEmptyObject(dwID);
            entity.name = name;
            entity.SetLayer(9);
            storyActorInfo objEmpty = new storyActorInfo();
            objEmpty.target = entity.gameObject;
            objEmpty.dwModelId = 0;
            objEmpty.nameIndex = 1;
            objEmpty.name = "objEmpty";
            objEmpty.type = 3;
            objEmpty.bFolderOut = false;

            _storyActor.Add(objEmpty);
            return objEmpty;
        }

        public void Execute()
        {
            Debug.Log("--------------LuaANimEvent:Execute:" + _boardIndex);
            OnBtnNextState(false);
            startTime = Time.time;
            isPlaying = true;
            durtime = _storyBoard[_boardIndex].time;
            _storyBoard[_boardIndex].Execute();
            ++_boardIndex;
        }

        public bool OnNextStep()
        {
            if (_boardIndex >= Count)
            {
                Stop();
                return false;
            }
            return true;
        }

        public void Stop()
        {
            Debug.Log("--------------LuaANimEvent:Stop:" + _boardIndex);
            //结束所有遗留事件-以前未处理遗留时间如背景音乐、特效
            for (int i = 0; i < Count; ++i)
            {
                _storyBoard[i].OnForceNext();
            }
            _boardIndex = 0;
            isPlaying = false;
            startTime = float.MaxValue;
            OnBtnNextState(false);
        }
        
        public void  InitStoryBoard(StoryBoardCtrl boardCtrl)
        {
            boardCtrl.Clear();
            for (int i=0, imax = actorCount; i < imax; ++i)
            {
                storyActorInfo actor = _storyActor[i];
                StoryShotCtrl lenCtrl = new StoryShotCtrl();
                lenCtrl.actor = actor;
                boardCtrl.Add(lenCtrl);
            }
        }

        public string ExportBasicInfo()
        {
            string OnBeforeInitStory = _storyBasicInfo.OnBeforeInitStory.Replace(";\n",";");
            OnBeforeInitStory = OnBeforeInitStory.Replace("'", "\"");
            string OnAfterInitStory = _storyBasicInfo.OnAfterInitStory.Replace(";\n", ";");
            OnAfterInitStory = OnAfterInitStory.Replace("'", "\"");
            string OnBeforeEndStory = _storyBasicInfo.OnBeforeEndStory.Replace(";\n", ";");
            OnBeforeEndStory = OnBeforeEndStory.Replace("'", "\"");
            string szResult = string.Format("tabBasic = {{\n\tszStageName='{0}';\n\tszPathName='{1}';\n\tdwNextTime={2};\n\tbNewSceneState={3};\n\tbNewLightState={4};\n\tdwType={5};\n\tszSceneName='{6}';\n\tOnBeforeInitStory='{7}';\n\tOnAfterInitStory='{8}';\n\tOnBeforeEndStory='{9}';\n}};",
                _storyBasicInfo.szStageName, _storyBasicInfo.szPathName, _storyBasicInfo.dwNextTime, _storyBasicInfo.bNewSceneState ? "true" : "false", _storyBasicInfo.bNewLightState ? "true" : "false", _storyBasicInfo.dwType, Application.loadedLevelName, OnBeforeInitStory, OnAfterInitStory, OnBeforeEndStory);
            return szResult;
        }

        public void ImportBasicInfo(int dwIndex)
        {
            ILuaState lua = _fileLua;
            lua.PushValue(dwIndex);
            lua.PushString("szStageName");
            lua.GetTable(-2);
            _storyBasicInfo.szStageName = lua.L_CheckString(-1);
            lua.Pop(1);

            lua.PushString("szPathName");
            lua.GetTable(-2);
            if (lua.Type(-1) == LuaType.LUA_TSTRING)
                _storyBasicInfo.szPathName = lua.L_CheckString(-1);
            lua.Pop(1);

            lua.PushString("bNewSceneState");
            lua.GetTable(-2);
            _storyBasicInfo.bNewSceneState = lua.ToBoolean(-1);
            lua.Pop(1);

            lua.PushString("bNewLightState");
            lua.GetTable(-2);
            _storyBasicInfo.bNewLightState = lua.ToBoolean(-1);
            lua.Pop(1);

            lua.PushString("dwNextTime");
            lua.GetTable(-2);
            if (lua.Type(-1) ==LuaType.LUA_TNUMBER)
                _storyBasicInfo.dwNextTime = (float)lua.L_CheckNumber(-1);
            lua.Pop(1);

            lua.PushString("dwType");
            lua.GetTable(-2);
            _storyBasicInfo.dwType = lua.L_CheckInteger(-1);
            lua.Pop(1);

            lua.PushString("szSceneName");
            lua.GetTable(-2);
            if (lua.Type(-1) == LuaType.LUA_TSTRING)
                _storyBasicInfo.szSceneName = lua.L_CheckString(-1);
            lua.Pop(1);

            lua.PushString("OnBeforeInitStory");
            lua.GetTable(-2);
            if (lua.Type(-1) == LuaType.LUA_TSTRING)
                _storyBasicInfo.OnBeforeInitStory = lua.L_CheckString(-1).Replace(";", ";\n");
            lua.Pop(1);

            lua.PushString("OnAfterInitStory");
            lua.GetTable(-2);
            if (lua.Type(-1) == LuaType.LUA_TSTRING)
                _storyBasicInfo.OnAfterInitStory = lua.L_CheckString(-1).Replace(";", ";\n");
            lua.Pop(1);

            lua.PushString("OnBeforeEndStory");
            lua.GetTable(-2);
            if (lua.Type(-1) == LuaType.LUA_TSTRING)
                _storyBasicInfo.OnBeforeEndStory = lua.L_CheckString(-1).Replace(";", ";\n");
            lua.Pop(1);

            lua.Pop(1);
        }

        public string ExportActorInfo()
        {
            string actorInfo = "tabActor = {";
            for (int i = 0; i < actorCount; ++i)
            {
                storyActorInfo actor = _storyActor[i];
                string oneInfo = string.Format("\n    {{name='{0}';szRoleName='{1}';dwModelID={2};skeleton ='{3}';dwType={4};localPosition={{x=0,y=0,z=0}};localEulerAngles={{x=0,y=0,z=0}};localScale={{x=1,y=1,z=1}};}};", 
                    actor.target.name, actor.name, actor.dwModelId, actor.skeleton, actor.type);
                actorInfo += oneInfo;
            }
            actorInfo += "\n};";
            return actorInfo;
        }

        public void ImportActorInfo(int dwIndex)
        {
            _storyActor.Clear();
            ILuaState lua = _fileLua;
            lua.PushValue(dwIndex);
            int len = _fileLua.L_Len(-1);
            for (int i = 1; i <= len; i++)
            {
                lua.PushNumber(i);
                lua.GetTable(-2);
                lua.PushString("dwType");
                lua.GetTable(-2);
                int dwType = lua.L_CheckInteger(-1);
                lua.Pop(1);
                lua.PushString("dwModelID");
                lua.GetTable(-2);
                int dwModelID = lua.L_CheckInteger(-1);
                lua.Pop(1);
                lua.PushString("szRoleName");
                lua.GetTable(-2);
                string szRoleName = lua.L_CheckString(-1);
                lua.Pop(1);
                if (dwType == 3)
                {
                    AddEmpty();
                }
                else// if (dwType == 0)
                {
                    AddActor(dwModelID, i, dwType, szRoleName);
                }
                _fileLua.Pop(1);
            }
            _fileLua.Pop(1);
        }

        public override void ImportProperty(int dwIndex)
        {
            _storyBoard.Clear();
            ILuaState lua = _fileLua;
            lua.PushValue(dwIndex);
            int len = lua.L_Len(-1);
            for (int i = 1; i <= len; i++)
            {
                _fileLua.PushNumber(i);
                _fileLua.GetTable(-2);
                StoryBoardCtrl board = new StoryBoardCtrl();
                InitStoryBoard(board);
                board.ImportProperty(lua, -1);
                _fileLua.Pop(1);
                AddBoard(board);
            }
            _fileLua.Pop(1);
       }

        public override string ExportProperty(string[] strProperty)
        {
            if (Count == 0)
            {
                Debug.LogWarning("LuaAnimEvent:there is no any StoryBoard....");
                return "";
            }
            string strResult = "";
            for (int i = 0; i < Count; ++i)
            {
                StoryBoardCtrl board = _storyBoard[i];
                strResult += "[" + (i + 1) + "] = {" + board.ExportProperty(null) + "\n};";
            }
            return strResult;


            //             _fileLua.NewTable();
            //             for (int i = 0; i < _listCtrl.Count; i++)
            //             {
            //                 _fileLua.PushNumber(i + 1);
            //                 StoryBaseCtrl objCtrl = _listCtrl[i];
            //                 objCtrl.ExportProperty(_fileLua, -1);
            //                 _fileLua.SetTable(-3);
            //             }
            //             string strResult = SerializeTable(-1);
            //             _fileLua.Pop(1);
            //             return strResult;
        }

        void Update()
        {
            if (_storyBoard != null)
            {
                for (int i = 0; i < _storyBoard.Count; ++i)
                {
                    _storyBoard[i].CalculateTime();
                    _storyBoard[i].Update();
                }
            }
            if (isPlaying == true)
            {
                if (Time.time - startTime > durtime)
                {
//                     if (OnNextStep())
//                         Execute();
                    this.onBtnAnimNext();
                }
            }
        }

        public void OnProxyFinish(StoryBaseCtrl objCtrl)
        {
            objCtrl.OnFinish();
        }

        public static void OnImportFromLua(LuaAnimEvent animEvent)
        {
            ILuaState LuaApi = animEvent._fileLua;

            LuaApi.GetGlobal("StoryBasicInfoImport");
            if (LuaApi.Type(-1) != LuaType.LUA_TFUNCTION)
            {
                Debug.LogWarning("not found StoryBasicInfoImport function in lua script.");
                LuaApi.Pop(1);
                return;
            }
            if (LuaApi.PCall(0, 1, 0) != 0)
            {
                Debug.LogWarning(LuaApi.ToString(-1));
                LuaApi.Pop(1);
            }
            if (LuaApi.Type(-1) == LuaType.LUA_TNIL)
            {
                Debug.LogWarning("importLua failed, there is no event....");
                return;
            }
            animEvent.ImportBasicInfo(-1);
            LuaApi.Pop(1);

            LuaApi.GetGlobal("StoryAllActorImport");
            if (LuaApi.Type(-1) != LuaType.LUA_TFUNCTION)
            {
                Debug.LogWarning("not found StoryAllActorImport function in lua script.");
                LuaApi.Pop(1);
                return;
            }
            if (LuaApi.PCall(0, 1, 0) != 0)
            {
                Debug.LogWarning(LuaApi.ToString(-1));
                LuaApi.Pop(1);
            }
            if (LuaApi.Type(-1) == LuaType.LUA_TNIL)
            {
                Debug.LogWarning("importLua failed, there is no event....");
                return;
            }
            animEvent.ImportActorInfo(-1);
            LuaApi.Pop(1);

            LuaApi.GetGlobal("StoryAllEventImport");
            if (LuaApi.Type(-1) != LuaType.LUA_TFUNCTION)
            {
                Debug.LogWarning("not found StoryAllEventImport function in lua script.");
                LuaApi.Pop(1);
                return;
            }
            if (LuaApi.PCall(0, 1, 0) != 0)
            {
                Debug.LogWarning(LuaApi.ToString(-1));
                LuaApi.Pop(1);
            }
            if (LuaApi.Type(-1) == LuaType.LUA_TNIL)
            {
                Debug.LogWarning("importLua failed, there is no event....");
                return;
            }
            animEvent.ImportProperty(-1);
            LuaApi.Pop(1);
        }


        public static string GetBasicString(LuaAnimEvent animEvent)
        {
            return animEvent.ExportBasicInfo();
        }
        public static string GetActorString(LuaAnimEvent animEvent)
        {
            return animEvent.ExportActorInfo();
        }
        public static string GetEventString(LuaAnimEvent animEvent)
        {
            string eventString = "tabEventCtrl = {};";
            if (animEvent != null)
            {
                eventString = string.Format("tabEventCtrl = {{{0}\n}};", animEvent.ExportProperty(null).Replace("[", "\n["));
            }
            return eventString;
        }
        public static string GetEnvString()
        {
            return "tabEnv = {};";
        }
        public static string GetLightString()
        {
            return "tabLight = {};";
        }
        public static string GetClassString(int scriptID)
        {
            return string.Format("_G.StoryScript_{0} = StoryBaseSystem:Create();\nStoryScript_{0}.storyConfig = StoryConfig;", scriptID);
        }

#if UNITY_EDITOR
          public static void ExportToScriptFile(LuaAnimEvent animEvent, int scriptID)
        {
            string scriptString = string.Format("local StoryConfig = {{\n{0}\n{1}\n{2}\n{3}\n{4}\n}}\n{5}",
                    GetBasicString(animEvent), GetActorString(animEvent), GetEventString(animEvent), GetEnvString(), GetLightString(), GetClassString(scriptID));
            string path = Application.streamingAssetsPath + "//ScriptConfig//StoryConfig//StoryScript_" + scriptID.ToString() + ".lua";
            if (!File.Exists(path))
            {
                FileStream fstream = new FileStream(path, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fstream);
                sw.WriteLine(scriptString);
                sw.Close();
                sw.Dispose();
                Debug.Log("剧情脚本" + scriptID.ToString() + "已经导出完毕......");
            }
            else
            {
                if (EditorUtility.DisplayDialog("文件已经存在", "是否覆盖", "确认", "取消"))
                {
                    FileStream fstream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fstream);
                    sw.WriteLine(scriptString);
                    sw.Close();
                    sw.Dispose();
                    Debug.Log("文件覆盖已经完成：");
                }
            }
        }
        private static bool mbCameraFolderOut;
        public static void CameraSetting(LuaAnimEvent animEvent)
        {
            if (animEvent == null) return;
            GUILayout.Label(animEvent.gameObject.name);
            LuaGameCamera objCamera = StoryBaseCtrl.objMainCamera;
            mbCameraFolderOut = EditorGUILayout.Foldout(mbCameraFolderOut, "摄像机参数实时设置:(目标:" + ((objCamera.target != null) ? objCamera.target.name : "nil") + ")");
            if (mbCameraFolderOut)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("特写镜头"))
                {
                    objCamera.distance = 5;
                    objCamera.offset = new Vector3(0f, 0.6f, 0f);
                    objCamera.UDAngle = 45;
                    objCamera.LRAnge = 0;
                    objCamera.calculatePos(false);
                }
                if (GUILayout.Button("中景镜头"))
                {
                    objCamera.distance = 5;
                    objCamera.offset = new Vector3(0f, 1f, 0f);
                    objCamera.UDAngle = 0;
                    objCamera.LRAnge = 0;
                    objCamera.calculatePos(false);
                }
                if (GUILayout.Button("远景镜头"))
                {
                    objCamera.distance = 10.5f;
                    objCamera.offset = new Vector3(0f, 0.6f, 0f);
                    objCamera.UDAngle = 45;
                    objCamera.LRAnge = 0;
                    objCamera.calculatePos(false);
                }
                if (GUILayout.Button("游戏镜头"))
                {
                    objCamera.distance = 10.5f;
                    objCamera.offset = new Vector3(0f, 0.6f, 0f);
                    objCamera.UDAngle = 30;
                    objCamera.LRAnge = 0;
                    objCamera.calculatePos(false);
                }
                GUILayout.EndHorizontal();
                float distance = EditorGUILayout.FloatField("视 点 距 离", objCamera.distance);
                objCamera.offset = EditorGUILayout.Vector3Field("视 点 偏 移", objCamera.offset);
                float UDAngle = EditorGUILayout.FloatField("仰俯 偏转角度", objCamera.UDAngle);
                float LRAngle = EditorGUILayout.FloatField("水平 偏转角度", objCamera.LRAnge);
                if (objCamera.distance != distance)
                {
                    objCamera.distance = distance;
                    objCamera.calculatePos(false);
                }
                if (objCamera.LRAnge != LRAngle)
                {
                    objCamera.LRAnge = LRAngle;
                    objCamera.calculatePos(false);
                }
                if (objCamera.UDAngle != UDAngle)
                {
                    objCamera.UDAngle = UDAngle;
                    objCamera.calculatePos(false);
                }
            }
        }

        private static bool bOnInitBeforeInit;
        private static bool bOnInitAfterInit;
        private static bool bOnInitBeforeEnd;
        public static void showExtraFuncString(LuaAnimEvent animEvent)
        {
            if (animEvent == null) return;
            GUILayout.Space(10);
            bOnInitBeforeInit = EditorGUILayout.Foldout(bOnInitBeforeInit, "初始化角色之前:OnBeforeInitStory");
            if (bOnInitBeforeInit)
            {
                animEvent._storyBasicInfo.OnBeforeInitStory = GUILayout.TextArea(animEvent._storyBasicInfo.OnBeforeInitStory);
            }
            GUILayout.Space(10);
            bOnInitAfterInit = EditorGUILayout.Foldout(bOnInitAfterInit, "初始化角色之后:OnBeforeInitStory");
            if (bOnInitAfterInit)
            {
                animEvent._storyBasicInfo.OnAfterInitStory = GUILayout.TextArea(animEvent._storyBasicInfo.OnAfterInitStory);
            }
            GUILayout.Space(10);
            bOnInitBeforeEnd = EditorGUILayout.Foldout(bOnInitBeforeEnd, "剧情结束之前:OnBeforeInitStory");
            if (bOnInitBeforeEnd)
            {
                animEvent._storyBasicInfo.OnBeforeEndStory = GUILayout.TextArea(animEvent._storyBasicInfo.OnBeforeEndStory);
            }

        }
#else
        public static void CameraSetting(LuaAnimEvent animEvent)
        {
        }
        public static void showExtraFuncString(LuaAnimEvent animEvent){}

        public static void ExportToScriptFile(LuaAnimEvent animEvent, int scriptID)
        {
        }
#endif
    }
}
