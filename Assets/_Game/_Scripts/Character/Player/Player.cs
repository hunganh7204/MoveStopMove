using UnityEngine;

public class Player : Character
{
    [SerializeField] private JoyStick joystick;
    [SerializeField] private CameraFollow cam;

    private void Start()
    {
        OnInit();
    }

    private void FixedUpdate()
    {
        if (IsDead()) return;

        Vector3 moveDirection = new Vector3(joystick.Direction.x, 0f, joystick.Direction.y);
        Move(moveDirection);

        if (!isMoving)
        {
            CleanUpTargets();
            if (HasTargets())
            {
                TryStartAttack(GetFirstTarget());
            }
        }
    }

    protected override void LevelUp()
    {
        base.LevelUp();
        cam.UpdateZoom(TF.localScale.x);
    }
}