using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    int[] array1 = new int[5] { 1, 2, 4, 5, 6 };
    int[] array2 = new int[10];
    void Start()
    {
        Array.Copy(array1, 3, array2, 3, 2);
        foreach (var item in array2)
            Debug.Log(item);
    }
}
