using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugMenu;
using UnityEngine;

public class DebugMenuTest : DefaultDebugPage
{
    public override string PageTitle => "デバッグメニューテスト";

    public Action<float> onSlider = null;

    protected List<string> dropdownListItems = new List<string>()
    {
        "アイテム1",
        "アイテム2",
        "アイテム3",
        "アイテム4",
        "アイテム5"
    };

    protected override void InitPageLayouts()
    {
        AddSlider("球の速度", 0, 5, 1, false, (_value) =>
        {
            onSlider?.Invoke(_value);
        });

        AddButton("ボタン",()=> { Debug.Log("ボタン押下"); });

        AddToggle("トグル", false,(_flg)=> { Debug.Log($"トグル:{_flg}"); });

        AddCounter("カウンタ", 0, 0, 10, 1, (_value) => { UnityEngine.Debug.Log($"count:{_value}"); }, null);

        AddInputField("インプット", (_str) => { UnityEngine.Debug.Log($"入力:{_str}"); },UnityEngine.UI.InputField.CharacterValidation.None);

        AddTextBox("テキスト");

        AddDropdown(dropdownListItems, 0, (_index) =>
        {
            string item = dropdownListItems[_index];
            UnityEngine.Debug.Log($"選択したアイテム:{item}");
        });

        AddPageLinkButton<DebugMenuTest2>("メニュー2へ");

    }

    protected override void _OpenMenu()
    {
        base._OpenMenu();
    }
}
