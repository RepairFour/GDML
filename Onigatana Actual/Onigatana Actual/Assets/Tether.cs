using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tether : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] Material inRangedMat;
    [SerializeField] Material maxRangedMat;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void MaxRanged()
	{
        lineRenderer.material = maxRangedMat;
	}
    public void InRange()
	{
        lineRenderer.material = inRangedMat;

    }
}
