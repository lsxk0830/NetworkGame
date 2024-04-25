using System.Threading;
using UnityEngine;

namespace Example
{
    public class Async : MonoBehaviour
    {
        private void Start()
        {
            Timer timer = new Timer(TimeOut, null, 5000, 0);
        }

        private void TimeOut(object state)
        {
            Debug.Log("铃铃铃");
        }
    }
}