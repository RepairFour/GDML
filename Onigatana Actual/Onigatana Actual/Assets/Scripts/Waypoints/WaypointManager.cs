using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roaming
{
	[System.Serializable]
	public struct Path
	{
		public List<Transform> waypoints;
		public Color color;
	}
}

[ExecuteInEditMode]
public class WaypointManager : MonoBehaviour
{
	[SerializeField] GameObject wayPoint;
	
	
	public List<Roaming.Path> paths;
	[HideInInspector]
	public int pathIdx = 0;
	[HideInInspector]
	public List<string> pathNames = new List<string>();


	public Roaming.Path ReturnPath(Transform pathStartNode)
	{
        foreach(var path in paths)
		{
            foreach(var point in path.waypoints)
			{
				if(point == pathStartNode)
				{
					return path;
				}
			}
		}
		Debug.LogError("Transform Given, isn't a node");
		return new Roaming.Path();
	}

	private void Update()
	{
		int x = 0;
		pathNames.Clear();
		
		foreach (var path in paths)
		{
			pathNames.Add($"Path {x}");
			++x;
			path.waypoints.TrimExcess();
			for (int i = 0; i < path.waypoints.Count -1; ++i)
			{
				try
				{
					Debug.DrawLine(path.waypoints[i].position, path.waypoints[i + 1].position, path.color);
				}
				catch(MissingReferenceException)
				{
					if(path.waypoints[i] == null)
					{
						path.waypoints.RemoveAt(i);
					}
					else
					{
						path.waypoints.RemoveAt(i+1);
					}					
					
				}				
			}
		}
	}

	public void BuildWaypoint(int pathId)
	{
		
		if (paths[pathId].waypoints.Count > 0)
		{
			var newWaypoint = Instantiate(wayPoint, paths[pathId].waypoints[paths[pathId].waypoints.Count - 1].position, transform.rotation, transform);
			paths[pathId].waypoints.Add(newWaypoint.transform);
			newWaypoint.name = $"Path {pathId} - Waypoint {paths[pathId].waypoints.Count}";					
		}
		else
		{
			var newWaypoint = Instantiate(wayPoint, Vector3.zero, transform.rotation, transform);
			paths[pathId].waypoints.Add(newWaypoint.transform);
			newWaypoint.name = $"Path {pathId} - Waypoint {paths[pathId].waypoints.Count}";			
		}
		
	}

}
