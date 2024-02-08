using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CPortalCrosshair : CComponent
{
    [SerializeField] private CPortalPair portalPair;
    [SerializeField] private Texture2D originalTexture;
    private Rect m_cutRectB;
    private Rect m_cutRectO;
    private Rect m_cutRectBPlaced;
    private Rect m_cutRectOPlaced;
    private Image m_BCrosshairImage;
    private Image m_OCrosshairImage;
    private Image m_BPlacedCrosshairImage;
    private Image m_OPlacedCrosshairImage;
    private Sprite m_spriteB;
    private Sprite m_spriteO;
    private Sprite m_spriteBPlaced;
    private Sprite m_spriteOPlaced;


    public override void Awake()
    {
        base.Awake();

        m_BCrosshairImage = transform.GetChild(0).GetComponent<Image>();
        m_OCrosshairImage = transform.GetChild(1).GetComponent<Image>();
        m_BPlacedCrosshairImage = transform.GetChild(2).GetComponent<Image>();
        m_OPlacedCrosshairImage = transform.GetChild(3).GetComponent<Image>();
    }

    public override void Start()
    {
        base.Start();
        //97f, 145f, 193f, 30f
        m_cutRectB = new Rect(0f, 0f, 46f, 64f);
        m_cutRectO = new Rect(50f, 0f, 44f, 64f);
        m_cutRectBPlaced = new Rect(98f, 0f, 44f, 64f);
        m_cutRectOPlaced = new Rect(146f, 0f, 44f, 64f);

        if (m_BCrosshairImage && m_OCrosshairImage && m_BPlacedCrosshairImage && m_OPlacedCrosshairImage)
        {
            m_spriteB = Sprite.Create(originalTexture, m_cutRectB, new Vector2(0.5f, 0.5f));
            m_spriteO = Sprite.Create(originalTexture, m_cutRectO, new Vector2(0.5f, 0.5f));
            m_spriteBPlaced = Sprite.Create(originalTexture, m_cutRectBPlaced, new Vector2(0.5f, 0.5f));
            m_spriteOPlaced = Sprite.Create(originalTexture, m_cutRectOPlaced, new Vector2(0.5f, 0.5f));

            m_BCrosshairImage.sprite = m_spriteB;
            m_OCrosshairImage.sprite = m_spriteO;
            m_BPlacedCrosshairImage.sprite = m_spriteBPlaced;
            m_OPlacedCrosshairImage.sprite = m_spriteOPlaced;

            m_BCrosshairImage.color = ChangeColorByRGB(0, 162, 255);
            m_OCrosshairImage.color = ChangeColorByRGB(255, 154, 0);
            m_BPlacedCrosshairImage.color = ChangeColorByRGB(0, 162, 255);
            m_OPlacedCrosshairImage.color = ChangeColorByRGB(255, 154, 0);
        }
        else
            Debug.LogError("portal crosshair Image component not found");
    }

    public override void Update()
    {
        base.Update();
        
        if (portalPair.portals[0].IsPlaced())
        {
            m_BCrosshairImage.enabled = false;
            m_BPlacedCrosshairImage.enabled = true;
        }
        else
        {
            m_BCrosshairImage.enabled = true;
            m_BPlacedCrosshairImage.enabled = false;
        }
        if (portalPair.portals[1].IsPlaced())
        {
            m_OCrosshairImage.enabled = false;
            m_OPlacedCrosshairImage.enabled = true;
        }
        else
        {
            m_OCrosshairImage.enabled = true;
            m_OPlacedCrosshairImage.enabled = false;
        }
        
    }

    private Color ChangeColorByRGB(int r, int g, int b)
    {
        float normalizeR = r / 255f;
        float normalizeG = g / 255f;
        float normalizeB = b / 255f;

        Color c = new Color(normalizeR, normalizeG, normalizeB);

        return c;
    }

}
