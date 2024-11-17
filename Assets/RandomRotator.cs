using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.up;
    [SerializeField] private List<float> rndRotations = new()
    {
        0f, 90f, 180f, 270f
    };
    private void OnEnable()
    {
        int rndIndex = Random.Range(0, rndRotations.Count);
        float rndRotation = rndRotations[rndIndex];
        transform.localRotation = Quaternion.Euler(axis * rndRotation);
    }
}
