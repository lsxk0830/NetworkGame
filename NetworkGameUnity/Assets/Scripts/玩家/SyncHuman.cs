using UnityEngine;

public class SyncHuman : BaseHuman
{
    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();
    }

    public void SyncAttack(float eulY)
    {
        transform.eulerAngles = new Vector3(0, eulY, 0);
        Attack();
    }
}