using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugListItem_InputField : DebugListItemBase<InputFieldData>
    {
		[SerializeField]
		protected Image m_imageTitleBg;
		[SerializeField]
		protected Text m_textTitle = null;
		[SerializeField]
		protected InputField m_inputField = null;

		protected Action<string> m_onEndEdit;
		private bool m_endEdit = false;

		public string Text
		{
			get { return m_inputField.text; }
		}

        protected override Graphic GraphBg
        {
            get
            {
				return m_imageTitleBg;
            }
        }

        public void Update()
        {
			//Debug.Log($"フォーカス:{m_inputField.isFocused}");
		}

        public void Initialize(string title, string text = null, Action<string> onEndEdit = null, InputField.CharacterValidation validation = InputField.CharacterValidation.Integer)
		{
			m_textTitle.text = string.Format("{0} : ", title);
			m_inputField.text = text;
			m_inputField.onEndEdit.AddListener(OnEndEdit);
			m_onEndEdit = onEndEdit;
			m_inputField.characterValidation = validation;
		}

		protected override void Initialize(InputFieldData data)
		{
			m_textTitle.text = string.Format("{0} : ", data.title);
			m_inputField.text = data.text;
			m_inputField.onEndEdit.AddListener(OnEndEdit);
			m_onEndEdit = data.onEndEdit;
			m_inputField.characterValidation = data.validation;

			m_inputField.onValueChanged.AddListener((_) =>
			{
				Debug.Log($"ValueChanged:{_}");
			});

			m_inputField.onValidateInput = (_a, _b, _c) =>
			{
				Debug.Log($"ValidateInput: str:{_a} char:{_c}");
				return _c;
			};

			m_inputField.onEndEdit.AddListener((_) =>
			{
				Debug.Log($"EndEdit:{_}");
			});
		}

		public void OpenInputField()
		{
			OnInputField();
		}

		private void OnEndEdit(string value)
		{
			//Debug.Log($"onEndEdit: {value}");
			m_onEndEdit?.Invoke(value);
			m_endEdit = true;
		}

		/// <summary>
		/// 文字数制限を設定
		/// </summary>
		/// <param name="limit"></param>
		public void SetCharaLimit(int limit)
		{
			m_inputField.characterLimit = limit;
		}

		/// <summary>
		/// テキスト設定
		/// </summary>
		/// <param name="text"></param>
		public void SetText(string text)
		{
			if (m_inputField != null)
			{
				m_inputField.SetTextWithoutNotify(text);
			}
		}

		/// 入力欄で決定ボタンを押した時の処理
		/// </summary>
		protected void OnInputField()
		{
#if UNITY_SWITCH
			m_text.Select();
#elif UNITY_PS4 || UNITY_PS5
			m_text.Select();
#elif UNITY_GAMECORE || UNITY_GAMECORE_XBOXONE || UNITY_GAMECORE_SCARLETT
			m_text.Select();
#elif UNITY_STANDALONE || UNITY_EDITOR
#if NP_STEAM

#if false//CUBEの処理のコピペ(独自処理のため封印)
			if (SteamManager.Initialized) 
			{
				if (AppCore.AppCoreManager.LanguageType == Define.ELanguage.English) {
					bool isShowTextInput = SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
						EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
						"",
						12,
						m_text.text);
				}
				else {
					bool isShowTextInput = SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal,
						EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine,
						"",
						8,
						m_text.text);
				}
			}
#endif

#endif
#endif
			m_inputField.ActivateInputField();
		}

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType)
            {
				case DebugMenuWindow.KeystrokeInfoType.Enter:
                    if (!m_inputField.isFocused && !m_endEdit)
                    {
						OnInputField();
						return true;
					}
					break;
            }

			if (m_inputField.isFocused) return true;//文字入力中は他操作を受け付けさせないようにする

			return false;
        }
    }

	public sealed class InputFieldData : ListItemDataBase
	{
		public string title;
		public string text = string.Empty;
		public Action<string> onEndEdit = null;
		public InputField.CharacterValidation validation = InputField.CharacterValidation.Integer;
	}
}
