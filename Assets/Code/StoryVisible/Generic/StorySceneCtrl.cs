using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace xxstory
{
    public class StoryEntity : MonoBehaviour
    {
        private int _id;
        private bool _visible;
        public int id
        {
            get{
                return _id;
            }
            set{
                _id = value;
            }
        }
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
    }
    public class StorySceneCtrl
    {
        private static StorySceneCtrl _instance;
        private GameObject m_objRoot;
        private bool m_bSceneVisible;
        protected Dictionary<int, StoryEntity> _entities = new Dictionary<int, StoryEntity>();

        public static StorySceneCtrl Instance()
        {
            if (_instance == null)
            {
                _instance = new StorySceneCtrl();
            }
            return _instance;
        }
        private void InitSceneRoot()
        {
            m_objRoot = new GameObject();
            m_objRoot.name = "StorySceneObjects";
            Object.DontDestroyOnLoad(m_objRoot);
        }
        private void AddEntity(StoryEntity entity)
        {
            _entities[entity.id] = entity;


            if (m_objRoot == null)
            {
                InitSceneRoot();
            }
            entity.transform.parent = m_objRoot.transform;

            entity.Visible = m_bSceneVisible;
        }
        private void RemoveEntity()
        {

        }
        private void ClearEntity()
        {

        }
        private StoryEntity GetEntity(int dwID)
        {
            return null;
        }
        public StoryEntity CreateEntity(string szPrefab)
        {
            GameObject pPrefab = Resources.Load(szPrefab) as GameObject;
            if (pPrefab == null)
            {
                Debug.LogWarning("CreateObject Load Failed At Path " + szPrefab);
                return null;
            }
            GameObject pObject = GameObject.Instantiate(pPrefab) as GameObject;
            StoryEntity pEntity = pObject.AddComponent<StoryEntity>();
            return pEntity;
        }
        public StoryEntity CreateActor(string szSkleton, int dwID)
        {
            StoryEntity objActor = GetEntity(dwID);
            if (objActor == null)
            {
                objActor = CreateEntity(szSkleton);
                objActor.id = dwID;
                this.AddEntity(objActor);
            }
            return objActor;
        }
    }
}
