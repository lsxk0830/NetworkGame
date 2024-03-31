using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    protected bool isMoving = false; // �Ƿ������ƶ�
    protected Vector3 targetPosition; // �ƶ�Ŀ���
    public float speed = 1.2f; // �ƶ��ٶ�
    private Animator animator; // �������
    public string desc = ""; // ����

    /// <summary>
    /// �ƶ���ĳ��
    /// </summary>
    /// <param name="pos"></param>
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    /// <summary>
    /// �ƶ�Update
    /// </summary>
    public void MoveUpdate()
    {
        if (!isMoving) return;

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }

    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected void Update()
    {
        MoveUpdate();
    }
}