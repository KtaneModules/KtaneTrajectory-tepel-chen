using KeepCoding;
using Kernels;
using System;
using System.Collections;
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
    [SerializeField]
    ComputeShader hologramShader;


    internal Action OnSubmit;
    internal Action OnReset;
    internal Action<int> OnConfigPress;
    internal Action OnClose;

    private bool isActive;
    private Texture baseTexture;
    private HologramKernel hologramKernel;


    void Start()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(0, 0, 0);
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
        transform.localPosition = new Vector3(0, 0.0632f, 0);
        transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        StartCoroutine(ActivateCoroutine());
    }

    internal IEnumerator ActivateCoroutine()
    {
        for (var str = 60.0f; str > 0; str -= Time.deltaTime * 100)
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
        StartCoroutine(DeactivateCoroutine());
    }

    internal IEnumerator DeactivateCoroutine()
    {
        for (var str = .0f; str < 60; str += Time.deltaTime * 300f)
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
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(0, 0, 0);
    }
}
