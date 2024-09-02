using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    public class DebugCustomDropDown : Dropdown
    {
        /// <summary>
        /// ドロップダウンアイテム
        /// </summary>
        protected class CustomDropdownItem : DropdownItem
        {
            /// <summary>表示カラー</summary>
            private Color m_initColor;
            /// <summary></summary>
            private  UnityAction<CustomDropdownItem> m_onPointerEnterAction;

            public void Initialize(UnityAction<CustomDropdownItem> onPointerEnter = null)
            {
                m_onPointerEnterAction = onPointerEnter;

                if (image != null)
                {
                    m_initColor = image.color;
                }
            }

            /// <summary>
            /// 表示更新
            /// </summary>
            /// <param name="isSelect">選択状態にするか</param>
            public void UpdateView(bool isSelect)
            {
                if(image != null)
                {
                    image.enabled = isSelect;
                    //フラグに応じて表示色を変える
                    var setColor = image.color;
                    if (isSelect)
                    {
                        setColor = Color.yellow;
                        setColor.a = m_initColor.a;
                    }
                    else
                    {
                        setColor = m_initColor;
                    }
                    image.color = setColor;
                }
            }

            public override void OnPointerEnter(PointerEventData eventData)
            {
                base.OnPointerEnter(eventData);
                m_onPointerEnterAction?.Invoke(this);
            }
        }

        /// <summary>ドロップダウンリスト表示時のコールバックイベント</summary>
        private UnityEvent m_openDropDownListEvent = new UnityEvent();
        /// <summary>ドロップダウンリストを閉じた際のコールバックイベント</summary>
        private UnityEvent m_closeDropDownListEvent = new UnityEvent();
        /// <summary>表示しているドロップダウンリスト内のアイテム</summary>
        private List<CustomDropdownItem> m_itemList = new List<CustomDropdownItem>();
        private CanvasGroup m_canvasGroupItemList;
        /// <summary>選択中のドロップダウンアイテムの番号</summary>
        private int m_selectListItemIndex = 0;
       
        /// <summary>ドロップダウンリストを表示しているか</summary>
        public bool IsOpenList { get; private set; } = false;
        /// <summary>ドロップダウンリストの項目数</summary>
        public int ItemCount => m_itemList.Count;
        /// <summary>選択中のドロップダウンアイテムの番号</summary>
        public int SelectIndex => m_selectListItemIndex;
        /// <summary>表示・非表示時のフェード中か</summary>
        public bool IsPlayingFadeAnimation
        {
            get
            {
                if(m_canvasGroupItemList != null)
                {
                    return m_canvasGroupItemList.alpha > 0 &&
                           m_canvasGroupItemList.alpha < 1;
                }
                return false;
            }
        }

        /// <summary>
        /// ドロップダウンリスト表示時
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        protected override GameObject CreateDropdownList(GameObject template)
        {
            var createList = base.CreateDropdownList(template);
            if(createList != null)
            {
                m_canvasGroupItemList = createList.GetComponent<CanvasGroup>();
            }

            return createList;
        }
        /// <summary>
        /// ドロップダウンリスト非表示時
        /// </summary>
        /// <param name="dropdownList"></param>
        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            IsOpenList = false;
            m_closeDropDownListEvent?.Invoke();
            m_itemList.Clear();
        }
        /// <summary>
        /// ドロップダウンアイテム生成
        /// </summary>
        /// <param name="itemTemplate"></param>
        /// <returns></returns>
        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            //一旦通常のドロップダウンアイテムを生成して、各参照オブジェクトの情報を取得
            var item = base.CreateItem(itemTemplate);
            var obj = item.gameObject;   
            var text = item.text;
            var image = item.image;
            var toggle = item.toggle;
            var rect = item.rectTransform;
            Destroy(item);//DropItem削除
            //Custom版生成
            var cItem = obj.AddComponent<CustomDropdownItem>();
            cItem.text = text;
            cItem.image = image;
            cItem.toggle = toggle;
            cItem.rectTransform = rect;
            cItem.Initialize((_) =>
            {
                UpdateSelectListItemIndex(_);
            });

            m_itemList.Add(cItem);

            return item;
        }

        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            var blocker = base.CreateBlocker(rootCanvas);

            IsOpenList = true;//表示処理(Show)の最後に呼び出されるので、ここで表示フラグをオンにしておく
            m_openDropDownListEvent?.Invoke();

            InitializeSelect();

            return blocker;
        }

        /// <summary>
        /// ドロップダウンリスト表示時のコールバックの登録
        /// </summary>
        /// <param name="action"></param>
        public void RegistOpenDropDownListAction(UnityAction action)
        {
            m_openDropDownListEvent.AddListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト表示時のコールバックの解除
        /// </summary>
        /// <param name="action"></param>
        public void UnRegistOpenDropDownListAction(UnityAction action)
        {
            m_openDropDownListEvent.RemoveListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト非表示時のコールバックの登録
        /// </summary>
        /// <param name="action"></param>
        public void RegistCloseDropDownListAction(UnityAction action)
        {
            m_closeDropDownListEvent.AddListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト非表示時のコールバックの解除
        /// </summary>
        /// <param name="action"></param>
        public void UnRegistCloseDropDownListAction(UnityAction action)
        {
            m_closeDropDownListEvent.RemoveListener(action);
        }

        /// <summary>
        /// 初期選択カーソル設定処理
        /// </summary>
        protected void InitializeSelect()
        {
            for(int i = 0; i < m_itemList.Count; i++)
            {
                //Toggleが選択状態になっているアイテムを選択中番号にする
                var item = m_itemList[i];
                if(item != null && item.toggle != null && item.toggle.isOn)
                {
                    m_selectListItemIndex = i;
                    item.UpdateView(true);
                    break;
                }
            }
        }

        /// <summary>
        /// 選択中ドロップダウンアイテム番号の更新
        /// </summary>
        /// <param name="isNext"></param>
        public void UpdateSelectListItemIndex(bool isNext)
        {
            int index = m_selectListItemIndex;
            if (isNext)
            {
                index = m_selectListItemIndex + 1;
                if (index >= m_itemList.Count)
                {
                    index = 0;
                }
            }
            else
            {
                index = m_selectListItemIndex - 1;
                if (index < 0)
                {
                    index = m_itemList.Count - 1;
                }
            }
            UpdateSelectListItemIndex(index);
        }
        protected void UpdateSelectListItemIndex(CustomDropdownItem item)
        {
            int index = m_itemList.FindIndex(_ => _ == item);
            if (index == -1) return;

            UpdateSelectListItemIndex(index);
        }
        private void UpdateSelectListItemIndex(int index)
        {
            //前回の選択項目を非選択に
            if (m_selectListItemIndex != -1)
            {
                m_itemList[m_selectListItemIndex].UpdateView(false);
            }
            //今回選択した項目を選択状態に
            var selectItem = m_itemList[index];
            selectItem.UpdateView(true);

            //InputManager登録のキーで操作された場合に、選択中の項目が正しく更新されない場合があるので手動で更新しておく
            var sys = EventSystem.current;
            if (sys != null)
            {
                sys.SetSelectedGameObject(selectItem.gameObject);   
            }

            m_selectListItemIndex = index;
        }
    }
}