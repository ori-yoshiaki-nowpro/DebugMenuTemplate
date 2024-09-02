using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DebugMenu
{
    /// <summary>
    /// デバッグメニュー項目：ドロップダウン
    /// </summary>
    public class DebugListItem_DropDown : DebugListItemBase<DropDownData>
    {
        protected Action<int> m_didSelect;
        [SerializeField]
        protected DebugCustomDropDown m_dropDown;

        protected override Graphic GraphBg
        {
            get
            {
                return m_dropDown?.targetGraphic ?? null;
            }
        }

        protected override void Initialize(DropDownData data)
        {
            m_didSelect = data.didSelect;

            var dropdown = m_dropDown;
            dropdown.ClearOptions();
            dropdown.AddOptions(data.option);
            dropdown.value = data.startIndex;
            dropdown.onValueChanged.AddListener((_index) =>
            {
                m_didSelect?.Invoke(_index);
            });

            dropdown.RegistCloseDropDownListAction(() =>
            {
                //EventSystem側でドロップダウンが選択状態になっていると、キー入力操作時に他UIを意図しない形で操作してしまうので解除しておく
                var sys = EventSystem.current;
                if (sys != null && sys.currentSelectedGameObject == m_dropDown.gameObject)
                {
                    sys.SetSelectedGameObject(null);
                }
            });
        }

        protected override bool UpdateInputKey(DebugMenuWindow.KeystrokeInfoType inputType)
        {
            switch (inputType) 
            {
                case DebugMenuWindow.KeystrokeInfoType.Enter:
                    if (m_dropDown.IsOpenList)
                    {
                        /*ドロップダウンリスト表示中の場合*/

                        //InputManager登録のキーで操作した場合、そちらの操作が先に反映されるのでここの判定に入らない場合がある(その場合はonValueChanged側で変更を拾っているはず)
                        //ドロップダウンリストのフェード中は操作不能にする
                        if (!m_dropDown.IsPlayingFadeAnimation)
                        {
                            //カーソル選択中の項目を選択状態にする
                            m_dropDown.value = m_dropDown.SelectIndex;
                            m_dropDown.Hide();//リストは非表示に
                        }
                    }
                    else
                    {
                        //ドロップダウンリスト表示
                        m_dropDown.Show();
                    }
                    return true;
                case DebugMenuWindow.KeystrokeInfoType.Cancel:
                    if (m_dropDown.IsOpenList)
                    {
                        m_dropDown.Hide();
                        return true;
                    }
                    break;
                case DebugMenuWindow.KeystrokeInfoType.Dir_Down:
                    if (m_dropDown.IsOpenList)
                    {
                        m_dropDown.UpdateSelectListItemIndex(true);
                        return true;
                    }
                    break;
                case DebugMenuWindow.KeystrokeInfoType.Dir_Up:
                    if (m_dropDown.IsOpenList)
                    {
                        m_dropDown.UpdateSelectListItemIndex(false);
                        return true;
                    }
                    break;
            }

            return false;
        }
    }

    public sealed class DropDownData:ListItemDataBase
    {
        public List<string> option;
        public int startIndex;
        public Action<int> didSelect;
    }
}
