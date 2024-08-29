using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    /// <summary>
    /// ボタンメニュー
    /// </summary>
    public class DebugListItem_Button : DebugListItemBase<ButtonData>
    {
        [SerializeField] protected Text m_text;
        [SerializeField] protected Button m_button;

        protected override Graphic GraphBg
        {
            get
            {
                return m_button.targetGraphic;
            }
        }

        private Action m_didTap;

        public void Initialize(string text, Action didTap)
        {
            SetText(text);

            m_didTap = didTap;
            if(m_button != null)
            {
                m_button.onClick.AddListener(()=> 
                {
                    m_didTap?.Invoke();
                }); 
            }
        }

        public void SetText(string text)
        {
            if(m_text != null)
            {
                m_text.text = text;
            }
        }

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
                case DebugMenuWindow.KeystrokeInfoType.Enter:
                    m_button.onClick.Invoke();//ボタンクリック時の処理を発火
                    return true;
            }

            return false;
        }

        protected override void Initialize(ButtonData data)
        {
            SetText(data.text);

            m_didTap = data.didTap;
            if (m_button != null)
            {
                m_button.onClick.AddListener(() =>
                {
                    m_didTap?.Invoke();
                });
            }
        }
    }

    public sealed class ButtonData : ListItemDataBase
    {
        public string text;
        public Action didTap;
    }
}
