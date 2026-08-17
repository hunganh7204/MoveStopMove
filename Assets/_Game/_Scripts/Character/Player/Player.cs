using UnityEngine;

public class Player : Character
{
    [SerializeField] private JoyStick joystick;

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
}