using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DebugMenu;

namespace DebugMenu.Test
{
    public class TestSceneMain : MonoBehaviour
    {
        [SerializeField] private MeshRenderer sphere;

        /// <summary>ãÖÇÃà⁄ìÆë¨ìx</summary>
        private float speed = 1f;
        private Vector2 counter = Vector2.zero;

        public void Start()
        {
            var debugWindow = DebugMenuWindow.StaticInstance;
            var page = debugWindow.Initialize<DebugMenuTest>();
            page.onSlider = (_value) =>
            {
                speed = _value;
            };
        }

        public void Update()
        {
            var debugWindow = DebugMenuWindow.StaticInstance;

            //ÉLÅ[ì¸óÕ
            if (debugWindow != null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (!debugWindow.IsOpenWindow)
                    {
                        debugWindow.OpenWindow();
                    }
                    else
                    {
                        debugWindow.CloseWindow();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Enter);
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Cancel);
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.W))
                        {
                            debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Up);
                        }
                        else if (Input.GetKeyDown(KeyCode.S))
                        {
                            debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Down);
                        }
                        else if (Input.GetKeyDown(KeyCode.A))
                        {
                            debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Left);
                        }
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            debugWindow.ReceiveKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Right);
                        }
                    }
                }
            }

            //ãÖÇÆÇÈÇÆÇÈ
            counter.x += Time.deltaTime * speed;
            counter.y += Time.deltaTime * speed;
            var x = Mathf.Sin(counter.x) * 2;
            var z = Mathf.Cos(counter.y) * 2;
            var pos = sphere.transform.position;
            pos.x = x;
            pos.z = z;
            sphere.transform.position = pos;
        }
    }
}


