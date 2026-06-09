using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIView> screens = new();

        private readonly Dictionary<ScreenId, UIView> lookup = new();

        private void Awake()
        {
            foreach (var screen in screens)
            {
                lookup[screen.ScreenId] = screen;
                screen.Hide();
            }
        }

        public void Show(ScreenId screenId)
        {
            foreach (var screen in lookup.Values)
            {
                screen.Hide();
            }

            lookup[screenId].Show();
        }

        public void Overlay(ScreenId screenId)
        {
            lookup[screenId].Show();
        }
    }
}
