using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public sealed class EditorWindowTest : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.Label("Example Editor Window");

            if (GUILayout.Button("Close"))
                Close();
        }

        [MenuItem("EditorWindowTest/OpenWindow")]
        public static void OpenWindow()
        {
            var window = GetWindow<EditorWindowTest>();
            window.Show();
        }

        [MenuItem("EditorWindowTest/Show Modal")]
        private static void Modal()
        {
            var window = CreateInstance<EditorWindowTest>();
            window.ShowModal();
        }

        [MenuItem("EditorWindowTest/Show Utility")]
        private static void Utility()
        {
            var window = CreateInstance<EditorWindowTest>();
            window.ShowUtility();
        }

        [MenuItem("EditorWindowTest/Show AUXWindow")]
        private static void AUXWindow()
        {
            var window = CreateInstance<EditorWindowTest>();
            window.ShowAuxWindow();
        }
    }
}
