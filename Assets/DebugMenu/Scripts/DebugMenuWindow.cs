using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DebugMenu
{
    public partial class DebugMenuWindow : MonoBehaviour
    {
        /// <summary>
        /// デバッグメニュー表示ボタンの表示位置タイプ
        /// </summary>
        public enum OpenWindowButtonPlacementType
        {
            /// <summary>左上</summary>
            LeftTop,
            /// <summary>左中央</summary>
            LeftMiddle,
            /// <summary>左下</summary>
            LeftBottom,
            /// <summary>中央上</summary>
            CenterTop,
            /// <summary>中央下</summary>
            CenterBottom,
            /// <summary>右上</summary>
            RightTop,
            /// <summary>右中央</summary>
            RightMiddle,
            /// <summary>右下</summary>
            RightBottom,
        }
        /// <summary>
        /// キー入力情報タイプ
        /// </summary>
        public enum KeystrokeInfoType
        {
            /// <summary>決定キー</summary>
            Enter,
            /// <summary>キャンセルキー</summary>
            Cancel,
            
            /// <summary>上キー</summary>
            Dir_Up,
            /// <summary>下キー</summary>
            Dir_Down,
            /// <summary>左キー</summary>
            Dir_Left,
            /// <summary>右キー</summary>
            Dir_Right,
        }

        /// <summary>デバッグ画面の実体(シングルトン無効時はnull固定)</summary>
		public static DebugMenuWindow SingletonInstance { get; private set; }
        /// <summary>シングルトンか</summary>
        public static bool IsSingleton { get; set; } = true;
        public static bool IsInstance
        {
            get
            {
                return SingletonInstance != null;
            }
        }

        /// <summary>生成中のページ一覧 key:ページ識別ID value:ページオブジェクトの実体</summary>
        private Dictionary<string, DebugPageBase> m_pushPageDic = new Dictionary<string, DebugPageBase>();
        /// <summary>ページの並び順リスト(ページ識別ID)</summary>
        private List<string> m_openPageOrderList = new List<string>();

        /// <summary>指定無しの場合に表示するデフォルトページのプレハブ</summary>
        [SerializeField] private GameObject m_prefabDefaultPage;
        [SerializeField] private Canvas m_canvas;
        /// <summary>メニュー表示ボタン</summary>
        [SerializeField] private Button m_menuButton;
        [SerializeField] private RectTransform m_menuButtonRect;
        [SerializeField] private Text m_menuButtonText;
        [SerializeField] private RawImage m_menuBG;
        [SerializeField] private RectTransform m_titleTextRoot;
        /// <summary>メニュータイトルテキスト</summary>
        [SerializeField] private Text m_menuTitleText;
        /// <summary>戻るボタン</summary>
        [SerializeField] private Button m_buttonBack;
        /// <summary>閉じるボタン</summary>
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private RectTransform m_menuRoot;
        [SerializeField] private RectTransform m_rectScrollBar;
        [SerializeField] private RectTransform m_rootPage;

        /// <summary>ページオブジェクトのプレハブのリスト</summary>
        private Dictionary<string,GameObject> m_pagePrefabs = new Dictionary<string, GameObject>();
        /// <summary>画面表示中か</summary>
        private bool m_isOpenWindow = false;

        /// <summary>最初のページ</summary>
        public DebugPageBase StartPage
        {
            get
            {
                if (m_openPageOrderList.Count <= 0) return null;
                var pageId = m_openPageOrderList[0];
                if (!m_pushPageDic.TryGetValue(pageId, out var page))
                {
                    return null;
                }

                return page;
            }
        }
        public bool IsOpenWindow => m_isOpenWindow;
        
        public void Awake()
        {
            if (IsSingleton)
            {
                //既にインスタンスが存在する場合は生成せずに破棄する
                if (IsInstance)
                {
                    Destroy(this.gameObject);
                    return;
                }
                //インスタンス保持＆DontDestroy設定
                SingletonInstance = this;
                DontDestroyOnLoad(this);
            }
            //デフォルト表示のページプレハブを登録
            AddPagePrefab(m_prefabDefaultPage);
            //各種ボタン初期化
            m_menuButton.onClick.RemoveAllListeners();
            m_menuButton.onClick.AddListener(()=> 
            {
                if (!m_isOpenWindow)
                {
                    OpenWindow();
                }
                else
                {
                    CloseWindow();
                }
            });
            m_buttonClose.onClick.RemoveAllListeners();
            m_buttonClose.onClick.AddListener(() =>
            {
                OnPressCloseMenuButton();
            });
            m_buttonBack.onClick.RemoveAllListeners();
            m_buttonBack.onClick.AddListener(() =>
            {
                OnPressBackMenuButton();
            });
            //
            m_titleTextRoot.gameObject.SetActive(false);
            m_menuRoot.gameObject.SetActive(false);
            //
            SetMenuButtonPlacement(OpenWindowButtonPlacementType.LeftTop);
        }

        /// <summary>
        /// ページプレハブ登録
        /// </summary>
        /// <param name="prefab"></param>
        public void AddPagePrefab(GameObject prefab)
        {
            if (m_pagePrefabs.ContainsKey(prefab.name))
            {
                Debug.Log($"{prefab.name}は登録済み");
                return;
            }
            m_pagePrefabs.Add(prefab.name, prefab);
        }

        /// <summary>
        /// 初回表示ページの登録
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="prefabName"></param>
        /// <param name="isOverwrite"></param>
        /// <returns></returns>
        public TPage Initialize<TPage>(string prefabName,bool isOverwrite = false) where TPage : DebugPageBase
        {
            //既に最初のページを生成済みの場合は処理しない
            if(!isOverwrite && StartPage != null)
            {
                return null;
            }
            //作成済みのページリストを一旦初期化
            ResetPages();
            //ページ生成
            var addPage = PushPage<TPage>(prefabName);
            return addPage;
        }
        public TPage Initialize<TPage>() where TPage : DebugPageBase
        {
            return Initialize<TPage>(m_prefabDefaultPage.name);
        }

        /// <summary>
        /// 生成済みページリストの初期化
        /// </summary>
        /// <param name="keepFirstPage"></param>
        private void ResetPages(bool keepFirstPage = false)
        {
            for(int i = m_openPageOrderList.Count - 1; i > 0; i--)
            {
                //一番初めのページは破棄しない
                if(i == 0 && keepFirstPage)
                {
                    continue;
                }

                var pageId = m_openPageOrderList[i];
                if (m_pushPageDic.TryGetValue(pageId, out var page))
                {
                    if (page.gameObject != null)
                    {
                        Destroy(page.gameObject);
                    }
                    m_pushPageDic.Remove(pageId);
                }
                m_openPageOrderList.RemoveAt(i);
            }
        }

        /// <summary>
        /// 開閉ボタン表示の更新
        /// </summary>
        private void UpdateMenuButton()
        {
            string btnText = m_isOpenWindow ? "<" : ">";
            m_menuButtonText.text = btnText;
        }
        /// <summary>
        /// 閉じるボタン押下時の処理
        /// </summary>
        private void OnPressCloseMenuButton()
        {
            ResetPages(true);

            m_titleTextRoot.gameObject.SetActive(false);
            m_menuButton.gameObject.SetActive(true);
            m_menuRoot.gameObject.SetActive(false);
            m_isOpenWindow = false;
            UpdateMenuButton();
        }
        /// <summary>
        /// 戻るボタン押下時の処理
        /// </summary>
        private void OnPressBackMenuButton()
        {
            PrevPage();
        }
        /// <summary>
        /// ページタイトル設定
        /// </summary>
        /// <param name="title"></param>
        public void SetPageTitle(string title)
        {
            m_menuTitleText.text = title;
        }

        /// <summary>
        /// メニュー表示ボタンの表示位置設定
        /// </summary>
        /// <param name="type"></param>
        public void SetMenuButtonPlacement(OpenWindowButtonPlacementType type, float offsetX = 0f, float offsetY = 0f)
        {
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.zero;
            Vector2 pivot = Vector2.zero;

            switch (type)
            {
                case OpenWindowButtonPlacementType.LeftTop:
                    anchorMin = new Vector2(0, 1);
                    anchorMax = new Vector2(0, 1);
                    pivot = new Vector2(0, 1);
                    break;
                case OpenWindowButtonPlacementType.LeftMiddle:
                    anchorMin = new Vector2(0, 0.5f);
                    anchorMax = new Vector2(0, 0.5f);
                    pivot = new Vector2(0, 0.5f);
                    break;
                case OpenWindowButtonPlacementType.LeftBottom:
                    anchorMin = new Vector2(0, 0);
                    anchorMax = new Vector2(0, 0);
                    pivot = new Vector2(0, 0);
                    break;
                case OpenWindowButtonPlacementType.CenterTop:
                    anchorMin = new Vector2(0.5f, 1);
                    anchorMax = new Vector2(0.5f, 1);
                    pivot = new Vector2(0.5f, 1);
                    break;
                case OpenWindowButtonPlacementType.CenterBottom:
                    anchorMin = new Vector2(0.5f, 0);
                    anchorMax = new Vector2(0.5f, 0);
                    pivot = new Vector2(0.5f, 0);
                    break;
                case OpenWindowButtonPlacementType.RightTop:
                    anchorMin = new Vector2(1, 1);
                    anchorMax = new Vector2(1, 1);
                    pivot = new Vector2(1, 1);
                    break;
                case OpenWindowButtonPlacementType.RightMiddle:
                    anchorMin = new Vector2(1, 0.5f);
                    anchorMax = new Vector2(1, 0.5f);
                    pivot = new Vector2(1, 0.5f);
                    break;
                case OpenWindowButtonPlacementType.RightBottom:
                    anchorMin = new Vector2(1, 0);
                    anchorMax = new Vector2(1, 0);
                    pivot = new Vector2(1, 0);
                    break;
            }

            m_menuButtonRect.anchoredPosition = new Vector2(offsetX, offsetY);

            m_menuButtonRect.anchorMin = anchorMin;
            m_menuButtonRect.anchorMax = anchorMax;
            m_menuButtonRect.pivot = pivot;
        }

        /// <summary>
        /// ページ追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PushPage<T>() where T : DebugPageBase
        {
            return PushPage<T>(m_prefabDefaultPage.name);
        }
        public T PushPage<T>(string prefabName) where T : DebugPageBase
        {
            if (!m_pagePrefabs.ContainsKey(prefabName))
            {
                return null;
            }

            var obj = Instantiate(m_pagePrefabs[prefabName], m_rootPage);
            //ページコンポーネントチェック
            var comp = obj.GetComponent<DebugPageBase>();
            T target = null;
            if (comp != null)
            {
                //既に指定のコンポーネントがついている場合はそのまま返す
                if (comp is T compT)
                {
                    target = compT;
                }
                else
                {
                    //別のページコンポーネントがついている場合は、生成失敗と判定してオブジェクト破棄する
                    Destroy(obj);
                    return null;
                }
            }
            else
            {
                //ページコンポーネントがついてない場合は追加
                target = obj.AddComponent<T>();
            }

            OnPushedPage(target);

            return target;
        }
        /// <summary>
        /// ページ追加
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="openPage"></param>
        public void PushPage(Type pageType,bool openPage = false)
        {
            PushPage(pageType, m_prefabDefaultPage.name, openPage);
        }
        public void PushPage(Type pageType, string prefabName,bool openPage = false)
        {
            if (!m_pagePrefabs.ContainsKey(prefabName))
            {
                return;
            }

            var obj = Instantiate(m_pagePrefabs[prefabName], m_rootPage);
            //ページコンポーネントチェック

            if(!obj.TryGetComponent(pageType,out var page))
            {
                page = obj.AddComponent(pageType);
            }
            var pushPage = (DebugPageBase)page;

            OnPushedPage(pushPage,openPage);
        }
        /// <summary>
        /// ページ追加後の処理
        /// </summary>
        /// <param name="page"></param>
        /// <param name="openPage"></param>
        private void OnPushedPage(DebugPageBase page, bool openPage = false)
        {
            page.Initialize(this);

            //前のページ非表示
            if (openPage)
            {
                if (m_openPageOrderList.Count > 0)
                {
                    string prevPageId = m_openPageOrderList[m_openPageOrderList.Count - 1];
                    if (m_pushPageDic.TryGetValue(prevPageId, out var prevPage))
                    {
                        prevPage.gameObject.SetActive(false);
                    }
                }
            }
            
            //作ったページ一旦非表示
            page.gameObject.SetActive(false);

            m_pushPageDic.Add(page.PageID, page);
            m_openPageOrderList.Add(page.PageID);

            if (openPage)
            {
                OpenPage(page.PageID);
            }
        }

        /// <summary>
        /// ページ表示
        /// </summary>
        /// <param name="index"></param>
        private void OpenPage(int index)
        {
            if (m_openPageOrderList.Count <= index) return;
            var pageID = m_openPageOrderList[index];            
            OpenPage(pageID);
        }
        private void OpenPage(string id)
        {
            if(!m_pushPageDic.TryGetValue(id,out var page))
            {
                return;
            }

            page.gameObject.SetActive(true);
            page.OpenMenu();
            SetPageTitle(page.PageTitle);
        }

        /// <summary>
        /// 前のページを表示
        /// </summary>
        private void PrevPage()
        {
            if (m_openPageOrderList.Count <= 0) return;
            
            if(m_openPageOrderList.Count > 1)
            {
                /*戻すページがある場合の処理*/

                //現在表示中のページを破棄
                var currentPageID = m_openPageOrderList[m_openPageOrderList.Count - 1];
                if (m_pushPageDic.TryGetValue(currentPageID, out var page))
                {
                    page.CloseMenu();
                    m_openPageOrderList.RemoveAt(m_openPageOrderList.Count - 1);
                    m_pushPageDic.Remove(page.PageID);
                }

                //前のページを再表示
                var prevPageID = m_openPageOrderList[m_openPageOrderList.Count - 1];
                OpenPage(prevPageID);
            }
            else
            {
                //戻すページが無い場合はデバッグ画面を閉じる
                CloseWindow();
            }
        }
        /// <summary>
        /// デバッグ画面表示のアクティブ設定
        /// </summary>
        /// <param name="isActive"></param>
        public static void SetActiveCanvas(bool isActive)
        {
            if (!IsInstance || SingletonInstance.m_canvas == null) return;
            SingletonInstance.m_canvas.enabled = isActive;
        }

        /// <summary>
        /// デバッグウィンドウ展開
        /// </summary>
        public void OpenWindow()
        {
            if (!m_isOpenWindow)
            {
                m_isOpenWindow = true;
                m_titleTextRoot.gameObject.SetActive(true);
                m_menuButton.gameObject.SetActive(false);
                m_menuRoot.gameObject.SetActive(true);
                
                if (m_pushPageDic.Count > 0)
                {
                    OpenPage(0);
                }
            }
        }
        /// <summary>
        /// デバッグウィンドウ閉じる
        /// </summary>
        public void CloseWindow()
        {
            //m_menuBG.gameObject.SetActive(false);
            m_titleTextRoot.gameObject.SetActive(false);
            m_menuButton.gameObject.SetActive(true);
            m_menuRoot.gameObject.SetActive(false);
            m_isOpenWindow = false;
            UpdateMenuButton();
        }

        /// <summary>
        /// デバッグ画面側にキー入力の情報を送信
        /// </summary>
        /// <param name="info"></param>
        public void SendKeystrokeInfo(KeystrokeInfoType info)
        {
            //
            bool isInput = false;
            var pageId = m_openPageOrderList[m_openPageOrderList.Count - 1];
            if (m_pushPageDic.TryGetValue(pageId, out var page))
            {
                isInput = page.OnInputKeyStroke(info);
            }
            //
            if (!isInput)
            {
                if (info == KeystrokeInfoType.Cancel)
                {
                    //ページバック
                    OnPressBackMenuButton();
                }
            }
        }

    }
}
