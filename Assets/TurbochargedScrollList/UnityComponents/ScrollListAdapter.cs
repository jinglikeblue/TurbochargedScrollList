using System;
using UnityEngine;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 列表的unity组件，用来实现列表的更新
    /// </summary>
    [DisallowMultipleComponent]
    public class ScrollListAdapter : MonoBehaviour
    {
        public event Action onUpdate;

        private void Update()
        {
            onUpdate?.Invoke();
        }
    }
}