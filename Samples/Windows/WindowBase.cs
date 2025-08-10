using System;
using UnityEngine;

namespace AiaalTools.Samples.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WindowBase : MonoBehaviour
    {
        public event Action<WindowBase> onWindowClose;
        public event Action<WindowBase> onStartClose;

        public bool IsShowed { get; private set; }

        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
    }
}