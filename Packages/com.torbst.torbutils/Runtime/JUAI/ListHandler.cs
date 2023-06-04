using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TorbuTils.JUAI
{
    [RequireComponent(typeof(RectTransform), typeof(VerticalLayoutGroup))]
    public class ListHandler : MonoBehaviour
    {
        private List<LayoutElement> elements = new();
        private VerticalLayoutGroup list;
        private new RectTransform transform;
        private bool queueRefresh;

        private void Awake()
        {
            list = GetComponent<VerticalLayoutGroup>();
            transform = GetComponent<RectTransform>();
        }
        private void Update()
        {
            if (queueRefresh)
            {
                Refresh();
            }
        }
        private void OnTransformChildrenChanged()
        {
            queueRefresh = true;
        }
        public void Refresh()
        {
            queueRefresh = false;
            elements = new();
            foreach (var element in GetComponentsInChildren<LayoutElement>())
            {
                if (element.transform.parent != transform) continue;
                elements.Add(element);
            }

            float y = list.padding.vertical - list.spacing;
            transform.sizeDelta = new(transform.sizeDelta.x, y);
            foreach (var element in elements)
            {
                y += element.preferredHeight + list.spacing;
            }
            transform.sizeDelta = Vector2.up * y;
            
        }
        private void OnGUI()
        {
            Handles.Label(Vector3.zero, $"{elements.Count}, {transform.sizeDelta}");
        }
    }
}
