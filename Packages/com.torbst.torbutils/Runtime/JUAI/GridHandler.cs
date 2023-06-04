using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TorbuTils.JUAI
{
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
    public class GridHandler : MonoBehaviour
    {
        private List<RectTransform> elements = new();
        private GridLayoutGroup grid;
        private new RectTransform transform;
        private bool queueRefresh;
        private int rows;
        private void Awake()
        {
            grid = GetComponent<GridLayoutGroup>();
            transform = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (queueRefresh)
            {
                Refresh();
            }
        }
        private void OnRectTransformDimensionsChange()
        {
            queueRefresh = true;
        }
        private void OnTransformChildrenChanged()
        {
            elements = new();
            foreach (var rectT in GetComponentsInChildren<RectTransform>())
            {
                if (rectT == transform) continue;
                if (rectT.parent != transform) continue;
                elements.Add(rectT);
            }
            queueRefresh = true;
        }
        private void Refresh()
        {
            queueRefresh = false;
            // Get row count
            var prevY = float.MinValue;
            rows = 0;
            foreach (var rectT in elements)
            {
                var y = rectT.anchoredPosition.y;
                if (Mathf.Abs(y - prevY) > 0f)
                    rows++;
                prevY = y;
            }

            float cellHeight = grid.cellSize.y;
            float cellSpacing = grid.spacing.y;
            float gridPadding = grid.padding.top+grid.padding.bottom;
            float finalHeight = cellHeight*rows + cellSpacing*(rows-1) + gridPadding;
            transform.sizeDelta = new(transform.sizeDelta.x, finalHeight);
        }
    }
}
