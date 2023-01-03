
using KeepCoding;
using System;
using System.Collections;
using System.Linq;
using KModkitLib;
using UnityEngine;
using System.Reflection;

public class TrajectoryModule : ModuleScript
{
    [SerializeField]
    internal PaperBehaviour Drawing;
    [SerializeField]
    internal DisplayBehaviour Display;

    internal Puzzle puzzle;
    internal bool isTP = false;

    IEnumerator Start()
    {
        Get<KMSelectable>().OnFocus += HandleFocus;
        Get<KMSelectable>().OnDefocus += HandleDefocus;

        Display.Get<KMSelectable>().OnInteract += () =>
        {
            if (IsSolved) return false;
            if (!Application.isEditor) ButtonEffect(Get<KMSelectable>(), 0.1f, "OpenDisplay");
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
#if DEBUG
            Type t = ReflectionHelper.FindType("TestHarness");
            t.GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(FindObjectOfType(t), new object[] { });
#else
            KTInputManager.Instance.SelectableManager.HandleCancel();
#endif
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
            SetIcons();
        };
        Display.OnSubmit += HandleSubmit;

        yield return null;
        SetIcons();
    }

    private void HandleSubmit()
    {
#if DEBUG
        Type t = ReflectionHelper.FindType("TestHarness");
        t.GetMethod("Cancel", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(FindObjectOfType(t), new object[] { });
#else
        if(!isTP) KTInputManager.Instance.SelectableManager.HandleCancel();
#endif
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