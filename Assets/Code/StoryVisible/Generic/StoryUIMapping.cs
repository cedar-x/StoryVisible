using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace xxstory
{

    public class storyMapData
    {
        public string refName;
        public string signName;
        public bool bFloder;
        public string funcName;
        public string funcImpl;
        public List<StoryUIMapping.funcData> funcs;
    }
    public class StoryUIMapping : MonoBehaviour
    {
        public struct funcData
        {
            public string key;
            public string value;
            public funcData(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }
        public static string GetSubName<T>(GameObject child, GameObject root) where T : Component
        {
            if (child == null || root == null)
            {
                Debug.LogWarning("StoryUIMapping GetSubName child or root is null");
                return string.Empty;
            }
            string subName = child.name;
            Transform parentTrans = child.transform.parent;

            while (parentTrans != null)
            {
                if (parentTrans == root.transform)
                    break;
                var hstjtype = parentTrans.gameObject.GetComponent<T>();
                if (!(hstjtype is T))
                {
                    parentTrans.gameObject.AddComponent<T>();
                }
                subName = parentTrans.gameObject.name + "." + subName;
                parentTrans = parentTrans.parent;
            }
            if (parentTrans == null)
                return string.Empty;
            return subName;
        }
        public class singlePanelInfo
        {
            public GameObject panelRoot;
            public string panelName;
            public bool bFolder;
            private Dictionary<string, storyMapData> _mapDatas = new Dictionary<string, storyMapData>();

            public Dictionary<string, storyMapData> mapDatas
            {
                get
                {
                    return _mapDatas;
                }
            }

            public string GetSubName(GameObject objChild)
            {
                return StoryUIMapping.GetSubName<Transform>(objChild, panelRoot);
            }

            public bool AddSingle(string refName, string signName)
            {
                if (_mapDatas.ContainsKey(refName))
                {
                    Debug.Log("ui mapping already contain key:" + refName);
                    return false;
                }
                storyMapData data = new storyMapData();
                data.funcs = new List<funcData>();
                data.refName = refName;
                data.signName = signName;
                _mapDatas.Add(refName, data);
                return true;
            }
            public void DeleteSingle(string refName)
            {
                storyMapData dataMap = null;
                if (!_mapDatas.TryGetValue(refName, out dataMap))
                {
                    Debug.Log("Delete Single failed, can't find storyMapData:" + refName);
                    return;
                }
                _mapDatas.Remove(refName);
            }
            public void AddFunction(string refName, string key, string value)
            {
                storyMapData dataMap = null;
                if (!_mapDatas.TryGetValue(refName, out dataMap))
                {
                    Debug.Log("Add Function failed, can't find mapdata:" + refName);
                    return;
                }
                foreach (var data in dataMap.funcs)
                {
                    if (data.key == key)
                    {
                        Debug.Log("Add Repeat funcName:" + data.key + ":" + refName);
                        return;
                    }
                }
                dataMap.funcs.Add(new funcData(key, value));
            }
            public void DeleteFunction(string refName, string key)
            {
                storyMapData dataMap = null;
                if (_mapDatas.TryGetValue(refName, out dataMap))
                {
                    foreach (var data in dataMap.funcs)
                    {
                        if (data.key == key)
                        {
                            dataMap.funcs.Remove(data);
                            return;
                        }

                    }
                }
            }
            public void AddAllText()
            {

//                 foreach (var pChild in panelRoot.GetComponentsInChildren<UILabel>())
//                 {
//                     string refName = this.GetSubName(pChild.gameObject);
//                     var hstjtype = pChild.GetComponent<HUIWidget>();
//                     if (hstjtype == null)
//                     {
//                         pChild.gameObject.AddComponent<HUILabel>();
//                     }
//                     else
//                         if (!(hstjtype is HUILabel))
//                         {
//                             Debug.LogWarning("UILabel's LuaObject is not HUILabel. please check.." + refName);
//                         }
//                     string name = pChild.name;
//                     if (this.AddSingle(refName, name))
//                         this.AddFunction(refName, "text", pChild.text);
//                 }
            }
            public bool CheckSignName()
            {
                List<string> checkes = new List<string>();
                foreach (var data in _mapDatas.Values)
                {
                    string signName = data.signName;
                    if(checkes.Contains(signName))
                    {
                        Debug.Log("signName repeated with name:" + signName + ":" + data.refName);
                        return false;
                    }
                    checkes.Add(signName);
                }
                return true;
            }

            public void AddJoson(JsonWriter writer)
            {
                //---------------------start save to json file---------------------------------
                writer.WritePropertyName(panelName);
                writer.WriteObjectStart();
                foreach (var data in _mapDatas.Values)
                {
                    writer.WritePropertyName(data.signName);
                    writer.WriteObjectStart();
                    {
                        writer.WritePropertyName("name");
                        writer.Write(data.refName);
                        foreach (var func in data.funcs)
                        {
                            writer.WritePropertyName(func.key);
                            writer.Write(func.value);
                        }
                    }
                    writer.WriteObjectEnd();
                }
                writer.WriteObjectEnd();
            }
        }

        public Dictionary<GameObject, singlePanelInfo> _panelInfos = new Dictionary<GameObject, singlePanelInfo>();
        public string preDir = "/ScriptConfig/UIReflectConfig/";
        public string saveName;
        public singlePanelInfo Add(GameObject obj)
        {
            singlePanelInfo panelInfo = null;
            if (_panelInfos.TryGetValue(obj, out panelInfo))
            {
                Debug.Log("alreay add object:" + panelInfo.panelRoot.name);
                return panelInfo;
            }
            panelInfo = new singlePanelInfo();
            panelInfo.panelName = obj.name;
            panelInfo.panelRoot = obj;
            _panelInfos.Add(obj, panelInfo);
            return panelInfo;
        }
        public void Delete(GameObject obj)
        {
            singlePanelInfo panelInfo = null;
            if (_panelInfos.TryGetValue(obj, out panelInfo))
            {
                _panelInfos.Remove(obj);
            }
        }


        [ContextMenu("Export to Json")]
        private void ExportToJson()
        {
            //---------------------start save to json file---------------------------------
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            writer.WriteObjectStart();
            foreach (var data in _panelInfos.Values)
            {
                data.AddJoson(writer);
            }
            writer.WriteObjectEnd();
            
            string savePath = Application.streamingAssetsPath + preDir + "jsonUIMap/" + saveName + ".json";
            string exePath = Application.streamingAssetsPath + preDir + "jsonTools/ui_json2lua.py";
            StreamWriter sw = new StreamWriter(savePath);
            sw.Write(sb.ToString());
            sw.Close();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = exePath;
            process.Start();
            Debug.Log("Story Export Success:" + saveName + ".json");
        }

        [ContextMenu("Import to Json")]
        private void ImportToJson()
        {
            string savePath = Application.streamingAssetsPath + preDir + "jsonUIMap/" + saveName + ".json";
            if (!File.Exists(savePath))
            {
                Debug.Log(savePath + " does not exit");
                return;
            }
            using (StreamReader sr = File.OpenText(savePath))
            {
                string text = sr.ReadToEnd();

                JsonData panelMap = JsonMapper.ToObject(text);
                panelMap.ToJson();
                foreach (string panelName in (panelMap as IDictionary).Keys)
                {
                    GameObject obj = GameObject.Find(panelName);
                    singlePanelInfo singleInfo = this.Add(obj);
                    JsonData comMap = panelMap[panelName];
                    foreach (string signName in (comMap as IDictionary).Keys)
                    {
                        JsonData kv = comMap[signName];
                        string refName = kv["name"].ToString();
                        singleInfo.AddSingle(refName, signName);
                        foreach (string key in (kv as IDictionary).Keys)
                        {
                            if(key != "name")
                            {
                                singleInfo.AddFunction(refName,key,kv[key].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
