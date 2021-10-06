using System;
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
		[HideInInspector]
		public List<GameObject> waypointHistory;
		public Path(Color color)
		{
			waypoints = new List<Transform>();
			waypointHistory = new List<GameObject>();
			this.color = color;
		}
	}
}

[ExecuteInEditMode]
public class WaypointManager : MonoBehaviour
{
	[SerializeField] GameObject wayPoint;


	public List<Roaming.Path> paths = new List<Roaming.Path>();
	[HideInInspector] public int pathIdx = 0;
	[HideInInspector] public List<string> pathNames = new List<string>();
	[SerializeField] bool placeWithCamera;

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

	public int PathID(Roaming.Path p)
	{
		int i = 0;
		foreach(var path in paths)
		{
			if(path.Equals(p))
			{
				return i;
			}
			++i;
		}
		Debug.LogError("Path Pass Invalid");
		return 0;
	}

	private void Update()
	{
		int x = 0;
		pathNames.Clear();

		foreach (var path in paths)
		{
			if (path.waypointHistory.Count != path.waypoints.Count)
			{
				List<GameObject> toDelete = new List<GameObject>();
				foreach(var go in path.waypointHistory)
				{
					if(!path.waypoints.Contains(go.transform))
					{
						toDelete.Add(go);
					}
				}
				for(int i = 0; i < toDelete.Count; ++i)
				{
					path.waypointHistory.Remove(toDelete[i]);
					DestroyImmediate(toDelete[i]);					
				}
				toDelete.Clear();
				//rename waypoints
				UpdateWaypointNames();
			}



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
				catch(NullReferenceException)
				{
					if (path.waypoints[i] == null)
					{
						path.waypoints.RemoveAt(i);
					}
					else
					{
						path.waypoints.RemoveAt(i + 1);
					}
				}
			}
		}
	}

	public void BuildWaypoint(int pathId)
	{
		Transform sceneCameraTransform = UnityEditor.SceneView.lastActiveSceneView.camera.transform;
		Vector3 waypointPos = sceneCameraTransform.position + sceneCameraTransform.forward * 10;
		if (paths[pathId].waypoints.Count > 0)
		{
			if (paths[pathId].waypoints[paths[pathId].waypoints.Count - 1] == null) //this fixes an issue where an empty list of one item would cause a nullreference
			{
				paths[pathId].waypoints.RemoveAt(paths[pathId].waypoints.Count - 1);
				InstantiateWayPoint(pathId, waypointPos);
				
			}
			else
			{
				if (placeWithCamera)
				{
					InstantiateWayPoint(pathId, waypointPos);
				}
				else
				{
					InstantiateWayPoint(pathId, paths[pathId].waypoints[paths[pathId].waypoints.Count - 1].position);
				}
			}
		}
		else
		{
			InstantiateWayPoint(pathId, waypointPos);
			Debug.Log(waypointPos);
		}
		
	}

	private void InstantiateWayPoint(int pathId,Vector3 pos)
	{
		var newWaypoint = Instantiate(wayPoint, pos, transform.rotation, transform);
		paths[pathId].waypoints.Add(newWaypoint.transform);
		paths[pathId].waypointHistory.Add(newWaypoint);
		UpdateWaypointNames();
	}

	private void UpdateWaypointNames()
	{
		foreach(var path in paths)
		{
			for (int i = 0; i < path.waypoints.Count; ++i)
			{
				path.waypoints[i].name = $"Path {PathID(path)} - Waypoint {i}";
				path.waypointHistory[i].name = $"Path {PathID(path)} - Waypoint {i}";
			}
		}
	}

	public void BuildPath()
	{
		paths.Add(new Roaming.Path(Color.yellow));
	}

}
