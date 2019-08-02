using System.Collections.Generic;
using UnityEngine;

public class RightSideParticleContainer : MonoBehaviour
{
    public Transform particlePrefab;
    public List<RectTransform> spawnPoints = new List<RectTransform>();

    void Start()
    {
        foreach (var point in spawnPoints)
        {
            var center = point.rect.center;

            var wp = point.TransformPoint(new Vector3(center.x, center.y, 0));
            wp += Vector3.back;
            wp += Vector3.up * .5f;
            Instantiate(particlePrefab, wp, particlePrefab.rotation);
        }
    }
}