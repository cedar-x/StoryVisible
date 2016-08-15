using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using Hstj;
using xxstory;

namespace xxstoryEditor
{
    public delegate void SidebarControlHandler(object sender, int boardIndex, int shotIndex, int baseIndex);
    public class SidebarControl
    {
        public event SidebarControlHandler DeleteRequest;
        public event SidebarControlHandler SelectRequest;

        private GameObject behaviour;
        private bool _isSelected;
        private float _width;

        public int expandedSize = 0;
        public bool isExpanded;
        public Rect showArea;

        public float zoom
        {
            get
            {
                return StoryLayoutEditorControl.Instance.Scale.x;
            }
        }

        public float clipHeight
        {
            get
            {
                return 17f;
            }
        }

        public float controlHeight
        {
            get
            {
                return clipHeight + expandedSize * clipHeight;
            }
        }

        public float time
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public float controlWidth
        {
            get
            {
                return _width*zoom;
            }
        }

        public GameObject Behaviour
        {
            get
            {
                return this.behaviour;
            }
            set
            {
                this.behaviour = value;
            }
        }

        public virtual bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public void RequestSelect(int boardIndex, int shotIndex, int baseIndex)
        {
            if (this.SelectRequest != null)
            {
                this.SelectRequest(this, boardIndex, shotIndex, baseIndex);
            }
//             if (this.behaviour == null)
//                 return;
//             if (Event.current.control)
//             {
//                 if (Selection.Contains(behaviour.gameObject))
//                 {
//                     GameObject[] gameObjects = Selection.gameObjects;
//                     ArrayUtility.Remove<GameObject>(ref gameObjects, behaviour.gameObject);
//                     Selection.objects = (gameObjects);
//                 }
//                 else
//                 {
//                     GameObject[] gameObjects2 = Selection.gameObjects;
//                     ArrayUtility.Add<GameObject>(ref gameObjects2, behaviour.gameObject);
//                     Selection.objects = (gameObjects2);
//                 }
//             }
//             else
//             {
//                 Selection.activeObject = behaviour;
//             }
//             Event.current.Use();
        }

        public void RequestDelete(int boardIndex, int shotIndex, int baseIndex)
        {
            if (this.DeleteRequest != null)
            {
                this.DeleteRequest(this, boardIndex, shotIndex, baseIndex);
            }
        }

    }
}
