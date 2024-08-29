using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DebugMenu;

public class TestSceneMain : MonoBehaviour
{
    public enum Menu
    {
        Vertical1,
        Horizontal1,
    }

    [SerializeField] private Dropdown dropDown;
    [SerializeField] private MeshRenderer sphere;
    [SerializeField] private RectTransform m_rectScroll;
    [SerializeField] private ScrollRect m_scrollView;
    [SerializeField] private RectTransform m_rootContent;
    [SerializeField] private ContentSizeFitter m_contentSizeFilter;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Dropdown m_dropdownMenu;

    private List<GameObject> m_contentObjectList = new List<GameObject>();
    private LayoutGroup m_layoutGroup;
    private float speed = 1f;
    private Vector2 counter = Vector2.zero;

    public void Start()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach(Menu val in System.Enum.GetValues(typeof(Menu)))
        {
            options.Add(new Dropdown.OptionData()
            {
                text = val.ToString()
            });
        }
        m_dropdownMenu.options = options;
        dropDown.onValueChanged.AddListener((_index) =>
        {
            string name = dropDown.options[_index].text;
            Menu selectMenuType = (Menu)System.Enum.Parse(typeof(Menu), name);
            SetUpScrollView(selectMenuType);
        });

        var debugWindow = DebugMenuWindow.SingletonInstance;
        var page = debugWindow.Initialize<DebugMenuTest>();
        page.onSlider = (_value) =>
        {
            speed = _value;
        };
    }

    public void Update()
    {
        var debugWindow = DebugMenuWindow.SingletonInstance;

        //ÉLÅ[ì¸óÕ
        if(debugWindow != null)
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
                    debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Enter);
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Cancel);
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Up);
                    }
                    else if (Input.GetKeyDown(KeyCode.S))
                    {
                        debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Down);
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Left);
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        debugWindow.SendKeystrokeInfo(DebugMenuWindow.KeystrokeInfoType.Dir_Right);
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

    public void SetUpScrollView(Menu type)
    {
        m_scrollView.horizontal = type == Menu.Horizontal1;
        m_scrollView.vertical = type == Menu.Vertical1;

        foreach(var obj in m_contentObjectList)
        {
            if(obj != null)
            {
                Destroy(obj);
            }
        }
        m_contentObjectList.Clear();

        if(m_layoutGroup != null)
        {
            DestroyImmediate(m_layoutGroup);
        }

        if (m_scrollView.horizontal)
        {
            var hLayoutGroup = m_rootContent.gameObject.AddComponent<HorizontalLayoutGroup>();
            hLayoutGroup.childControlWidth = false;
            hLayoutGroup.childControlHeight = false;
            hLayoutGroup.spacing = 5f;
            m_layoutGroup = hLayoutGroup;

            m_contentSizeFilter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            m_contentSizeFilter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var tempMin = m_rectScroll.offsetMin;
            var tempMax = m_rectScroll.offsetMax;
            tempMin.x = 10f;
            tempMax.y = -555f;
            m_rectScroll.offsetMin = tempMin;
            m_rectScroll.offsetMax = tempMax;

            m_rootContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 200);
        }
        else if (m_scrollView.vertical)
        {
            var vLayoutGroup = m_rootContent.gameObject.AddComponent<VerticalLayoutGroup>();
            vLayoutGroup.childControlWidth = false;
            vLayoutGroup.childControlHeight = false;
            vLayoutGroup.spacing = 10f;
            m_layoutGroup = vLayoutGroup;

            m_contentSizeFilter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            m_contentSizeFilter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var tempMin = m_rectScroll.offsetMin;
            var tempMax = m_rectScroll.offsetMax;
            tempMin.x = 1100f;
            tempMax.y = -100f;
            m_rectScroll.offsetMin = tempMin;
            m_rectScroll.offsetMax = tempMax;

            m_rootContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 120);
        }

        m_rootContent.anchoredPosition = Vector2.zero;

        switch (type)
        {
            case Menu.Vertical1:
            case Menu.Horizontal1:

                for (int i = 0; i < 100; i++)
                {
                    var obj = Instantiate(m_prefab, m_rootContent);
                    obj.gameObject.SetActive(true);
                    var text = obj.transform.GetChild(0)?.GetComponent<Text>();
                    if (text != null)
                    {
                        text.text = $"content:{i}";
                    }
                    m_contentObjectList.Add(obj);
                }

                break;
        }
    }
}
