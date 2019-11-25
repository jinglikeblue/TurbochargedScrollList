using System;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{    
    public class HorizontalScrollListComponent : BaseScrollListComponent
    {
        [Header("item gap")]
        public float gap;                

        private void Awake()
        {
            list = new HorizontalScrollList<object>(gameObject, itemPrefab, ItemRender, gap);
        }
    }
}