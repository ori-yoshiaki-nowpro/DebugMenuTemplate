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
	/// デバッグメニュー項目：カウンター
	/// </summary>
    public class DebugListItem_Counter : DebugListItemBase<CounterData>
    {
		private const string c_TitleFormat = "{0} : {1}";

		[SerializeField] protected Text m_textTitle = null;
		[SerializeField] protected Button m_buttonRight;
		[SerializeField] protected Button m_buttonLeft;
		[SerializeField] protected Button m_buttonCenter;

		private string m_title = "";
		private float m_value = 0.0f;

		private Func<float> m_didTapR;
		private Func<float> m_didTapL;
		/// <summary>値変更時のコールバック</summary>
		private Action<float> m_didAfterTap;
		/// <summary>変動値</summary>
		private float m_changeValue;
		/// <summary>最小値</summary>
		private float m_min;
		/// <summary>最大値</summary>
		private float m_max;

        protected override Graphic GraphBg
        {
            get
            {
				return m_buttonCenter?.targetGraphic ?? null;
            }
        }

		protected override void Initialize(CounterData data)
		{
			m_didTapR = data.didTapRight;
			m_didTapL = data.didTapLeft;
			m_didAfterTap = data.afterDidTap;
			m_title = data.text;
			m_value = data.initValue;

			m_changeValue = data.changeValue;
			m_min = data.minValue;
			m_max = data.maxValue;

			UpdateValueText();
			
			m_buttonRight.onClick.RemoveAllListeners();
			m_buttonRight.onClick.AddListener(DidTapRightItem);

			m_buttonLeft.onClick.RemoveAllListeners();
			m_buttonLeft.onClick.AddListener(DidTapLeftItem);
			
			if (data.didTapCenter != null)
			{
				m_buttonCenter.enabled = true;
				m_buttonCenter.onClick.AddListener(() => { data.didTapCenter?.Invoke(m_value); });
			}
			else
			{
				m_buttonCenter.enabled = false;
			}
		}

		/// <summary>
		/// 右側ボタンをタップした時の処理
		/// </summary>
		public void DidTapRightItem()
		{
			//if (m_didTapR != null)
			{
				UpdateValue(m_changeValue);
			}
			m_didAfterTap?.Invoke(m_value);
		}

		/// <summary>
		/// 左側ボタンをタップした時の処理
		/// </summary>
		public void DidTapLeftItem()
		{
			//if (m_didTapL != null)
			{
				UpdateValue(-m_changeValue);
			}
			m_didAfterTap?.Invoke(m_value);
		}

		/// <summary>
		/// 真ん中を押したときの処理
		/// </summary>
		public void DidTapItem()
		{
			var centerButton = m_buttonCenter;
			if (centerButton != null)
			{
				centerButton.onClick?.Invoke();
			}
		}

		/// <summary>
		/// カウント更新
		/// </summary>
		/// <param name="flucValue"></param>
		/// <param name="updateText"></param>
		protected void UpdateValue(float flucValue, bool updateText = true)
        {
			m_value = Mathf.Clamp(m_value + flucValue, m_min, m_max);
			if (updateText) UpdateValueText();
		}

		/// <summary>
		/// テキスト設定
		/// </summary>
		protected void UpdateValueText()
        {
			m_textTitle.text = string.Format(c_TitleFormat, m_title, m_value);
		}

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
				case DebugMenuWindow.KeystrokeInfoType.Dir_Left:
					DidTapLeftItem();
					return true;
				case DebugMenuWindow.KeystrokeInfoType.Dir_Right:
					DidTapRightItem();
					return true;
            }
			return false;
        }
    }

	public sealed class CounterData : ListItemDataBase
    {
		/// <summary></summary>
		public string text;
		/// <summary>初期値</summary>
		public float initValue;
		/// <summary>最低値</summary>
		public float minValue;
		/// <summary>最大値</summary>
		public float maxValue;
		/// <summary>上昇値</summary
		public float changeValue;
		/// <summary>中央ボタン押下時に値を初期値に戻すか</summary>
		public bool isResetValueByCenterBtn;

		public Func<float> didTapRight;
		public Func<float> didTapLeft;
		public Action<float> afterDidTap;
		public Action<float> didTapCenter;
	}
}
