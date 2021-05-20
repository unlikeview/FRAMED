using TMPro;
using UnityEngine;

public class PlacementObject : MonoBehaviour
{
    public Material newMaterial;
    public Material oldMaterial;

    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skMeshRenderer;

    [SerializeField]
    private bool IsSelected;

    [SerializeField]
    private bool IsLocked;

    public bool Selected
    {
        get
        {
            return this.IsSelected;
        }
        set
        {
            IsSelected = value;
        }
    }

    public bool Locked
    {
        get
        {
            return this.IsLocked;
        }
        set
        {
            IsLocked = value;
        }
    }

    [SerializeField]
    private TextMeshPro OverlayText;

    [SerializeField]
    private Canvas canvasComponent;

    [SerializeField]
    private string OverlayDisplayText;

    void Awake()
    {
        OverlayText = canvasComponent.GetComponentInChildren<TextMeshPro>();

    }
    public void SetOverlayText(string text)
    {
        OverlayText.text = text;
        OverlayText.gameObject.SetActive(true);
    }

    void Start()
    {
        ChangeSelectedObject();
    }

    public void ChangeSelectedObject()
    {
        MeshRenderer[] mss = GetComponentsInChildren<MeshRenderer>();
        foreach (var ms in mss)
        {
            ms.material = newMaterial;
        }

        SkinnedMeshRenderer[] skMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        {
            foreach (var sk in skMeshRenderer)
            {
                sk.material = newMaterial;
            }
        }
    }

    public void BackToBeginColor()
    { 
        MeshRenderer[] mss = GetComponentsInChildren<MeshRenderer>();
        foreach (var ms in mss)
        {
            ms.material = oldMaterial;
        }
        SkinnedMeshRenderer[] skMeshRenderer2 = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var sk2 in skMeshRenderer2)
        {
            sk2.material = oldMaterial;
        }
    }

}


