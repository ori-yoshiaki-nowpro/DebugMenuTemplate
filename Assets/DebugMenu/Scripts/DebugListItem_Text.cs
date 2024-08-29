using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    /// <summary>
    /// デバッグメニュー項目：テキスト表示
    /// </summary>
    public class DebugListItem_Text : DebugListItemBase<TextData>
    {
        [SerializeField]
        protected Image m_imageBg;
        [SerializeField]
        protected Text m_text;

        protected override Graphic GraphBg => m_imageBg;

        public void SetText(string text)
        {
            if (m_text != null)
            {
                m_text.text = text;
            }
        }

        protected override void Initialize(TextData data)
        {
            SetText(data.text);
        }
    }

    public sealed class TextData : ListItemDataBase
    {
        public string text;
    }
}