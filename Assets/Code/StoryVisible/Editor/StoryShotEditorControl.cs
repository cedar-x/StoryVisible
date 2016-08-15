using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xxstory;

namespace xxstoryEditor
{
    public class StoryShotEditorControl : SidebarControl
    {
        private List<StoryBaseEditorControl> _storyBaseEditorCtrl = new List<StoryBaseEditorControl>();
        private StoryShotCtrl _storyShotCtrl;
        public int actorIndex;
        
        public int Count
        {
            get
            {
                return _storyBaseEditorCtrl.Count;
            }
        }
        
        public StoryBaseEditorControl this[int index]
        {
            get
            {
                if (index < 0)
                    Debug.LogError("StoryShotEditorControl Index can't be minus");
                if (index >= Count)
                    Debug.LogError("StoryShotEditorControl Index out of range");
                return _storyBaseEditorCtrl[index];
            }
        }
        
        public void Replace(StoryBaseEditorControl baseECtrl, int dwIndex)
        {
            int yuanIndex = _storyBaseEditorCtrl.IndexOf(baseECtrl);
            if (yuanIndex == -1)
            {
                Debug.Log("StoryShotEditorCtrl can't replace StorybaseCtrl:" + baseECtrl.ctrlName);
                return;
            }
            if (dwIndex < 0 || dwIndex >= Count)
            {
                Debug.LogError("StoryShotEditorCtrl Index min zero or out of range:" + dwIndex);
                return;
            }
            var temp = _storyBaseEditorCtrl[yuanIndex];
            _storyBaseEditorCtrl[yuanIndex] = _storyBaseEditorCtrl[dwIndex];
            _storyBaseEditorCtrl[dwIndex] = temp; 
            baseECtrl._storyBaseCtrl._shotCtrl.Replace(baseECtrl._storyBaseCtrl, dwIndex);
        }

        public void Delete(StoryBaseEditorControl bCtrl)
        {
            _storyBaseEditorCtrl.Remove(bCtrl);
        }
        public void Execute()
        {
            this._storyShotCtrl.Execute();
        }
        
        public void bindControls(StoryShotCtrl storyShotCtrl)
        {
            this._storyShotCtrl = storyShotCtrl;
            this.time = storyShotCtrl.time;
            this.actorIndex = storyShotCtrl.actorIndex;
            for (int i = 0, imax = storyShotCtrl.Count; i < imax; ++i)
            {
                StoryBaseCtrl baseCtrl = storyShotCtrl[i];
                if (_storyBaseEditorCtrl.Count <= i)
                {
                    StoryBaseEditorControl bsCtrl = new StoryBaseEditorControl();
                    _storyBaseEditorCtrl.Insert(i, bsCtrl);
                }
                _storyBaseEditorCtrl[i].bindControls(baseCtrl, this);
            }
        }

    }
}
