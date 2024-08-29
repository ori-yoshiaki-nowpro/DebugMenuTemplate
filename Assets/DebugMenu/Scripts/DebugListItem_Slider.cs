using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugListItem_Slider : DebugListItemBase<SliderData>
    {
        /// <summary></summary>
        private readonly float ratePercent = 10f;

        [SerializeField]
        protected Image m_imageBg;
        [SerializeField]
        protected Slider m_slider;
        [SerializeField]
        private Text m_textTitle;
        [SerializeField]
        private Text m_textValue;

        protected Action<float> m_didChangeValue;

        protected override Graphic GraphBg
        {
            get
            {
                return m_imageBg;
            }
        }

        protected override void Initialize(SliderData data)
        {
            m_didChangeValue = data.didChangeValue;

            m_textTitle.text = data.title;

            m_slider.wholeNumbers = data.wholeNumber;
            m_slider.minValue = data.min;
            m_slider.maxValue = data.max;
            m_slider.value = data.value;
            m_slider.onValueChanged.AddListener((_value) =>
            {
                SetValueText(_value);
                m_didChangeValue?.Invoke(_value);
                
            });
            SetValueText(data.value);
        }

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
                case DebugMenuWindow.KeystrokeInfoType.Enter:                    
                    //return true;
                case DebugMenuWindow.KeystrokeInfoType.Dir_Left:
                    KeyinputVal(false);
                    return true;
                case DebugMenuWindow.KeystrokeInfoType.Dir_Right:
                    KeyinputVal(true);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// キー入力によるスライダー操作
        /// </summary>
        /// <param name="isUp"></param>
        private void KeyinputVal(bool isUp)
        {
            float diff = m_slider.maxValue - m_slider.minValue;
            float val = diff * ratePercent * 0.01f;
            if (!isUp) val = -val;
            m_slider.value += val;
        }
        
        /// <summary>
        /// 現在値テキストの設定
        /// </summary>
        /// <param name="value"></param>
        private void SetValueText(float value)
        {
            m_textValue.text = $"{value}";
        }

    }

    public sealed class SliderData : ListItemDataBase
    {
        public string title;
        public float min;
        public float max;
        public float value;
        public bool wholeNumber;
        public Action<float> didChangeValue;
    }
}
