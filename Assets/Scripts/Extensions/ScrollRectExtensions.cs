using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExtensions
{
    /// <summary>
    /// 垂直方向スクロール位置の更新
    /// </summary>
    /// <param name="scrollRect"></param>
    /// <param name="layout"></param>
    /// <param name="contentSizeRect">スクロール画面内に生成する要素のRect情報</param>
    /// <param name="contentCount">スクロール画面内に生成されている要素の総数</param>
    /// <param name="selectContentIndex">選択されたスクロール画面内の要素の番号</param>
    /// <param name="beforeContentIndex">前回選択したスクロール画面内の要素の番号</param>
    public static void UpdateScrollVertical(this ScrollRect scrollRect, LayoutGroup layout, RectTransform contentSizeRect, int contentCount, int selectContentIndex, int beforeContentIndex = 0)
    {
        float setNormalizedPos = scrollRect.verticalNormalizedPosition;
        if (setNormalizedPos < 0)
        {
            setNormalizedPos = 0f;
        }

        if (selectContentIndex <= 0)
        {
            //一番上まで上げる
            setNormalizedPos = 1f;
        }
        else if (contentCount - 1 <= selectContentIndex)
        {
            //一番下まで下げる
            setNormalizedPos = 0f;
        }
        else
        {
            var scrollRatio = 1f - setNormalizedPos;
            var viewRect = scrollRect.viewport.rect;
            var heightMax = scrollRect.content.rect.height - viewRect.height;
            var padding = layout.padding;

            double beforeHeight = heightMax * scrollRatio;
            var height = CalculateScrollPositionHeight(layout, contentSizeRect, selectContentIndex, selectContentIndex > beforeContentIndex);

            //現在のスクロール位置から変動させるかチェック
            if (height - beforeHeight > viewRect.height)
            {
                if(height > beforeHeight)
                {
                    //選択した要素がスクロール画面の一番下に納まるようスクロール位置を移動
                    height = CalculateScrollPositionHeight(layout, contentSizeRect, selectContentIndex, true);
                    height = Mathf.Max(0, (float)(height - viewRect.height + padding.bottom));
                    setNormalizedPos = height > 0f ? Mathf.Clamp01(1 - (float)(height / heightMax)) : 0f;
                }
                else
                {
                    //選択した要素がスクロール画面の一番上に納まるようスクロール位置を移動
                    height = CalculateScrollPositionHeight(layout, contentSizeRect, selectContentIndex) - padding.top;
                    setNormalizedPos = 1f - Mathf.Clamp01((float)(height / heightMax));
                }
            }
            else if (height < beforeHeight)
            {
                //選択した要素がスクロール画面の一番上に納まるようスクロール位置を移動
                height = CalculateScrollPositionHeight(layout, contentSizeRect, selectContentIndex) - padding.top;
                setNormalizedPos = 1f - Mathf.Clamp01((float)(height / heightMax));
            }
        }

        //スクロール位置更新
        scrollRect.verticalNormalizedPosition = setNormalizedPos;
    }

    /// <summary>
    /// 垂直方向スクロール位置の更新
    /// サイズが異なる子要素を並べている場合の更新処理はこちら
    /// </summary>
    /// <param name="scrollRect"></param>
    /// <param name="layout"></param>
    /// <param name="contentRectList"></param>
    /// <param name="selectContentIndex"></param>
    /// <param name="beforeContentIndex"></param>
    public static void UpdateScrollVertical(this ScrollRect scrollRect, LayoutGroup layout, List<RectTransform> contentRectList, int selectContentIndex, int beforeContentIndex = 0)
    {
        float setNormalizedPos = scrollRect.verticalNormalizedPosition;
        if (setNormalizedPos < 0)
        {
            setNormalizedPos = 0f;
        }

        if (selectContentIndex <= 0)
        {
            //一番上まで上げる
            setNormalizedPos = 1f;
        }
        else if (contentRectList.Count - 1 <= selectContentIndex)
        {
            //一番下まで下げる
            setNormalizedPos = 0f;
        }
        else
        {
            var scrollRatio = 1f - setNormalizedPos;
            var viewRect = scrollRect.viewport.rect;
            var heightMax = scrollRect.content.rect.height - viewRect.height;
            var padding = layout.padding;

            double beforeHeight = heightMax * scrollRatio;
            var height = CalculateScrollPositionHeight(layout,contentRectList,selectContentIndex, selectContentIndex > beforeContentIndex);

            //現在のスクロール位置から変動させるかチェック
            if (height - beforeHeight > viewRect.height)
            {
                if (height > beforeHeight)
                {
                    //選択した要素がスクロール画面の一番下に納まるようスクロール位置を移動
                    height = CalculateScrollPositionHeight(layout, contentRectList, selectContentIndex, true);
                    height = Mathf.Max(0, (float)(height - viewRect.height + padding.bottom));
                    setNormalizedPos = height > 0f ? Mathf.Clamp01(1 - (float)(height / heightMax)) : 0f;
                }
                else
                {
                    //選択した要素がスクロール画面の一番上に納まるようスクロール位置を移動
                    height = CalculateScrollPositionHeight(layout, contentRectList, selectContentIndex) - padding.top;
                    setNormalizedPos = 1f - Mathf.Clamp01((float)(height / heightMax));
                }
            }
            else if (height < beforeHeight)
            {
                //選択した要素がスクロール画面の一番上に納まるようスクロール位置を移動
                height = CalculateScrollPositionHeight(layout, contentRectList, selectContentIndex) - padding.top;
                setNormalizedPos = 1f - Mathf.Clamp01((float)(height / heightMax));
            }
        }

        //スクロール位置更新
        scrollRect.verticalNormalizedPosition = setNormalizedPos;
    }
    public static void UpdateScrollVertical<T>(this ScrollRect scrollRect, LayoutGroup layout, List<T> contentList, int selectContentIndex, int beforeContentIndex = 0) where T : UnityEngine.Object
    {
        if (contentList.Count <= 0) return;

        //子要素からRectTransformを取得
        List<RectTransform> contentRectList = new List<RectTransform>();
        foreach(var content in contentList)
        {
            var rect = content.GetComponent<RectTransform>();
            if(rect == null)
            {
                break;
            }
            contentRectList.Add(rect);
        }
        //RectTransformが取得できなかった場合は処理しない
        if (contentRectList.Count != contentList.Count)
        {
            return;
        }

        UpdateScrollVertical(scrollRect, layout, contentRectList, selectContentIndex, beforeContentIndex);
    }

    /// <summary>
    /// 水平方向スクロール位置の更新
    /// </summary>
    /// <param name="scrollRect"></param>
    /// <param name="layout"></param>
    /// <param name="contentSizeRect"></param>
    /// <param name="contentCount"></param>
    /// <param name="selectContentIndex"></param>
    /// <param name="beforeContentIndex"></param>
    public static void UpdateScrollHorizontal(this ScrollRect scrollRect, LayoutGroup layout, RectTransform contentSizeRect, int contentCount, int selectContentIndex, int beforeContentIndex = 0)
    {
        float setNormalizedPos = scrollRect.horizontalNormalizedPosition;
        if (setNormalizedPos < 0)
        {
            setNormalizedPos = 0f;
        }

        if (selectContentIndex <= 0)
        {
            //左端へ移動
            setNormalizedPos = 0f;
        }
        else if (contentCount - 1 <= selectContentIndex)
        {
            //右端まで移動
            setNormalizedPos = 1f;
        }
        else
        {
            var scrollRatio = setNormalizedPos;
            var viewRect = scrollRect.viewport.rect;
            var widthMax = scrollRect.content.rect.width - viewRect.width;
            var padding = layout.padding;

            double beforeWidth = widthMax * scrollRatio;
            var width = CalculateScrollPositionWidth(layout, contentSizeRect, selectContentIndex, selectContentIndex > beforeContentIndex);

            //現在のスクロール位置から変動させるかチェック
            if (width - beforeWidth > viewRect.width)
            {
                if (width > beforeWidth)
                {
                    //選択した要素がスクロール画面の右端に納まるようスクロール位置を移動
                    width = CalculateScrollPositionWidth(layout, contentSizeRect, selectContentIndex, true);
                    width = Mathf.Max(0, (float)(width - viewRect.width + padding.left));
                    setNormalizedPos = width > 0f ? Mathf.Clamp01((float)(width / widthMax)) : 0f;
                }
                else
                {
                    //選択した要素がスクロール画面の左端に納まるようスクロール位置を移動
                    width = CalculateScrollPositionWidth(layout, contentSizeRect, selectContentIndex) - padding.right;
                    setNormalizedPos = Mathf.Clamp01((float)(width / widthMax));
                }
            }
            else if (width < beforeWidth)
            {
                //選択した要素がスクロール画面の左端に納まるようスクロール位置を移動
                width = CalculateScrollPositionWidth(layout, contentSizeRect, selectContentIndex) - padding.right;
                setNormalizedPos = Mathf.Clamp01((float)(width / widthMax));
            }
        }

        //スクロール位置更新
        scrollRect.horizontalNormalizedPosition = setNormalizedPos;
    }

    /// <summary>
    /// 水平方向スクロール位置の更新
    /// サイズが異なる子要素を並べている場合の更新処理はこちら
    /// </summary>
    /// <param name="scrollRect"></param>
    /// <param name="layout"></param>
    /// <param name="contentRectList"></param>
    /// <param name="selectContentIndex"></param>
    /// <param name="beforeContentIndex"></param>
    public static void UpdateScrollHorizontal(this ScrollRect scrollRect, LayoutGroup layout, List<RectTransform> contentRectList, int selectContentIndex, int beforeContentIndex = 0)
    {
        float setNormalizedPos = scrollRect.horizontalNormalizedPosition;
        if (setNormalizedPos < 0)
        {
            setNormalizedPos = 0f;
        }

        if (selectContentIndex <= 0)
        {
            //左端へ移動
            setNormalizedPos = 0f;
        }
        else if (contentRectList.Count - 1 <= selectContentIndex)
        {
            //右端まで移動
            setNormalizedPos = 1f;
        }
        else
        {
            var scrollRatio = setNormalizedPos;
            var viewRect = scrollRect.viewport.rect;
            var widthMax = scrollRect.content.rect.width - viewRect.width;
            var padding = layout.padding;

            double beforeWidth = widthMax * scrollRatio;
            var width = CalculateScrollPositionWidth(layout, contentRectList, selectContentIndex, selectContentIndex > beforeContentIndex);

            //現在のスクロール位置から変動させるかチェック
            if (width - beforeWidth > viewRect.width)
            {
                if (width > beforeWidth)
                {
                    //選択した要素がスクロール画面の右端に納まるようスクロール位置を移動
                    width = CalculateScrollPositionWidth(layout, contentRectList, selectContentIndex, true);
                    width = Mathf.Max(0, (float)(width - viewRect.width + padding.left));
                    setNormalizedPos = width > 0f ? Mathf.Clamp01((float)(width / widthMax)) : 0f;
                }
                else
                {
                    //選択した要素がスクロール画面の左端に納まるようスクロール位置を移動
                    width = CalculateScrollPositionWidth(layout, contentRectList, selectContentIndex) - padding.right;
                    setNormalizedPos = Mathf.Clamp01((float)(width / widthMax));
                }
            }
            else if (width < beforeWidth)
            {
                //選択した要素がスクロール画面の左端に納まるようスクロール位置を移動
                width = CalculateScrollPositionWidth(layout, contentRectList, selectContentIndex) - padding.right;
                setNormalizedPos = Mathf.Clamp01((float)(width / widthMax));
            }
        }

        //スクロール位置更新
        scrollRect.horizontalNormalizedPosition = setNormalizedPos;
    }
    public static void UpdateScrollHorizontal<T>(this ScrollRect scrollRect, LayoutGroup layout, List<T> contentList, int selectContentIndex, int beforeContentIndex = 0) where T : UnityEngine.Object
    {
        //子要素からRectTransformを取得
        bool failedFindTransforms = false;
        List<RectTransform> contentRectList = new List<RectTransform>();
        foreach (var content in contentList)
        {
            var rect = content.GetComponent<RectTransform>();
            if (rect == null)
            {
                failedFindTransforms = true;
                break;
            }
            contentRectList.Add(rect);
        }
        //RectTransformが取得できなかった場合は処理しない
        if (failedFindTransforms)
        {
            return;
        }

        UpdateScrollHorizontal(scrollRect, layout, contentRectList, selectContentIndex, beforeContentIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="contentSize"></param>
    /// <param name="selectIdx"></param>
    /// <param name="isBottom"></param>
    /// <returns></returns>
    private static double CalculateScrollPositionHeight(LayoutGroup layout, RectTransform contentSizeRect, int selectIdx, bool isBottom = false)
    {
        Vector2 contentSize = Vector2.zero;
        float spacingHeight = 0f;
        RectOffset padding = null;
        if(layout != null)
        {
            //空白サイズ取得
            if (layout is GridLayoutGroup gridLayoutGroup)
            {
                contentSize = gridLayoutGroup.cellSize;
                spacingHeight = gridLayoutGroup.spacing.y;
            }
            else if (contentSizeRect != null)
            {
                contentSize = contentSizeRect.sizeDelta;
            }
            else
            {
                contentSize = Vector2.zero;
            }
            padding = layout.padding;

            if (layout is VerticalLayoutGroup verticalLayoutGroup)
            {
                spacingHeight = verticalLayoutGroup.spacing;
            }
        }
        var contentSizeHeight = contentSize.y + spacingHeight;

        int constraintCount = 0;//横方向に並ぶ要素の数
        if (layout != null)
        {
            if (layout is GridLayoutGroup gridLayoutGroup)
            {
                constraintCount = gridLayoutGroup.constraintCount;
            }
            else
            {
                constraintCount = 1;
            }
        }
        else
        {
            constraintCount = 1;
        }

        double ret = Mathf.Max(0, contentSizeHeight * (selectIdx / constraintCount));
        if (isBottom)
        {
            ret += contentSizeHeight;
        }
        else
        {
            ret += padding?.top ?? 0;
        }
        return ret;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="contentRectList"></param>
    /// <param name="selectIdx"></param>
    /// <param name="isBottom"></param>
    /// <returns></returns>
    private static double CalculateScrollPositionHeight(LayoutGroup layout, List<RectTransform> contentRectList, int selectIdx, bool isBottom = false)
    {
        //空白サイズ取得
        float spacingHeight = 0f;
        RectOffset padding = null;
        if (layout != null)
        {
            if (layout is GridLayoutGroup gridLayoutGroup)
            {
                spacingHeight = gridLayoutGroup.spacing.y;
            }
            else if (layout is VerticalLayoutGroup verticalLayoutGroup)
            {
                spacingHeight = verticalLayoutGroup.spacing;
            }
            padding = layout.padding;
        }
        var contentSizeHeight = contentRectList[selectIdx].sizeDelta.y;

        double ret = 0;
        for (int i = 0; i < selectIdx; i++)
        {
            var contentRect = contentRectList[i];
            ret += contentRect.sizeDelta.y + spacingHeight;
        }

        if (isBottom)
        {
            ret += contentSizeHeight;
        }
        else
        {
            ret += padding?.top ?? 0;
        }
        return ret;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="contentSizeRect"></param>
    /// <param name="selectIdx"></param>
    /// <param name="isBottom"></param>
    /// <returns></returns>
    private static double CalculateScrollPositionWidth(LayoutGroup layout, RectTransform contentSizeRect, int selectIdx, bool isBottom = false)
    {
        //1要素の表示サイズ算出
        Vector2 contentSize;
        float spacingWidth = 0f;
        RectOffset padding = null;
        if (layout != null)
        {
            //要素自体の表示サイズ取得
            if (layout is GridLayoutGroup gridLayoutGroup)
            {
                contentSize = gridLayoutGroup.cellSize;
                spacingWidth = gridLayoutGroup.spacing.x;//空白サイズも取得しておく
            }
            else if (contentSizeRect != null)
            {
                contentSize = contentSizeRect.sizeDelta;
            }
            else
            {
                contentSize = Vector2.zero;
            }
            padding = layout.padding;

            //空白サイズ取得
            if (layout is HorizontalLayoutGroup horizontalLayoutGroup)
            {
                spacingWidth = horizontalLayoutGroup.spacing;
            }
        }
        else
        {
            contentSize = Vector2.zero;
        }
        float contentSizeWidth = contentSize.x + spacingWidth;

        int constraintCount = 0;//縦方向に並ぶ要素の数
        if (layout != null)
        {
            if (layout is GridLayoutGroup gridLayoutGroup)
            {
                constraintCount = gridLayoutGroup.constraintCount;
            }
            else
            {
                constraintCount = 1;
            }
        }
        else
        {
            constraintCount = 1;
        }

        double ret = Mathf.Max(0, contentSizeWidth * (selectIdx / constraintCount));
        if (isBottom)
        {
            ret += contentSizeWidth;
        }
        else
        {
            ret += padding?.left ?? 0;
        }
        return ret;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="contentRectList"></param>
    /// <param name="selectIdx"></param>
    /// <param name="isBottom"></param>
    /// <returns></returns>
    private static double CalculateScrollPositionWidth(LayoutGroup layout, List<RectTransform> contentRectList, int selectIdx, bool isBottom = false)
    {
        //空白サイズ取得
        float spacingWidth = 0f;
        RectOffset padding = null;
        if (layout != null)
        {
            if (layout is GridLayoutGroup groupLayout)
            {
                spacingWidth = groupLayout.spacing.x;
            }
            else if (layout is HorizontalLayoutGroup horizontalLayout)
            {
                spacingWidth = horizontalLayout.spacing;
            }
            padding = layout.padding;
        }
        var contentSizeWidth = contentRectList[selectIdx].sizeDelta.x;

        double ret = 0;
        for (int i = 0; i < selectIdx; i++)
        {
            var contentRect = contentRectList[i];
            ret += contentRect.sizeDelta.x + spacingWidth;
        }

        if (isBottom)
        {
            ret += contentSizeWidth;
        }
        else
        {
            ret += padding?.left ?? 0;
        }
        return ret;
    }
}
