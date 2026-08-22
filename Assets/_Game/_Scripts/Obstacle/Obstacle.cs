using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Material fadeMat; 
    [SerializeField] private Renderer rend;

    private Material originalMat;
    private bool isFaded;

    private void Awake()
    {
        originalMat = rend.material;
    }

    public void FadeOut()
    {
        if (fadeMat == null) Debug.Log("Empty fade mat");
        if (!isFaded)
        {
            rend.material = fadeMat;
            Debug.Log($"mat: {rend.material}");
            isFaded = true;
        }
    }
    public void FadeIn()
    {
        if (isFaded)
        {
            rend.material = originalMat;
            isFaded = false;
        }
    }
}
