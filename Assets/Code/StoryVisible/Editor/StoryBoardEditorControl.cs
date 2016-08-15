using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using xxstory;

namespace xxstoryEditor
{
    public class StoryBoardEditorControl : SidebarControl
    {


        public List<StoryShotEditorControl> _shotEditorCtrl = new List<StoryShotEditorControl>();
        private int boardIndex;

        public override bool IsSelected
        {
            get
            {
                return StoryLayoutEditorControl.Instance.isSelectBoard(this.boardIndex);
            }
        }

        public int Count
        {
            get
            {
                return _shotEditorCtrl.Count;
            }
        }

        public StoryShotEditorControl this[int index]
        {
            get
            {
                if (index < 0)
                    Debug.LogError("StoryBoardEditorCtrl Index can't be minus");
                if (index >= Count)
                    Debug.LogError("StoryBoardEditorCtrl Index out of range");
                return _shotEditorCtrl[index];
            }
        }

        public void Delete(StoryShotEditorControl bCtrl)
        {
            _shotEditorCtrl.Remove(bCtrl);
        }
        public void bindControls(StoryBoardCtrl storyBoardCtrl, int boardIndex)
        {
            this.time = storyBoardCtrl.time;
            this.boardIndex = boardIndex;
            for (int i = 0, imax = storyBoardCtrl.Count; i < imax; ++i)
            {
                StoryShotCtrl shotCtrl = storyBoardCtrl[i];
                if (_shotEditorCtrl.Count <= i)
                {
                    StoryShotEditorControl beCtrl = new StoryShotEditorControl();
                    _shotEditorCtrl.Insert(i, beCtrl);
                }
                _shotEditorCtrl[i].bindControls(shotCtrl);
            }
        }

        public void updateShowContent(Rect controlArea)
        {
            Rect controlPosition = controlArea;
            int controlID = GUIUtility.GetControlID((FocusType)2, controlPosition);
            if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown)
            {
                if (controlPosition.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    if (Event.current.control)
                    {
                        this.RequestSelect(this.boardIndex, 0, 0);
                    }
                    else if (Event.current.alt)
                    {
                        if(Event.current.shift)
                            this.RequestSelect(this.boardIndex-1, 0, 0);
                        else
                            this.RequestSelect(this.boardIndex + 1, 0, 0);
                    }
                    else
                    {
                        bool isSel = this.IsSelected;
                        StoryLayoutEditorControl.Instance.ClearSelectBoard();
                        if(!isSel)
                            this.RequestSelect(this.boardIndex, 0, 0);
                    }
                }
                else if (controlPosition.Contains(Event.current.mousePosition) && Event.current.button == 1)
                {
                    GenericMenu expr_05 = new GenericMenu();
                    expr_05.AddItem(new GUIContent("Execute"), false, new GenericMenu.MenuFunction(this.board_Execute));
                    expr_05.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.board_delete));
                    expr_05.ShowAsContext();
                }
            }
            if (this.IsSelected)
            {
                GUI.Box(controlPosition,new GUIContent(string.Format("{0}s", this.time)), TimelineTrackControl.styles.ShotTrackItemSelectedStyle);
            }
            else
            {
                GUI.Box(controlPosition, new GUIContent(string.Format("{0}s", this.time)), TimelineTrackControl.styles.ShotTrackItemStyle);
            }
        }
        private void board_Execute()
        {
            StoryLayoutEditorControl.Instance.ExecuteBoard(this.boardIndex);
        }
        private void board_delete()
        {
            StoryLayoutEditorControl.Instance.deleteBoard(this.boardIndex);
        }

    }
}
