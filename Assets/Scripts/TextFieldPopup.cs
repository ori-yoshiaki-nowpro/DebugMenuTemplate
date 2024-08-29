#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// エディタ用入力ポップアップ
/// https://light11.hatenadiary.com/entry/2020/02/10/211326
/// </summary>
public class TextFieldPopup : PopupWindowContent
{
    private const float WINDOW_PADDING = 8.0f;

    private string _text;
    private string _message;
    private float _width;
    private Action<string> _changed;
    private Action _closed;
    private GUIStyle _messageLabelStyle;
    private Vector2 _windowSize;
    private bool _didFocus = false;

    public static void Show(Vector2 position, string text, Action<string> changed, Action closed, string message = null, float width = 300)
    {
        var rect = new Rect(position, Vector2.zero);
        var content = new TextFieldPopup(text, changed, closed, message, width);
        PopupWindow.Show(rect, content);
    }

    private TextFieldPopup(string text, Action<string> changed, Action closed, string message = null, float width = 300)
    {
        _message = message;
        _text = text;
        _width = width;
        _changed = changed;
        _closed = closed;

        _messageLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        _messageLabelStyle.wordWrap = true;

        // ウィンドウサイズを計算する
        var labelWidth = width - (WINDOW_PADDING * 2);
        _windowSize = Vector2.zero;
        _windowSize.x = width;
        _windowSize.y += WINDOW_PADDING; // Space
        _windowSize.y += _messageLabelStyle.CalcHeight(new GUIContent(message), labelWidth); // Message
        _windowSize.y += EditorGUIUtility.standardVerticalSpacing; // Space
        _windowSize.y += EditorGUIUtility.singleLineHeight; // TextField
        _windowSize.y += WINDOW_PADDING; // Space
    }

    public override void OnGUI(Rect rect)
    {
        // Enterで閉じる
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
        {
            editorWindow.Close();
        }

        var textFieldName = $"{GetType().Name}{nameof(_text)}";
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(WINDOW_PADDING);
            using (new EditorGUILayout.VerticalScope())
            {
                // タイトルを描画
                EditorGUILayout.LabelField(_message, _messageLabelStyle);
                // TextFieldを描画
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    GUI.SetNextControlName(textFieldName);
                    _text = EditorGUILayout.TextField(_text);
                    if (ccs.changed)
                    {
                        _changed?.Invoke(_text);
                    }
                }
            }
            GUILayout.Space(WINDOW_PADDING);
        }
        // 最初の一回だけ自動的にフォーカスする
        if (!_didFocus)
        {
            GUI.FocusControl(textFieldName);
            _didFocus = true;
        }
    }

    public override void OnClose()
    {
        _closed?.Invoke();
        base.OnClose();
    }

    public override Vector2 GetWindowSize() => _windowSize;
}
#endif