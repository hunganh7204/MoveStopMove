using UnityEngine;
using UnityEngine.Video;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private bool isDestroyOnClose = false;

    private void Awake()
    {
        //xu ly tai tho
        RectTransform rect = GetComponent<RectTransform>();
        float ratio = (float)Screen.width / (float)Screen.height;
        if (ratio > 2.1f)
        {
            Vector2 leftBottom = rect.offsetMin;
            Vector2 rightTop = rect.offsetMax;

            leftBottom.y = 0f;
            rightTop.y = -100f;

            rect.offsetMin = leftBottom;
            rect.offsetMax = rightTop;
        }
    }

    //Goi truoc khi canvas duoc active
    public virtual void Setup()
    {
        // Setup logic for the UI canvas
    }

    //goi khi canvas duoc active
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    //tat canvas sau 1 khoang time
    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirectly), time);
    }

    //tat canvas truc tiep
    public virtual void CloseDirectly()
    {
        if (isDestroyOnClose)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            gameObject.SetActive(false);
        }    
    }
}
