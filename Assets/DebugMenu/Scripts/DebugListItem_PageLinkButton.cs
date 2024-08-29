using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    /// <summary>
    /// デバッグメニュー項目:ページ遷移ボタン
    /// </summary>
    public class DebugListItem_PageLinkButton : DebugListItemBase<PageLinkData>
    {
        [SerializeField] protected Text m_text;
        [SerializeField] protected Button m_button;

        private Action m_didTap;

        protected override Graphic GraphBg
        {
            get
            {
                return m_button?.targetGraphic ?? null;
            }
        }

        public void Initialize(string text, Action didTap)
        {
            SetText(text);

            m_didTap = didTap;
            if (m_button != null)
            {
                m_button.onClick.AddListener(() =>
                {
                    m_didTap?.Invoke();
                });
            }
        }

        public void SetText(string text)
        {
            if (m_text != null)
            {
                m_text.text = text;
            }
        }

        public override void OnSelect()
        {
            var graph = m_button.targetGraphic;
            if (graph != null)
            {
                var setColor = DefaultSelectedColor;
                setColor.a = m_initColor.a;
                graph.color = setColor;
            }
        }

        public override void OnDeselect()
        {
            var graph = m_button.targetGraphic;
            if (graph != null)
            {
                graph.color = m_initColor;
            }
        }

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
                case DebugMenuWindow.KeystrokeInfoType.Enter:
                    m_button.onClick.Invoke();
                    return true;
            }
            return false;
        }

        protected override void Initialize(PageLinkData data)
        {
            SetText(data.text);

            if (m_button != null)
            {
                m_button.onClick.AddListener(() =>
                {
                    var window = m_pageOwner?.Window;
                    if(window != null)
                    {
                        window.PushPage(data.pageType, true);
                    }
                });
            }
        }
    }

    public sealed class PageLinkData : ListItemDataBase
    {
        public string text;
        /// <summary>遷移先ページタイプ</summary>
        public Type pageType;
    }
}