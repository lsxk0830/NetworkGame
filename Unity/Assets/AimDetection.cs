using UnityEngine;

public class AimDetection : MonoBehaviour
{
    [Tooltip("This object represents the aim target.  We always point toeards this")]
    public Transform AimTarget;

    public Vector3 offset;

    private void Start()
    {
        offset = transform.localEulerAngles;
    }

    void Update()
    {
        // 瞄准目标
        if (AimTarget == null)
            return;
        var dir = AimTarget.position - transform.position;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir - offset);
    }
}
