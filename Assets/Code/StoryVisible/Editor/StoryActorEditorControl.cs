using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using xxstory;
using Hstj;

namespace xxstoryEditor
{
    public class StoryActorEditorControl : SidebarControl
    {

        private Texture LabelPrefix;
        private bool renameRequested;
        private bool isRenaming;
        private bool showContext;
        private int renameControlID;
        private int focusBaseid=-1;
        private int deleteBaseIndex = -1;

        public int type;
        public int dwModelId;
        public int nameIndex;
        public string skeleton;
        public string name;

        public override bool IsSelected
        {
            get
            {
                return StoryLayoutEditorControl.Instance.isSelectShot(this.nameIndex);
            }
            
        }

        public void bindControls(storyActorInfo sActorInfo)
        {
            this.type = sActorInfo.type;
            this.dwModelId = sActorInfo.dwModelId;
            this.nameIndex = sActorInfo.nameIndex;
            this.skeleton = sActorInfo.skeleton;
            this.name = sActorInfo.name;

            this.Behaviour = sActorInfo.target.gameObject;
            this.LabelPrefix = TrackGroupControl.styles.CharacterGroupIcon.normal.background;
        }

        public void updateShowContent( Rect header, Rect content)
        {
            this.showArea = new Rect(header.x, header.y, header.width + content.width, header.height);
            base.expandedSize = 0;
            this.updateHeaderBackground(new Rect(header.x, header.y, header.width, this.clipHeight));
            this.updateContentBackground(content);
            if (this.isExpanded)
            {
                this.updateTracks(header, content);
            }
            this.updateHeaderContent(header);

        }

        protected void updateHeaderBackground(Rect position)
        {
            if (this.IsSelected && !this.isRenaming)
            {
                GUI.Box(position, string.Empty, TrackGroupControl.styles.backgroundSelected);
            }
        }

        protected void updateContentBackground(Rect content)
        {
            if (this.IsSelected && !this.isRenaming)
            {
                GUI.Box(this.showArea, string.Empty, TrackGroupControl.styles.backgroundContentSelected);
                return;
            }
            GUI.Box(this.showArea, string.Empty, TrackGroupControl.styles.trackGroupArea);
        }

        protected void updateHeaderContent(Rect header)
        {
            Rect arg_9E_0 = new Rect(header.x, header.y, 14f, this.clipHeight);
            Rect rect = new Rect(header.x + 14f, header.y, 16f, this.clipHeight);
            Rect rect2 = new Rect(rect.x + rect.width, header.y, header.width - (rect.x + rect.width) - 32f, this.clipHeight);
            string text = "";
            if (!string.IsNullOrEmpty(this.name))
                text = string.Format("{0}:{1}",this.nameIndex,this.name);
            else
                text = string.Format("{0}:Actor{0}", this.nameIndex, this.name);
            bool flag = EditorGUI.Foldout(arg_9E_0, this.isExpanded, GUIContent.none, false);
            if (flag != this.isExpanded)
            {
                this.isExpanded = flag;
            }
            //Header
            GUI.Box(rect, this.LabelPrefix, GUIStyle.none);
            if (GUI.Button(new Rect(header.width - 64f, header.y, 16f, 16f), GUIContent.none, TrackGroupControl.styles.PlayIcon))
            {
                int boardIndex = StoryLayoutEditorControl.Instance.SelectBoardIndex;
                StoryLayoutEditorControl.Instance.boardEditorControls[boardIndex][nameIndex - 1].Execute();
            }
            if (!this.isRenaming)
            {
                string text2 = text;
                Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(text2));
                while (vector.x > rect2.width && text2.Length > 5)
                {
                    text2 = text2.Substring(0, text2.Length - 4) + "...";
                    vector = GUI.skin.label.CalcSize(new GUIContent(text2));
                }
                int controlID = GUIUtility.GetControlID(this.Behaviour.GetInstanceID(), (FocusType)2, header);
                if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown)
                {
                    if (header.Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        if (!this.IsSelected)
                        {
                            this.RequestSelect(0, this.nameIndex,0);
                        }
                        this.showHeaderContextMenu(nameIndex, false);
                        Event.current.Use();
                    }
                    else if (header.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        this.RequestSelect(0, this.nameIndex, 0);
                        Event.current.Use();
                    }
                }
                if (this.IsSelected)
                {
                    GUI.Label(rect2, text2, EditorStyles.whiteLabel);
                    return;
                }
                GUI.Label(rect2, text2);
            }

        }

        protected void updateTracks(Rect header, Rect content)
        {
            
            foreach (int dwFouseBoardID in StoryLayoutEditorControl.Instance.SelectBoardIndexs)
            {
                StoryShotEditorControl shotCtrl = StoryLayoutEditorControl.Instance.boardEditorControls[dwFouseBoardID][this.nameIndex - 1];
                base.expandedSize = shotCtrl.Count;
                float bWidth = StoryLayoutEditorControl.Instance.pixelTotime(content.x);
                for (int i = 0; i < dwFouseBoardID; ++i)
                {
                    bWidth += StoryLayoutEditorControl.Instance.boardEditorControls[i].controlWidth;
                }
                float contentHeight = header.y+clipHeight;
                for (int i = 0, imax = shotCtrl.Count; i < imax; ++i)
                {
                    StoryBaseEditorControl bctrl = shotCtrl[i];
                    Rect rect = new Rect(content.x, contentHeight, content.width, clipHeight);
                    Rect headerBackground = new Rect(header.x, contentHeight, header.width, clipHeight);
                    Rect labelGround = new Rect(header.x + 14f, contentHeight, header.width - 14f-48f, clipHeight);
                    Rect contentGround = new Rect(bWidth, contentHeight, bctrl.controlWidth, clipHeight);
                    Rect rect3 = new Rect(rect.x - 48f, rect.y, 16f, 16f);
                    Rect rect4 = new Rect(rect.x - 32f, rect.y, 16f, 16f);
                    Rect rect5 = new Rect(rect.x-16f, rect.y, 16f, 16f);
                    GUI.Box(rect, string.Empty, TimelineTrackControl.styles.TrackAreaStyle);
                    bctrl.UpdateTracksBackground(contentGround, rect);
                    if (labelGround.Contains(Event.current.mousePosition) && Event.current.button == 1)
                    {
                        this.deleteBaseIndex = i;
                        this.showHeaderContextMenu(i, true);
                    }
                    string ctrlName = string.Format("{0}:{1}({2}s)",i ,shotCtrl[i].ctrlName, shotCtrl[i].time);
                    if (bctrl.IsSelected)
                    {
                        GUI.Box(headerBackground, string.Empty, TimelineTrackControl.styles.backgroundSelected);
                        GUI.Label(labelGround,ctrlName, EditorStyles.whiteLabel);
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            GUI.Box(headerBackground, string.Empty, TimelineTrackControl.styles.TrackSidebarBG2);
                        }
                        else
                        {
                            GUI.Box(headerBackground, string.Empty, TimelineTrackControl.styles.TrackSidebarBG1);
                        }
                        GUI.Label(labelGround, ctrlName);
                    }
                    if(GUI.Button(rect3, string.Empty, TrackGroupControl.styles.PlayIcon))
                    {
                        bctrl._storyBaseCtrl.Execute();
                    }
                    if (GUI.Button(rect4, string.Empty, TrackGroupControl.styles.UpArrowIcon))
                    {
                        if (i != 0)
                        {
                            bctrl._storyshotECtrl.Replace(bctrl, i - 1);
                        }
                    }
                    if (GUI.Button(rect5, string.Empty, TrackGroupControl.styles.DownArrowIcon))
                    {
                        if (i != imax - 1)
                        {
                            bctrl._storyshotECtrl.Replace(bctrl, i + 1);
                        }
                    }
                    contentHeight += base.clipHeight;
                    if (bctrl.bWait == true)
                    {
                        bWidth += bctrl.controlWidth;
                    }
                }
            }
        }

        protected void showHeaderContextMenu(int index, bool bTracks)
        {
            GenericMenu expr_05 = new GenericMenu();
            expr_05.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction(this.shot_copy));
            expr_05.AddItem(new GUIContent("Paste"), false, new GenericMenu.MenuFunction(this.shot_paste));
            expr_05.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.base_delete));
            if (bTracks)
            {
                expr_05.AddDisabledItem(new GUIContent("Copy"));
                expr_05.AddDisabledItem(new GUIContent("Paste"));
            }
            expr_05.ShowAsContext();
        }
        private void shot_copy() 
        {
            StoryLayoutEditorControl.Instance.copyShotCtrl(this.nameIndex - 1);
        }
        private void shot_paste()
        {
            StoryLayoutEditorControl.Instance.pasteShotCtrl(this.nameIndex - 1);
        }
        private void base_delete() 
        {
            if (this.deleteBaseIndex == -1)
            {
                StoryLayoutEditorControl.Instance.deleteShot(this.nameIndex - 1);
            }
            else
            {
                StoryLayoutEditorControl.Instance.deleteBase(this.nameIndex - 1, this.deleteBaseIndex);
            }
            this.deleteBaseIndex = -1;
        }

    }

}
