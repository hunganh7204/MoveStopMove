using UnityEngine;
using UnityEngine.EventSystems;
public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform joystickBase;
    [SerializeField] private RectTransform joystickHandle;

    [Header("Settings")]
    [SerializeField] private float moveRadius = 100f;

    private Vector2 inputDirection;

    public float Horizontal { get { return inputDirection.x; } }
    public float Vertical { get { return inputDirection.y; } }
    public Vector2 Direction { get { return inputDirection; } }

    private void Awake()
    {
        HideJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       

        joystickBase.gameObject.SetActive(true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        joystickBase.localPosition = localPoint;

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBase,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 handleLocalPoint);

        if (handleLocalPoint.magnitude > moveRadius)
        {
            handleLocalPoint = handleLocalPoint.normalized * moveRadius;
        }

        joystickHandle.localPosition = handleLocalPoint;
        inputDirection = handleLocalPoint / moveRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HideJoystick();
    }

    private void HideJoystick()
    {
        inputDirection = Vector2.zero;
        joystickHandle.localPosition = Vector2.zero;
        joystickBase.gameObject.SetActive(false);
    }
}
