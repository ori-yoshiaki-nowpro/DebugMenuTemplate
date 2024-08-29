using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    public class CustomDropDown : Dropdown
    {
        protected class CustomDropdownItem : DropdownItem
        {
            private Color m_color;

            public Action<CustomDropdownItem> onPointer;

            public void Init()
            {
                if (image != null)
                {
                    m_color = image.color;
                }
            }

            public void Set(bool isSet)
            {
                if(image != null)
                {
                    image.enabled = isSet;
                    var setColor = image.color;
                    if (isSet)
                    {
                        setColor = Color.yellow;
                        setColor.a = m_color.a;
                    }
                    else
                    {
                        setColor = m_color;
                    }
                    image.color = setColor;
                }
            }

            public override void OnPointerEnter(PointerEventData eventData)
            {
                base.OnPointerEnter(eventData);
                onPointer?.Invoke(this);
            }

            public override void OnCancel(BaseEventData eventData)
            {
                base.OnCancel(eventData);
            }
        }

        private UnityEvent m_eventCreateDropDownList = new UnityEvent();
        private UnityEvent m_eventDeleteDropDownList = new UnityEvent();

        private List<CustomDropdownItem> m_itemList = new List<CustomDropdownItem>();

        private CanvasGroup m_canvasGroupItemList;

        private int m_selectCursorIndex = 0;

        private float m_prevAlpha = 0;

        public Action onOpen = null;
        public Action onClose = null;

        /// <summary>ドロップダウンリストを表示しているか</summary>
        public bool IsOpenList { get; private set; } = false;
        /// <summary>ドロップダウンリストの項目数</summary>
        public int ItemCount => m_itemList.Count;
        /// <summary>カーソル選択中の項目番号</summary>
        public int SelectIndex => m_selectCursorIndex;
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

        public void Update()
        {
            if(m_canvasGroupItemList != null)
            {
                var alpha = m_canvasGroupItemList.alpha;
                if (m_prevAlpha != 0 && m_prevAlpha != 1)
                {
                    if(alpha == 0)
                    {
                        onClose?.Invoke();
                    }
                    else if(alpha == 1)
                    {
                        onOpen?.Invoke();
                    }
                }
                m_prevAlpha = alpha;
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
            m_eventDeleteDropDownList?.Invoke();
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
            cItem.Init();

            cItem.onPointer = (_) =>
            {
                UpdateSelectCursor(_);
            };

            m_itemList.Add(cItem);

            return item;
        }

        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            var blocker = base.CreateBlocker(rootCanvas);

            IsOpenList = true;//表示処理(Show)の最後に呼び出されるので、ここで表示フラグをオンにしておく
            m_eventCreateDropDownList?.Invoke();

            InitializeSelect();

            return blocker;
        }

        /// <summary>
        /// ドロップダウンリスト表示時のコールバックの登録
        /// </summary>
        /// <param name="action"></param>
        public void RegistOpenDropDownListAction(UnityAction action)
        {
            m_eventCreateDropDownList.AddListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト表示時のコールバックの解除
        /// </summary>
        /// <param name="action"></param>
        public void UnRegistOpenDropDownListAction(UnityAction action)
        {
            m_eventCreateDropDownList.RemoveListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト非表示時のコールバックの登録
        /// </summary>
        /// <param name="action"></param>
        public void RegistCloseDropDownListAction(UnityAction action)
        {
            m_eventDeleteDropDownList.AddListener(action);
        }

        /// <summary>
        /// ドロップダウンリスト非表示時のコールバックの解除
        /// </summary>
        /// <param name="action"></param>
        public void UnRegistCloseDropDownListAction(UnityAction action)
        {
            m_eventDeleteDropDownList.RemoveListener(action);
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
                    m_selectCursorIndex = i;
                    item.Set(true);
                    break;
                }
            }
        }

        /// <summary>
        /// カーソル位置更新
        /// </summary>
        /// <param name="isNext"></param>
        public void UpdateSelectCursor(bool isNext)
        {
            int index = m_selectCursorIndex;
            if (isNext)
            {
                index = m_selectCursorIndex + 1;
                if (index >= m_itemList.Count)
                {
                    index = 0;
                }
            }
            else
            {
                index = m_selectCursorIndex - 1;
                if (index < 0)
                {
                    index = m_itemList.Count - 1;
                }
            }
            UpdateSelectCursor(index);
        }
        protected void UpdateSelectCursor(CustomDropdownItem item)
        {
            int index = m_itemList.FindIndex(_ => _ == item);
            if (index == -1) return;

            UpdateSelectCursor(index);
        }
        private void UpdateSelectCursor(int index)
        {
            //前回の選択項目を非選択に
            if (m_selectCursorIndex != -1)
            {
                m_itemList[m_selectCursorIndex].Set(false);
            }
            //今回選択した項目を選択状態に
            var selectItem = m_itemList[index];
            selectItem.Set(true);

            //InputManager登録のキーで操作された場合に、選択中の項目が正しく更新されない場合があるので手動で更新しておく
            var sys = EventSystem.current;
            if (sys != null)
            {
                sys.SetSelectedGameObject(selectItem.gameObject);   
            }

            m_selectCursorIndex = index;
        }
    }
}