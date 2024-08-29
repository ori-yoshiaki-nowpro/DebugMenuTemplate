using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace DebugMenu
{
    /// <summary>
    /// ページオブジェクトのデフォルトクラス
    /// 各ページ内項目を生成する処理を定義
    /// </summary>
    public abstract class DefaultDebugPage : DebugPageBase
    {
        public void AddButton(string text, Action didTap = null)
        {
            ButtonData data = new ButtonData()
            {
                text = text,
                didTap = didTap,
            };

            AddListItem(DefaultListItemAssetKeys.ButtonListItem, data);
        }

        public void AddToggle(string text, bool isOn, Action<bool> didTap = null, ToggleGroup group = null)
        {
            ToggleData data = new ToggleData()
            {
                text = text,
                isOn = isOn,
                didTap = didTap,
                group = group,
            };
            AddListItem(DefaultListItemAssetKeys.ToggleListItem, data);
        }

        public void AddDropdown(List<string> option, int startIndex, Action<int> didSelect)
        {
            DropDownData data = new DropDownData()
            {
                option = option,
                didSelect = didSelect,
                startIndex = startIndex,
            };
            AddListItem(DefaultListItemAssetKeys.DropdownListItem, data);
        }

        public void AddInputField(string title, Action<string> onEdit, InputField.CharacterValidation validation)
        {
            InputFieldData data = new InputFieldData()
            {
                title = title,
                onEndEdit = onEdit,
                validation = validation,
            };
            AddListItem(DefaultListItemAssetKeys.InputFieldListItem, data);
        }

        public void AddSlider(string title, float min, float max, float value, bool wholeNumber, Action<float> changeValue)
        {
            SliderData data = new SliderData()
            {
                title = title,
                min = min,
                max = max,
                value = value,
                wholeNumber = wholeNumber,
                didChangeValue = changeValue,
            };
            AddListItem(DefaultListItemAssetKeys.SliderListItem, data);
        }

        public void AddCounter(string text, float rate, Func<float> didTapRight, Func<float> didTapLeft, Action<float> afterDidTap = null, Action<float> didTapCenter = null)
        {
            CounterData data = new CounterData()
            {
                text = text,
                initValue = rate,
                didTapRight = didTapRight,
                didTapCenter = didTapCenter,
                didTapLeft = didTapLeft,
                afterDidTap = afterDidTap,
            };
            AddListItem(DefaultListItemAssetKeys.CounterListItem, data);
        }
        public void AddCounter(string text,float value,float min,float max,float fluctuationValue,Action<float> afterTap,Action<float> didTapCenter)
        {
            CounterData data = new CounterData()
            {
                text = text,
                initValue = value,
                minValue = min,
                maxValue = max,
                changeValue = fluctuationValue,
                afterDidTap = afterTap,
                didTapCenter = didTapCenter,
            };
            AddListItem(DefaultListItemAssetKeys.CounterListItem, data);
        }

        public void AddPageLinkButton<TPage>(string text)where TPage : DebugPageBase
        {
            PageLinkData data = new PageLinkData()
            {
                pageType = typeof(TPage),
                text = text,
            };
            AddListItem(DefaultListItemAssetKeys.PageLinkItem, data);
        }

        public void AddTextBox(string text)
        {
            TextData data = new TextData()
            {
                text = text,
            };
            AddListItem(DefaultListItemAssetKeys.TextBoxListItem, data);
        }
    }
}