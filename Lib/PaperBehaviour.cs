using KeepCoding;
using Kernels;
using System;
using System.Collections;
using UnityEngine;

class PaperBehaviour : CacheableBehaviour
{

	[SerializeField]
	ComputeShader DrawlineShader;
	[SerializeField]
	ComputeShader MergeIconShader;
	[SerializeField]
	ComputeShader ApplyMaskShader;
	[SerializeField]
	Texture2D[] IconsTextures;

	DrawLineKernel drawLineKernel;
	DecayKernel decayKernel;
	MergeIconsKernel mergeIconsKernel;
	ApplyMaskKernel applyMaskKernel;
	Vector2? prev;
	RenderTexture mask;

	Icon[] icons = new Icon[3];

	internal bool isInteracted = false;
	private float colorOffset = 0;
	Color color;
	private Coroutine updatePaperCoroutine;
	private bool isSolved = false;

	void Start()
	{
		drawLineKernel = new DrawLineKernel(DrawlineShader);
		decayKernel = new DecayKernel(DrawlineShader);
		mergeIconsKernel = new MergeIconsKernel(MergeIconShader);
		applyMaskKernel = new ApplyMaskKernel(ApplyMaskShader);

		mask = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit(Get<Renderer>().material.mainTexture, mask);

		icons = new Icon[3];
		updatePaperCoroutine = StartCoroutine(UpdatePaper());
	}

	internal void SetIcons(int[] index, bool[] isClock)
    {
		for(var i = 0; i < 3; i++)
        {
			icons[i] = new Icon(IconsTextures[index[i]], isClock[i] ? 0.5f : -0.5f);
        }
    }

	private IEnumerator UpdatePaper()
	{
		yield return null;
		while (true)
		{
			foreach (var icon in icons) icon.Update();
			UpdateColor();
			if(isSolved) Draw(0);
			else if (isInteracted)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Get<MeshCollider>().Raycast(ray, out RaycastHit hit, 100.0f))
                {
                    Draw(hit.textureCoord * 256);
                } else
                {
					Draw(1);
                }
            }
			else
			{
				Draw(1);
			}
			yield return new WaitForSecondsRealtime(.01f);
		}
	}
	private void Draw(Vector2 p)
	{
		if (prev == null) prev = p;
		var tmp = drawLineKernel.Compute(mask, (Vector2)prev, p);
		Destroy(mask);
		mask = tmp;
		DrawBase();
		prev = p;
	}

	private void Draw(float fade)
	{
		prev = null;
		var tmp = decayKernel.Compute(mask, fade);
		Destroy(mask);
		mask = tmp;
		DrawBase();

	}

	private void DrawBase()
	{
		var unmasked = mergeIconsKernel.Compute(icons[0], icons[1], icons[2]);
		var tmp = Get<Renderer>().material.mainTexture;
		Get<Renderer>().material.mainTexture = applyMaskKernel.Compute(mask, unmasked, color);
		Destroy(unmasked);
		if(tmp is RenderTexture) Destroy(tmp);
	}

	private void UpdateColor()
	{
		colorOffset += Time.deltaTime * 2.8f;
		if (colorOffset > 3) colorOffset -= 3;
		if (colorOffset < 1) color = new Color(colorOffset, 0, 1 - colorOffset, 1);
		else if (colorOffset < 2) color = new Color(2 - colorOffset, colorOffset - 1, 0, 1);
		else color = new Color(0, 3 - colorOffset, colorOffset - 2, 1);

	}

	internal void HandleStrike(Action strike)
    {
		StartCoroutine(HandleStrikeCorutine(strike));

	}
	private IEnumerator HandleStrikeCorutine(Action strike)
	{
		yield return FadeIn();
		colorOffset = 1f;
		UpdateColor();
		strike();
		yield return FadeOut();

	}

	internal void HandleSolve(Action solve)
	{
		StartCoroutine(HandleSolveCorutine(solve));

	}
	private IEnumerator HandleSolveCorutine(Action solve)
	{
		yield return FadeIn();
		colorOffset = 2f;
		isSolved = true;
		solve();
		updatePaperCoroutine = StartCoroutine(UpdatePaper());

	}

	private IEnumerator FadeIn()
    {
		StopCoroutine(updatePaperCoroutine);
		Color initColor = color;
		for(var fade = 1f; fade > 0; fade -= Time.deltaTime * 0.3f)
        {
			var a = 1 - fade;
			color = new Color((1 - initColor.r)* a * a + initColor.r, (1 - initColor.g) * a * a + initColor.g, (1 - initColor.b) * a * a + initColor.b, 1);
			Draw(fade*fade);
			yield return null;
        }
	}

	private IEnumerator FadeOut()
	{

		for (var fade = 0f; fade < 1; fade += Time.deltaTime)
		{
			Draw(1);
			yield return null;
		}
		updatePaperCoroutine = StartCoroutine(UpdatePaper());

	}
}