using UnityEngine;

namespace MiaFantasyRun.UI
{
    public abstract class UIView : MonoBehaviour
    {
        [SerializeField] private ScreenId screenId;

        public ScreenId ScreenId => screenId;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
