using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyCraft
{
    public class InventoryTitle : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private Vector2 offset;

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.parent.position = eventData.position - offset;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            offset = eventData.position - new Vector2(this.transform.parent.position.x, this.transform.parent.position.y);
        }

    }
}