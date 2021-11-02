using KeepCoding;
using Kernels;
using System;
using System.Collections;
using UnityEngine;


class DisplayBehaviour: CacheableBehaviour
{
    [SerializeField]
    internal KMSelectable SubmitButton;
    [SerializeField]
    internal KMSelectable ResetButton;
    [SerializeField]
    internal KMSelectable[] ConfigButtons;
    [SerializeField]
    internal KMSelectable CloseButton;
    [SerializeField]
    ComputeShader hologramShader;


    internal Action OnSubmit;
    internal Action OnReset;
    internal Action<int> OnConfigPress;
    internal Action OnClose;

    internal bool isActive;
    private float str = 60;
    private Coroutine animationCoroutine;
    private Texture baseTexture;
    private HologramKernel hologramKernel;


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
        baseTexture = Get<Renderer>().material.mainTexture;
        hologramKernel = new HologramKernel(hologramShader);
        Get<Renderer>().enabled = false;
    }

    void Update()
    {
        if (!isActive || baseTexture == null) return;
        var tmp = Get<Renderer>().material.mainTexture;
        Get<Renderer>().material.mainTexture = hologramKernel.Compute(baseTexture, 0);
        if(tmp != baseTexture) DestroyImmediate(tmp, true);

    }

    internal void Activate()
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(ActivateCoroutine());
    }

    internal IEnumerator ActivateCoroutine()
    {
        Get<Renderer>().enabled = true;
        for (; str > 0; str -= Time.deltaTime * 100)
        {
            if (baseTexture == null)
            {
                yield return null;
                continue;
            }
            var tmp = Get<Renderer>().material.mainTexture;
            Get<Renderer>().material.mainTexture = hologramKernel.Compute(baseTexture, (int)str);
            if (tmp != baseTexture) DestroyImmediate(tmp, true);
            yield return null;
        }
        isActive = true;
    }

    internal void Deactivate()
    {
        isActive = false;
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(DeactivateCoroutine());
    }

    internal IEnumerator DeactivateCoroutine()
    {
        for (; str < 60; str += Time.deltaTime * 300f)
        {
            if (baseTexture == null)
            {
                yield return null;
                continue;
            }
            var tmp = Get<Renderer>().material.mainTexture;
            Get<Renderer>().material.mainTexture = hologramKernel.Compute(baseTexture, (int)str);
            if (tmp != baseTexture) DestroyImmediate(tmp, true);
            yield return null;
        }
        Get<Renderer>().enabled = false;
    }
}
