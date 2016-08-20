using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using xxstory;

namespace xxstoryEditor
{
    public class StoryLayoutEditorControl : TimeArea
    {
        protected static StoryLayoutEditorControl _Instance = null;

        private bool hasLayoutChanged;

        private Rect previousControlArea;
        private Rect headerArea;
        private Rect bodyArea;
        private Rect timeRuleArea;
        private Rect boardBodyArea;
        private Rect trackBodyBackground;
        private Rect trackHeadBackground;
        private Rect trackBodyBackgroundWithHorizontalScrollbar;
        private Rect verticalScrollbarArea;
        private Rect sidebarControlArea;
        private float headHeight = 45f;
        private float clipHeight = 17f;
        private Dictionary<int, StoryActorEditorControl> _actorEditorControls = new Dictionary<int, StoryActorEditorControl>();
        private List<StoryBoardEditorControl> _boardEditorControls = new List<StoryBoardEditorControl>();
        private List<int> tabBoardIndexs = new List<int>();
        private int tabShotIndex;
        private StoryBaseEditorControl selectBECtrl;

        private LuaAnimEvent _luaAnimEvent;

        private float nameWidth
        {
            get
            {
                return 200f;
            }
        }

        public StoryLayoutEditorControl()
        {
            _Instance = this;
        }
        public static StoryLayoutEditorControl Instance
        {
            get
            {
                return _Instance;
            }
        }

        public List<StoryBoardEditorControl> boardEditorControls
        {
            get
            {
                return _boardEditorControls;
            }
        }

        public List<int> SelectBoardIndexs
        {
            get
            {
                return tabBoardIndexs;
            }
        }

        public int SelectBoardIndex
        {
            get
            {
                if (tabBoardIndexs.Count == 0)
                    return 0;
                return tabBoardIndexs[0];
            }
            set
            {
                if (tabBoardIndexs.Contains(value))
                {
                    tabBoardIndexs.Remove(value);
                }
                else
                {
                    tabBoardIndexs.Add(value);
                }
            }
        }

        public int SelectShotIndex
        {
            get
            {
                return tabShotIndex;
            }
            set
            {
                tabShotIndex = value;
            }
        }

        public StoryBaseEditorControl SelectBaseCtrl
        {
            get
            {
                return selectBECtrl;
            }
            set
            {
                selectBECtrl = value;
            }
        }



        public void OnGUI(Rect controlArea, LuaAnimEvent animEvent)
        {
            this._luaAnimEvent = animEvent;
            this.updateControlLayout(controlArea);
            this.drawBackground();
            this.updateTimelineHeader(this.headerArea, this.timeRuleArea);
            //Debug.Log("======OnGUI:" + base.Translation.x);
            if (this._luaAnimEvent != null)
            {
                this.bindControls(this._luaAnimEvent);
                this.updateStoryBoard(this.boardBodyArea);
                GUILayout.BeginArea(this.bodyArea, string.Empty);
                this.updateActorEvent();
                GUILayout.EndArea();
            }
        }

        private void updateControlLayout(Rect controlArea)
        {
            this.hasLayoutChanged = (controlArea != this.previousControlArea);

            if (this.hasLayoutChanged)
            {
                this.sidebarControlArea = new Rect(0, 0, 0, 0);
                this.headerArea = new Rect(controlArea.x, controlArea.y, controlArea.width, headHeight);
                this.timeRuleArea = new Rect(nameWidth + this.sidebarControlArea.width, controlArea.y + 25, controlArea.width - nameWidth - this.sidebarControlArea.width - 15f, headHeight - 25);
                this.boardBodyArea = new Rect(nameWidth + this.sidebarControlArea.width, controlArea.y, controlArea.width - nameWidth - this.sidebarControlArea.width - 15f, headHeight - 25);
                this.bodyArea = new Rect(controlArea.x, controlArea.y + headHeight, controlArea.width, controlArea.height - headHeight);
                this.trackHeadBackground = new Rect(controlArea.x, this.bodyArea.y + headHeight, nameWidth, this.bodyArea.height - headHeight);
                this.trackBodyBackground = new Rect(controlArea.x + nameWidth + this.sidebarControlArea.width, this.bodyArea.y, controlArea.width - 15f - this.nameWidth - this.sidebarControlArea.width, controlArea.height - headHeight - 15f);
                this.trackBodyBackgroundWithHorizontalScrollbar = new Rect(controlArea.x + nameWidth + this.sidebarControlArea.width, this.bodyArea.y, controlArea.width - 15f - this.nameWidth - this.sidebarControlArea.width, controlArea.height - headHeight);
                this.verticalScrollbarArea = new Rect(this.bodyArea.x + this.bodyArea.width - 15f, this.bodyArea.y, 15f, controlArea.height - headHeight - 15f);
            }

            this.previousControlArea = controlArea;
        }

        private void bindControls(LuaAnimEvent luaAnimEvent)
        {
            //更新事件名称列
            for (int i = 0; i < luaAnimEvent.actorCount; ++i)
            {
                int nameIndex = luaAnimEvent._storyActor[i].nameIndex;
                if (!_actorEditorControls.ContainsKey(nameIndex))
                {
                    StoryActorEditorControl actorECtrl = new StoryActorEditorControl();
                    actorECtrl.SelectRequest += new SidebarControlHandler(this.shotControl_SelectRequest);
                    _actorEditorControls.Add(nameIndex, actorECtrl);
                }
                _actorEditorControls[nameIndex].bindControls(_luaAnimEvent._storyActor[i]);
            }
            //更新事件Board列
            for (int i = 0, imax = luaAnimEvent.Count; i < imax; ++i)
            {
                StoryBoardCtrl boardCtrl = luaAnimEvent[i];
                if (_boardEditorControls.Count <= i)
                {
                    StoryBoardEditorControl beCtrl = new StoryBoardEditorControl();
                    beCtrl.SelectRequest += new SidebarControlHandler(this.boardControl_SelectRequest);
                    _boardEditorControls.Insert(i, beCtrl);
                }
                _boardEditorControls[i].bindControls(boardCtrl, i);
            }

        }

        private void drawBackground()
        {
            GUI.Box(this.trackBodyBackground, GUIContent.none, "AnimationCurveEditorBackground");
            base.rect = this.trackBodyBackgroundWithHorizontalScrollbar;
            base.BeginViewGUI(false);
            base.SetTickMarkerRanges();
            base.DrawMajorTicks(this.trackBodyBackground, 60f);
            base.EndViewGUI();
        }

        private void updateTimelineHeader(Rect headerArea, Rect timeRulerArea)
        {
            GUILayout.BeginArea(headerArea, GUIContent.none, "AnimationCurveEditorBackground");
            GUILayout.BeginHorizontal();
            if (_luaAnimEvent != null && GUILayout.Button(TrackGroupControl.textures.playImage, EditorStyles.toolbarButton, GUILayout.Width(30)))
            {
                _luaAnimEvent.Execute();
            }
            GUILayout.Label("StoryBoard", GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            base.TimeRuler(timeRuleArea, 60f);
        }

        private void updateStoryBoard(Rect content)
        {
            float x = this.pixelTotime(content.x);
            for (int i = 0, imax = _boardEditorControls.Count; i < imax; ++i)
            {
                StoryBoardEditorControl boardECtrl = _boardEditorControls[i];
                float width = boardECtrl.controlWidth;
                if (width < 0.9f)
                    width = 5*base.Scale.x;
                boardECtrl.updateShowContent(new Rect(x, content.y, width, content.height));
                x += width;
            }
        }

        private void updateActorEvent()
        {
            float curveClipHeight = this.clipHeight;
            float eventWidth = this.bodyArea.width - nameWidth;
            float actorHeight = 0f;
            //更新事件名称列
            foreach (int key in _actorEditorControls.Keys)
            {
                StoryActorEditorControl aControl = _actorEditorControls[key];
                float height = aControl.controlHeight;
                Rect header = new Rect(this.trackHeadBackground.x, actorHeight, this.trackHeadBackground.width, height);
                Rect content = new Rect(this.trackBodyBackground.x, actorHeight, this.trackBodyBackground.width, height);
                aControl.updateShowContent(header, content);
                actorHeight += height;
            }
        }

        private void boardControl_SelectRequest(object sender, int boardIndex, int shotIndex, int baseIndex)
        {
            Debug.Log("=====boardControl_SelectRequest======" + boardIndex + ":" + shotIndex + ":" + baseIndex);
            SelectBoardIndex = boardIndex;
        }

        private void shotControl_SelectRequest(object sender, int boardIndex, int shotIndex, int baseIndex)
        {
            Debug.Log("=====shotControl_SelectRequest======" + boardIndex + ":" + shotIndex + ":" + baseIndex);
            SelectShotIndex = shotIndex;
        }

        private void deleteBoard(StoryBoardEditorControl bCtrl)
        {
            _boardEditorControls.Remove(bCtrl);
        }

        public void deleteShot(int index, StoryShotEditorControl bCtrl)
        {
            _boardEditorControls[index].Delete(bCtrl);
        }

        public void deleteBase(int boardIndex, int shotIndex, StoryBaseEditorControl bCtrl)
        {
            _boardEditorControls[boardIndex][shotIndex].Delete(bCtrl);
        }

        public void ExecuteBoard(int index)
        {
            _luaAnimEvent._storyBoard[index].Execute();
        }

        public void deleteBoard(int index)
        {
            this.deleteBoard(_boardEditorControls[index]);
            StoryBoardCtrl bCtrl = _luaAnimEvent._storyBoard[index];
            _luaAnimEvent.DeleteBoard(bCtrl);
        }

        public void deleteShot(int index)
        {
            int boardIndex = SelectBoardIndex;
            StoryShotEditorControl bECtrl = _boardEditorControls[boardIndex][index];
            this.deleteShot(boardIndex, bECtrl);
            StoryShotCtrl bCtrl = _luaAnimEvent._storyBoard[boardIndex][index];
            bool bFlag = _luaAnimEvent._storyBoard[boardIndex].Delete(bCtrl);
            if (bFlag == true && _luaAnimEvent.objEditorShotCtrl == bCtrl)
            {
                _luaAnimEvent.objEditorShotCtrl = null;
            }
        }

        public void deleteBase(int shotIndex, int baseIndex)
        {
            int boardIndex = SelectBoardIndex;
            StoryBaseEditorControl bEctrl = _boardEditorControls[boardIndex][shotIndex][baseIndex];
            this.deleteBase(boardIndex, shotIndex, bEctrl);
            StoryBaseCtrl bCtrl = _luaAnimEvent._storyBoard[boardIndex][shotIndex][baseIndex];
            _luaAnimEvent._storyBoard[boardIndex][shotIndex].Delete(bCtrl);
        }

        public void copyShotCtrl(int index)
        {
            _luaAnimEvent.objEditorCopyCtrl = _luaAnimEvent._storyBoard[SelectBoardIndex][index];
        }

        public void pasteShotCtrl(int index)
        {
            _luaAnimEvent._storyBoard[SelectBoardIndex][index].paste(_luaAnimEvent.objEditorCopyCtrl);
        }

        public bool isSelectBoard(int index)
        {
            return tabBoardIndexs.Contains(index);
        }

        public void ClearSelectBoard()
        {
            tabBoardIndexs.Clear();
        }

        public bool isSelectShot(int index)
        {
            return SelectShotIndex == index;
        }

        public bool isSelectBase(StoryBaseEditorControl baseEctrl)
        {
            return baseEctrl == SelectBaseCtrl;
        }

        public float pixelTotime(float x)
        {
            return x + base.Translation.x;
        }

        public void Rescale()
        {
            float duration = 0;
            if (this._luaAnimEvent != null)
            {
                for (int i = 0, imax = this._luaAnimEvent.Count; i < imax; ++i)
                {
                    duration += this._luaAnimEvent[i].time;
                }
            }
            if (duration == 0)
                duration = 30f;
            base.HScrollMax = duration;
            base.SetShownHRangeInsideMargins(0f, duration);
        }

        public void ZoomIn()
        {
            base.Scale *= 1.5f;
        }

        public void ZoomOut()
        {
            base.Scale *= 0.75f;
        }

        /// <summary>
        /// ///////////////////////////////////////////
        /// </summary> 
        private bool mbExportFolderOut;
        private int _dwScriptID;
        private int _insertIndex = -1;//使用此参数代表当前时间添加到哪个位置
        private string szEditorState = "实例参数"; //使用此参数代表当前是待添加事件还是修改事件
        private StoryBaseCtrl _tmpEditorCtrl;

        public void updateInspector(Rect content)
        {
            LuaAnimEvent.CameraSetting(_luaAnimEvent);
            StoryBaseCtrl _selectCtrl = this._tmpEditorCtrl;
            int boardIndex = this.SelectBoardIndex;
            int shotIndex = this.SelectShotIndex;

            if (this.SelectBaseCtrl != null && this.SelectBaseCtrl._storyBaseCtrl != null)
            {
                _selectCtrl = this.SelectBaseCtrl._storyBaseCtrl;
                szEditorState = "修改事件:" + _selectCtrl._shotCtrl.actorName;
            }
            else if (_selectCtrl != null)
            {
                szEditorState = "待添加事件:" + boardIndex+":"+shotIndex;
            }
            else
            {
                this.updateEventItemList(content);
                return;
            }

            GUILayout.Label("---------------------------------------------\n----" + szEditorState + "----\n----------------------------------------");
            _selectCtrl.OnParamGUI();

            GUILayout.BeginHorizontal();
            _insertIndex = EditorGUILayout.IntField(_insertIndex, GUILayout.Width(30));
            if (GUILayout.Button("添加"))
            {
                if (_selectCtrl == null) return;
                StoryBaseCtrl objCtrl = _selectCtrl.CopySelf();
                objCtrl.ModInfo();

                foreach (StoryActorEditorControl actorECtrl in _actorEditorControls.Values)
                {
                    if (actorECtrl.IsSelected)
                    {
                        _luaAnimEvent[boardIndex][actorECtrl.nameIndex - 1].Add(objCtrl, _insertIndex);
                    }
                }
                _insertIndex = -1;
            }
            if (GUILayout.Button("修改"))
            {
                if (_selectCtrl == null) return;
                _selectCtrl.ModInfo();
            }
            if (GUILayout.Button("存储点"))
            {
                if (_selectCtrl == null) return;
                _selectCtrl.SavePoint();
            }
            if (GUILayout.Button("重设"))
            {
                if (_selectCtrl == null) return;
                _selectCtrl.ResetPoint(false);
            }
            if (GUILayout.Button("放弃"))
            {
                if (_selectCtrl != null)
                    _selectCtrl.ResetPoint(false);
                if (this.SelectBaseCtrl != null)
                    this.SelectBaseCtrl = null;
                else
                    this._tmpEditorCtrl = null;
            }
            GUILayout.EndHorizontal();
            //GUILayout.EndArea();
        }

        public void updateEventItemList(Rect content)
        {
            string titleName = "";
            int dwIndex = 1;
            bool bInit = false;
            for (int i = 0; i < TrackGroupControl.EventItemName.GetLength(0); ++i)
            {
                string preName = TrackGroupControl.EventItemName[i, 0];
                string nexName = TrackGroupControl.EventItemName[i, 1];
                string luaName = TrackGroupControl.EventItemName[i, 2];
                if (titleName != preName)
                {
                    if (bInit == true)
                        GUILayout.EndHorizontal();
                    GUILayout.Label(preName);
                    GUILayout.BeginHorizontal();
                    titleName = preName;
                    bInit = true;
                    dwIndex = 1;
                }
                if (GUILayout.Button(nexName))
                {
                    this._tmpEditorCtrl = StoryShotCtrl.InstanceEventCtrl(luaName);
                }
                if (dwIndex % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                dwIndex += 1;
            }
            GUILayout.EndHorizontal();
        }

        public void updateExportInfo()
        {
            mbExportFolderOut = EditorGUILayout.Foldout(mbExportFolderOut, "导出剧情至lua脚本");
            if (mbExportFolderOut == true)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("剧情ID：");
                _dwScriptID = EditorGUILayout.IntField(_dwScriptID);
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("导出"))
                {
                    if (_luaAnimEvent == null)
                    {
                        Debug.LogWarning("there is no Create Time Ctrl...");
                        return;
                    }
                    LuaAnimEvent.ExportToScriptFile(_luaAnimEvent, _dwScriptID);
                }
            }
            if (!Application.isPlaying) return;
            if (_luaAnimEvent == null)
            {
                if (GUILayout.Button("Create Time Ctrl"))
                {
                    GameObject obj = new GameObject("New Anim Event");
                    obj.AddComponent<LuaAnimEvent>().InitMemeber();
                }
            }
            else
            {
                if (GUILayout.Button("ImportLua"))
                {
                    LuaAnimEvent.OnImportFromLua(_luaAnimEvent);
                    this.Rescale();
                }
                LuaAnimEventEditor.ShowActorInfo(_luaAnimEvent);
            }
        }

        public void updateExtraFunc()
        {
            LuaAnimEvent.showExtraFuncString(_luaAnimEvent);
        }

    }
}
