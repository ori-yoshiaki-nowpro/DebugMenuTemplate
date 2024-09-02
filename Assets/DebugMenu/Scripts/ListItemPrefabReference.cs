using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugMenu
{
    /// <summary>
    /// デバッグメニュー項目のプレハブ参照一覧
    /// </summary>
    public class ListItemPrefabReference : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_prefabs = new List<GameObject>();
        /// <summary>プレハブ名をkeyとしたメニュープレハブのリスト</summary>
        private Dictionary<string, GameObject> m_dicNameToPrefab = new Dictionary<string, GameObject>();

        private bool m_bInitPrefabDic = false;

        private void Awake()
        {
            InitPrefabDictionary();
        }

        private void InitPrefabDictionary()
        {
            if (!m_bInitPrefabDic)
            {
                //参照設定されているプレハブをDictionaryの方に登録
                foreach(var prefab in m_prefabs)
                {
                    AddPrefab(prefab);
                }

                m_bInitPrefabDic = true;
            }
        }

        /// <summary>
        /// プレハブの登録
        /// </summary>
        /// <param name="prefab"></param>
        public void AddPrefab(GameObject prefab)
        {
            if (!m_dicNameToPrefab.ContainsKey(prefab.name))
            {
                m_dicNameToPrefab[prefab.name] = prefab;
            }
        }

        /// <summary>
        /// メニュープレハブの取得
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public GameObject GetPrefab(string prefabName)
        {
            InitPrefabDictionary();
            if(!m_dicNameToPrefab.TryGetValue(prefabName,out var prefab))
            {
                Debug.LogError($"指定名のプレハブが存在しない プレハブ名:{prefabName}");
                return null;
            }
            return prefab;
        }
    }
}