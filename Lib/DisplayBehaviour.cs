using KeepCoding;
using System;
using UnityEngine;


class DisplayBehaviour: CacheableBehaviour
{
    [SerializeField]
    KMSelectable SubmitButton;
    [SerializeField]
    KMSelectable ResetButton;
    [SerializeField]
    KMSelectable[] ConfigButtons;
    [SerializeField]
    KMSelectable CloseButton;


    internal Action OnSubmit;
    internal Action OnReset;
    internal Action<int> OnConfigPress;
    internal Action OnClose;

    private bool isActive;

    void Start()
    {
        SubmitButton.OnInteract += () =>
        {
            if (!isActive) return false;
            OnSubmit();
            return false;
        };

        ResetButton.OnInteract += () =>
        {
            if (!isActive) return false;
            OnReset();
            return false;
        };

        for(int i = 0; i < 3; i++)
        {
            var j = i;
            ConfigButtons[i].OnInteract += () =>
            {
                if (!isActive) return false;
                OnConfigPress(j);
                return false;
            };
        }

        CloseButton.OnInteract += () =>
        {
            if (!isActive) return false;
            OnClose();
            return false;
        };
    }

    internal void Activate()
    {
        transform.localPosition = new Vector3(0, 0.0632f, 0);
        transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        isActive = true;
    }

    internal void Deactivate()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(0, 0, 0);

        isActive = false;
    }

}
