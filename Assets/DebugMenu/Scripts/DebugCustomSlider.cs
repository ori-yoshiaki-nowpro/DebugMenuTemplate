using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugMenu
{
    public class DebugCustomSlider : Slider
    {
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            //EventSystem側で選択状態になっていると、キー入力操作時に意図しない形で操作が入る場合がある(InputManager側の入力が受け付けられる)ので、選択状態を切っておく
            var sys = EventSystem.current;
            if (sys != null && sys.currentSelectedGameObject == gameObject)
            {
                sys.SetSelectedGameObject(null);
            }
        }
    }
}