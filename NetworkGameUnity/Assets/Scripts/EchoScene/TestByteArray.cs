using System;
using UnityEngine;

public class TestByteArray:MonoBehaviour
{
    void Start()
    {
        // 创建
        ByteArray buff = new ByteArray(8);
        Debug.Log($"[1 Debug] -> {buff.Debug()}");
        Debug.Log($"[1 string] -> {buff.ToString()}");

        // write
        byte[] wb = new byte[]{1,2,3,4,5};
        buff.Write(wb,0,5);
        Debug.Log($"[2 Debug] -> {buff.Debug()}");
        Debug.Log($"[2 string] -> {buff.ToString()}");

        // read
        byte[] rb = new byte[4];
        buff.Read(rb,0,2);
        Debug.Log($"[3 Debug] -> {buff.Debug()}");
        Debug.Log($"[3 string] -> {buff.ToString()}");
        Debug.Log($"[3 rb] -> {BitConverter.ToString(rb)}");

        // write resize
        wb = new byte[]{6,7,8,9,10,11};
        buff.Write(wb,0,6);
        Debug.Log($"[4 Debug] -> {buff.Debug()}");
        Debug.Log($"[4 string] -> {buff.ToString()}");
    }
}