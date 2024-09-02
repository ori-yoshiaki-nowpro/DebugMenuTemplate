using System.Collections;
using UnityEngine;

namespace DebugMenu
{
    /// <summary>
    /// デフォルト設定時に読み込む各メニュー項目のプレハブ名の定義
    /// </summary>
    public static class DefaultListItemAssetKeys
    {
        public static readonly string ButtonListItem = "DebugListItemButton";
        public static readonly string CounterListItem = "DebugListItemCounter";
        public static readonly string DropdownListItem = "DebugListItemDropdown";
        public static readonly string InputFieldListItem = "DebugListItemInputField";
        public static readonly string SliderListItem = "DebugListItemSlider";
        public static readonly string TextBoxListItem = "DebugListItemTextBox";
        public static readonly string ToggleListItem = "DebugListItemToggle";
        public static readonly string PageLinkItem = "DebugListItemPageLink";
    }
}