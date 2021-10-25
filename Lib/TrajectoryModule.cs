
using KeepCoding;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace KModkitLib
{

    public class TrajectoryModule : ModuleScript
    {
        [SerializeField]
        PaperBehaviour Drawing;
        [SerializeField]
        DisplayBehaviour Display;

        Puzzle puzzle;

        IEnumerator Start()
        {
            Display.Deactivate();
            Get<KMSelectable>().OnFocus += HandleFocus;
            Get<KMSelectable>().OnDefocus += HandleDefocus;

            Display.Get<KMSelectable>().OnInteract += () =>
            {
                if (!IsEditor) ButtonEffect(Get<KMSelectable>(), 0.1f, "OpenDisplay");
                Display.Activate();
                return true;
            };

            Display.Get<KMSelectable>().OnDefocus += () =>
            {
                PlaySound("CloseDisplay");
                Display.Deactivate();
                HandleFocus();
            };

            Display.OnClose += () =>
            {
                KTInputManager.Instance.SelectableManager.HandleCancel();
            };

            puzzle = new Puzzle(this);
            SetIcons();

            Display.OnConfigPress += (int i) =>
            {
                PlaySound("DisplayButton");
                puzzle.ApplyConfiguration(i);
                SetIcons();
            };

            Display.OnReset += () =>
            {
                PlaySound("DisplayButton");
                puzzle.Reset();
            };
            Display.OnSubmit += HandleSubmit;

            yield return null;
            SetIcons();
        }

        private void HandleSubmit()
        {
            KTInputManager.Instance.SelectableManager.HandleCancel();
            if (puzzle.Check())
            {
                Drawing.HandleSolve(() => { Solve(); });
            } else
            {
                Drawing.HandleStrike(() => { Strike(); });
            }
        }

        void HandleFocus()
        {
            Drawing.isInteracted = true;
        }

        void HandleDefocus()
        {
            Drawing.isInteracted = false;
        }

        void SetIcons()
        {
            var index = puzzle.currentStates.Select(state => state.place.ToIconIndex()).ToArray(); 
            var isClock = puzzle.currentStates.Select(state => state.isClock).ToArray();

            Drawing.SetIcons(index, isClock);
        }
	}
}