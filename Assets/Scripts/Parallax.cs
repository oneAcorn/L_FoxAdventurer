using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform camera;
    public float moveRate;
    [Tooltip("是否锁定Y轴跟随")] public bool lockY;

    private float startPointX, startPointY;

    private void Start()
    {
        startPointX = transform.position.x;
        startPointY = transform.position.y;
    }

    private void Update()
    {
        if (lockY)
        {
            transform.position = new Vector2(startPointX + camera.position.x * moveRate, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(startPointX + camera.position.x * moveRate,
                startPointY + camera.position.y * moveRate);
        }
    }
}