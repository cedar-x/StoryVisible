using UnityEngine;
using System.Collections;
using UnityEditor;
using xxstory;
using Hstj;

namespace xxstoryEditor
{
    public class StoryBaseEditorControl : SidebarControl
    {
        public bool bWait;
        public string luaName;
        public string ctrlName;
        public float startTime;
        public StoryBaseCtrl _storyBaseCtrl;
        public StoryShotEditorControl _storyshotECtrl;
        public override bool IsSelected
        {
            get
            {
                return StoryLayoutEditorControl.Instance.isSelectBase(this);
            }
        }
        public void bindControls(StoryBaseCtrl storyBaseCtrl, StoryShotEditorControl storyShotECtrl)
        {
            this._storyBaseCtrl = storyBaseCtrl;
            this._storyshotECtrl = storyShotECtrl;
            this.bWait = storyBaseCtrl.bWait;
            this.time = storyBaseCtrl.time;
            this.luaName = storyBaseCtrl.luaName;
            this.ctrlName = storyBaseCtrl.ctrlName;
        }
        public void UpdateTracksBackground(Rect content, Rect clickArea)
        {
            float num = content.x;
            float num2 = content.x + content.width;
            bool expr_67 = content.width < 15f;
            float num3 = expr_67 ? 0f : 5f;
            Rect rect = new Rect(num, content.y, num3, content.height);
            Rect rect2 = new Rect(num + num3, content.y, num2 - num - 2f * num3, content.height);
            Rect rect3 = new Rect(num2 - num3, content.y, num3, content.height);
            EditorGUIUtility.AddCursorRect(rect2, MouseCursor.SlideArrow);
            if (!expr_67)
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(rect3, MouseCursor.ResizeHorizontal);
            }
            int controlID = GUIUtility.GetControlID(FocusType.Passive, clickArea);
            if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown )
            {
                if (clickArea.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    if (this.IsSelected)
                        StoryLayoutEditorControl.Instance.SelectBaseCtrl = null;
                    else
                        StoryLayoutEditorControl.Instance.SelectBaseCtrl = this;
                }
            }
            if (this.IsSelected)
            {
                GUI.Box(content, GUIContent.none, TimelineTrackControl.styles.TrackItemSelectedStyle);
            }
            else
            {
                GUI.Box(content, GUIContent.none, TimelineTrackControl.styles.TrackItemStyle);
            }
        }
    }
}
