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
    /// デバッグメニュー項目：トグル
    /// </summary>
    public class DebugListItem_Toggle : DebugListItemBase<ToggleData>
    {
        [SerializeField]
        protected Text m_text;
        [SerializeField]
        protected Toggle m_toggle;
        /// <summary>トグル設定変更時のコールバック</summary>
        protected Action<bool> m_didTap;

        protected override Graphic GraphBg
        {
            get
            {
                return m_toggle.targetGraphic;
            }
        }

        protected override void Initialize(ToggleData data)
        {
            m_didTap = data.didTap;

            if (m_text != null)
            {
                m_text.text = data.text;
            }

            if (m_toggle != null)
            {
                m_toggle.onValueChanged.AddListener((_isOn) =>
                {
                    m_didTap?.Invoke(_isOn);
                });
                m_toggle.SetIsOnWithoutNotify(data.isOn);
                m_toggle.group = data.group;
            }
        }

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
                case DebugMenuWindow.KeystrokeInfoType.Enter:
                    m_toggle.isOn = !m_toggle.isOn;//トグル選択状態反転
                    break;
            }

            return false;
        }
    }

    public sealed class ToggleData : ListItemDataBase
    {
        public string text;
        public bool isOn;
        public Action<bool> didTap;
        public ToggleGroup group;
    }
}
