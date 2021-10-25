using UnityEngine;

class Icon
{
    public RenderTexture texture;
    public float rotation;
    readonly float speed;
    internal Icon(Texture2D icon, float speed)
    {
        texture = new RenderTexture(192, 192, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(icon, texture);
        rotation = 0;
        this.speed = speed;
    }

    internal void Update()
    {
        rotation += speed * Time.deltaTime;
    }
}

