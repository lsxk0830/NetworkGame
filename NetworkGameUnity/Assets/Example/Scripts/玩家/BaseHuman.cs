using UnityEngine;

namespace Example
{
    public class BaseHuman : MonoBehaviour
    {
        protected bool isMoving = false; // 是否正在移动
        protected Vector3 targetPosition; // 移动目标点
        public float speed = 1.2f; // 移动速度
        private Animator animator; // 动画组件
        public string desc = ""; // 描述
        internal bool isAttacking = false; // 是否攻击
        internal float attackTime = float.MinValue;

        /// <summary>
        /// 移动到某处
        /// </summary>
        public void MoveTo(Vector3 pos)
        {
            targetPosition = pos;
            isMoving = true;
            animator.SetBool("isMoving", true);
        }

        /// <summary>
        /// 移动Update
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

        /// <summary>
        /// 攻击
        /// </summary>
        public void Attack()
        {
            isAttacking = true;
            attackTime = Time.time;
            animator.SetBool("isAttacking", true);
        }

        /// <summary>
        /// 攻击Update
        /// </summary>
        public void AttackUpdate()
        {
            if (!isAttacking) return;
            if (Time.time - attackTime < 1.2f) return;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }

        protected void Start()
        {
            animator = GetComponent<Animator>();
        }

        protected void Update()
        {
            MoveUpdate();
            AttackUpdate();
        }
    }
}