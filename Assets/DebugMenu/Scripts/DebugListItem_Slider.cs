using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugListItem_Slider : DebugListItemBase<SliderData>
    {
        /// <summary>キー操作でスライダーの値を変更する際の変動幅(百分率)</summary>
        protected readonly float VariationRatePercent = 10f;

        [SerializeField]
        protected Image m_imageBg;
        [SerializeField]
        protected DebugCustomSlider m_slider;
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

            m_textTitle.text = data.titleText;

            m_slider.wholeNumbers = data.wholeNumber;
            m_slider.minValue = data.minValue;
            m_slider.maxValue = data.maxValue;
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
                    UpdateSliderValue(false);
                    return true;
                case DebugMenuWindow.KeystrokeInfoType.Dir_Right:
                    UpdateSliderValue(true);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// スライダーの現在値の更新
        /// </summary>
        /// <param name="isValueUp"></param>
        private void UpdateSliderValue(bool isValueUp)
        {
            float diff = m_slider.maxValue - m_slider.minValue;
            float val = diff * VariationRatePercent * 0.01f;
            if (!isValueUp) val = -val;
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
        public string titleText;
        /// <summary>スライダーの最低値</summary>
        public float minValue;
        /// <summary>スライダーの最大値</summary>
        public float maxValue;
        /// <summary>スライダーの初期値</summary>
        public float value;
        /// <summary>整数値限定か</summary>
        public bool wholeNumber;
        /// <summary>スライダーの値変更時のコールバック</summary>
        public Action<float> didChangeValue;
    }
}
