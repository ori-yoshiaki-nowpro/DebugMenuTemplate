using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace DebugMenu
{
    public abstract class DebugPageBase : MonoBehaviour
    {
        /// <summary>
        /// 生成したリストアイテムの情報
        /// </summary>
        public sealed class ListItemInfo
        {
            public readonly GameObject listItemObj;
            public readonly IListEvent listItemEvent;
            public readonly ListItemDataBase data;

            public ListItemInfo(GameObject obj,IListEvent listEvent,ListItemDataBase setData)
            {
                listItemObj = obj;
                listItemEvent = listEvent;
                data = setData;
            }
        }

        /// <summary>リストアイテムのプレハブ一覧</summary>
        protected ListItemPrefabReference m_listItemReferences;
        /// <summary>スクロール画面</summary>
        protected ScrollRect m_scrollView;
        /// <summary>スクロールレイアウト</summary>
        protected LayoutGroup m_scrollLayout;
        /// <summary>ページを配置しているデバッグ画面の参照先</summary>
        protected DebugMenuWindow m_targetWindow = null;
        /// <summary>各リストアイテムのオブジェクトプール</summary>
        private Dictionary<string, ObjectPool<GameObject>> m_prefabPoolDic 
            = new Dictionary<string, ObjectPool<GameObject>>();
        /// <summary>生成済みリストアイテム</summary>
        protected List<ListItemInfo> m_itemInfoList = new List<ListItemInfo>();
        /// <summary>選択中リストアイテムの番号</summary>
        protected int m_selectIndex = 0;
        /// <summary>ページ識別ID</summary>
        public string PageID { get; private set; } = string.Empty;
        /// <summary>ページタイトル</summary>
        public abstract string PageTitle { get; }
        public DebugMenuWindow Window => m_targetWindow;
        /// <summary>表示中の項目リストの取得</summary>
        protected List<GameObject> MenuItemList
        {
            get
            {
                List<GameObject> list = new List<GameObject>();
                foreach(var itemData in m_itemInfoList)
                {
                    list.Add(itemData.listItemObj);
                }
                return list;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="owner"></param>
        public void Initialize(DebugMenuWindow owner)
        {
            m_targetWindow = owner;

            SetReferences();
            InitPageID();
            InitPageLayouts();
        }

        /// <summary>
        /// ページID初期化
        /// </summary>
        protected void InitPageID()
        {
            //未設定の場合のみ実行
            if (string.IsNullOrEmpty(PageID))
            {
                PageID = Guid.NewGuid().ToString();
            }
        }
        /// <summary>
        /// 各変数の参照を設定
        /// </summary>
        protected virtual void SetReferences()
        {
            if(m_listItemReferences == null)
            {
                m_listItemReferences = gameObject.GetComponent<ListItemPrefabReference>();
            }
            if(m_scrollView == null)
            {
                m_scrollView = gameObject.GetComponentInChildren<ScrollRect>();
            }
            if(m_scrollLayout == null)
            {
                m_scrollLayout = gameObject.GetComponentInChildren<LayoutGroup>();
            }
        }
        /// <summary>
        /// ページ内のレイアウトの初期化
        /// </summary>
        protected virtual void InitPageLayouts()
        {
        }
        /// <summary>
        /// ページ表示
        /// </summary>
        public void OpenMenu()
        {
            if (m_targetWindow == null) return;
            _OpenMenu();

            SetSelectIndex(0);//一番上を選択
            gameObject.SetActive(true);
        }
        protected virtual void _OpenMenu()
        {
        }
        /// <summary>
        /// ページ閉じる
        /// </summary>
        public void CloseMenu()
        {
            _CloseMenu();
            gameObject.SetActive(false);
        }
        protected virtual void _CloseMenu()
        {       
        }

        /// <summary>
        /// デバッグメニュー項目の追加
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="setData"></param>
        /// <returns></returns>
        public int AddListItem(string prefabName, ListItemDataBase setData)
        {
            var rootParent = m_scrollView?.content ?? null;
            if (rootParent == null)
            {
                //生成先がnull
                return -1;
            }

            if (!m_prefabPoolDic.ContainsKey(prefabName))
            {
                m_prefabPoolDic.Add(prefabName, new ObjectPool<GameObject>(() =>
                {
                    var getPrefab = m_listItemReferences.GetPrefab(prefabName);
                    if (getPrefab == null)
                    {
                        //プレハブ見つからず
                        return null;
                    }

                    return Instantiate(getPrefab, rootParent);
                }));
            }

            var obj = m_prefabPoolDic[prefabName].Get();
            if(obj == null)
            {
                return -1;
            }

            var comp = obj.GetComponent<IListEvent>();
            if(comp == null)
            {
                //IListEvent見つからず
                return -1;
            }

            setData.owner = this;
            setData.onEnterPointer = (_obj) =>
            {
                int index = m_itemInfoList.FindIndex(_ => _.listItemObj == _obj);
                if(index != -1)
                {
                    SetSelectIndex(index);
                }
            };
            comp.Setup(setData);

            ListItemInfo info = new ListItemInfo(obj,comp,setData);
            m_itemInfoList.Add(info);

            int index = m_itemInfoList.Count - 1;
            return index;
        }

        /// <summary>
        /// キー入力結果の反映
        /// </summary>
        /// <param name="info"></param>
        public bool OnInputKeyStroke(DebugMenuWindow.KeystrokeInfoType info)
        {
            //選択中の項目に対してのキー入力判定
            var curSelectListItem = m_itemInfoList[m_selectIndex];
            bool isInputRef = false;
            if(curSelectListItem != null)
            {
                isInputRef = curSelectListItem.listItemEvent.OnInputKey(info);
            }
            //項目側でキー入力による操作が行われなかった場合は、メニュー画面に対してキー入力判定を行う
            if (!isInputRef)
            {
                //Debug.Log("メニュー画面側のキー入力判定");
                if (info == DebugMenuWindow.KeystrokeInfoType.Dir_Up)
                {
                    if (m_itemInfoList.Count > 1)
                    {
                        int index = 0;
                        if (m_selectIndex == 0)
                        {
                            index = m_itemInfoList.Count - 1;
                        }
                        else
                        {
                            index = m_selectIndex - 1;
                        }
                        SetSelectIndex(index);
                    }
                }
                else if (info == DebugMenuWindow.KeystrokeInfoType.Dir_Down)
                {
                    if (m_itemInfoList.Count > 1)
                    {
                        int index = 0;
                        if (m_selectIndex == m_itemInfoList.Count - 1)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = m_selectIndex + 1;
                        }
                        SetSelectIndex(index);
                    }
                }
            }

            return isInputRef;
        }
        
        /// <summary>
        /// 選択中のメニュー項目番号の更新
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectIndex(int index)
        {
            if (m_itemInfoList.Count <= index) return;

            int tempSelectIndex = m_selectIndex;
            m_selectIndex = index;
            //前回選択していた項目を非選択状態に
            if(tempSelectIndex != index)
            {
                var prevSelectListItem = m_itemInfoList[tempSelectIndex];
                prevSelectListItem.listItemEvent.OnDeselect();
            }
            //今回選択した項目を選択状態に
            var curSelectListItem = m_itemInfoList[m_selectIndex];
            curSelectListItem.listItemEvent.OnSelect();
            //スクロール位置更新
            m_scrollView.UpdateScrollVertical(m_scrollLayout, MenuItemList, m_selectIndex, tempSelectIndex);
        }
    }
}
