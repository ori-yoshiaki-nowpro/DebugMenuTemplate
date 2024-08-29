using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    /// <summary>
    /// ページ内項目の初期化時に扱うデータのベースクラス
    /// </summary>
    public abstract class ListItemDataBase 
    {
        public DebugPageBase owner;
        public Action<GameObject> onEnterPointer;
    }

    public interface IListEvent
    {
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="data"></param>
        public void Setup(ListItemDataBase data);
        /// <summary>
        /// 選択状態移行時
        /// </summary>
        public void OnSelect();
        /// <summary>
        /// 非選択状態移行時
        /// </summary>
        public void OnDeselect();
        /// <summary>
        /// キー入力操作受付
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool OnInputKey(DebugMenuWindow.KeystrokeInfoType info);
    }

    public abstract class DebugListItemBase<TData> : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IListEvent where TData :ListItemDataBase
    {
        /// <summary>選択状態時の表示カラーのデフォルト値</summary>
        protected readonly Color DefaultSelectedColor = Color.yellow;
        /// <summary></summary>
        protected RectTransform m_rectSelf;
        public RectTransform RectSelf
        {
            get
            {
                if(m_rectSelf == null)
                {
                    m_rectSelf = GetComponent<RectTransform>();
                }
                return m_rectSelf;
            }
        }
        /// <summary>初期表示カラー</summary>
        protected Color m_initColor;
        /// <summary>背景オブジェクト(選択状態切り替わり時に表示カラーの変更対象となるオブジェクト)</summary>
        protected virtual Graphic GraphBg { get; } = null;

        private Action<GameObject> m_onPointerEnterAct = null;
        protected DebugPageBase m_pageOwner;

        public virtual void Awake()
        {
            SetupInitBgColor();
        }

        #region Interface

        public void Setup(ListItemDataBase data)
        {
            m_onPointerEnterAct = data.onEnterPointer;
            m_pageOwner = data.owner;

            var genericData = (TData)data;
            if(genericData != null)
            {
                Initialize(genericData);
            }
            else
            {
                Debug.LogError($"Failed to convert initialization data: TargetDataType:{typeof(TData)}");
            }
        }

        public virtual void OnSelect()
        {
            SetColorBackGround(true);
        }

        public virtual void OnDeselect()
        {
            SetColorBackGround(false);
        }

        public bool OnInputKey(DebugMenuWindow.KeystrokeInfoType info)
        {
            return UpdateInputKey(info);
        }
        
        #endregion

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="data"></param>
        protected abstract void Initialize(TData data);

        /// <summary>
        /// キー入力による操作の反映処理
        /// </summary>
        protected virtual bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            return false;
        }

        /// <summary>
        /// 背景の初期カラー値取得
        /// </summary>
        protected virtual void SetupInitBgColor()
        {
            m_initColor = GraphBg?.color ?? Color.white;
        }

        /// <summary>
        /// 背景オブジェクトのカラー設定
        /// </summary>
        /// <param name="isSelect"></param>
        protected virtual void SetColorBackGround(bool isSelect)
        {
            var bg = GraphBg;
            if(bg != null)
            {
                var setColor = bg.color;
                if (isSelect)
                {
                    setColor = DefaultSelectedColor;
                    setColor.a = m_initColor.a;
                }
                else
                {
                    setColor = m_initColor;
                }
                bg.color = setColor;
            }
        }

        /// <summary>
        /// メニュー表示の高さの設定
        /// LayoutGroup側でサイズ指定されている場合は機能しない点に注意
        /// </summary>
        /// <param name="height"></param>
        public void SetHeight(float height)
        {
            if (RectSelf != null)
            {
                var size = RectSelf.sizeDelta;
                size.y = height;
                RectSelf.sizeDelta = size;
            }
        }

        #region PointerEvents

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_onPointerEnterAct?.Invoke(gameObject);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }

        #endregion 
    }
}
