using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ground : NetworkBehaviour
{
    [SerializeField]
    private Texture2D baseTexture;

    private Texture2D cloneTexture;

    private SpriteRenderer spriteRenderer;

    private float _widthWorld, _heightWorld;
    private float _widthPixel, _heightPixel;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cloneTexture = Instantiate(baseTexture);

        cloneTexture.alphaIsTransparency = true;

        UpdateTexture();

        gameObject.AddComponent<PolygonCollider2D>();
    }

    /*public override void OnNetworkSpawn()
    {
        SetupPlayGroundServerRpc();
    }*/

    void UpdateTexture()
    {
        spriteRenderer.sprite = Sprite.Create(cloneTexture,
            new Rect(0, 0, cloneTexture.width, cloneTexture.height),
            new Vector2(0.5f, 0.5f), 120f);
    }

    private Vector2Int WorldToPixel(Vector2 pos)
    {
        Vector2Int v = Vector2Int.zero;

        float dx = (pos.x - transform.position.x);
        float dy = (pos.y - transform.position.y);

        v.x = Mathf.RoundToInt(0.5f * WidthPixel + dx * (WidthPixel / WidthWorld));
        v.y = Mathf.RoundToInt(0.5f * HeightPixel + dy * (HeightPixel / HeightWorld));

        return v;
    }

    public void ExplosionHole(CapsuleCollider2D col, float explosionRange)
    {
        Vector2Int c = WorldToPixel(col.bounds.center);

        int r = Mathf.RoundToInt(explosionRange * WidthPixel / WidthWorld);
        

        int px, nx, py, ny, d;

        for (int i = 0; i <= r; i++)
        {
            d = Mathf.RoundToInt(Mathf.Sqrt(r * r - i * r));

            for (int j = 0; j <= d; j++)
            {
                px = c.x + i;
                nx = c.x - i;
                py = c.y + j;
                ny = c.y - j;

                cloneTexture.SetPixel(px, py, Color.clear);
                cloneTexture.SetPixel(nx, py, Color.clear);
                cloneTexture.SetPixel(px, ny, Color.clear);
                cloneTexture.SetPixel(nx, ny, Color.clear);
            }
        }    

        cloneTexture.Apply();
        UpdateTexture();

        Destroy(gameObject.GetComponent<PolygonCollider2D>());

        gameObject.AddComponent<PolygonCollider2D>();
    }

    public float WidthWorld
    {
        get
        {
            if(_widthWorld == 0)
                _widthWorld = spriteRenderer.bounds.size.x;
            return _widthWorld;
        }
    }

    public float HeightWorld
    {
        get
        {
            if(_heightWorld == 0)
                _heightWorld = spriteRenderer.bounds.size.y;
            return _heightWorld;
        }
    }

    public float WidthPixel
    {
        get
        {
            if (_widthPixel == 0)
                _widthPixel = spriteRenderer.sprite.texture.width;
            return _widthPixel;
        }
    }

    public float HeightPixel
    {
        get
        {
            if (_heightPixel == 0)
                _heightPixel = spriteRenderer.sprite.texture.height;
            return _heightPixel;
        }
    }

    [ServerRpc]
    public void SetupPlayGroundServerRpc()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cloneTexture = Instantiate(baseTexture);

        cloneTexture.alphaIsTransparency = true;

        UpdateTexture();

        SetupPlayGroundClientRpc();
    }

    [ClientRpc]
    public void SetupPlayGroundClientRpc() 
    {
        gameObject.AddComponent<PolygonCollider2D>();
    }

}
