using UnityEngine;

public class HammerBullet : BulletBase
{
    protected override void RotateVisual()
    {
        if(visual != null)
        {
            Vector3 rotateAxis = new Vector3(0, 0, 1);
            visual.Rotate(rotateAxis * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
}
