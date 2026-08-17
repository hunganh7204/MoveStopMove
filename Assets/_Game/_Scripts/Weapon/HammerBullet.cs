using UnityEngine;

public class HammerBullet : BulletBase
{
    protected override void RotateVisual()
    {
        if(visual != null)
        {
            visual.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
