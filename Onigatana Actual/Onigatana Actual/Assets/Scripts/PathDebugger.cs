using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(LineRenderer))]
public class PathDebugger : MonoBehaviour
{
    NavMeshAgent agent;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null)
        {
            if (agent.hasPath)
            {
                lineRenderer.positionCount = agent.path.corners.Length;
                lineRenderer.SetPositions(agent.path.corners);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
		else
		{
            Debug.Log("Cannot find NavMeshAgent in parent");
		}
    }
}
