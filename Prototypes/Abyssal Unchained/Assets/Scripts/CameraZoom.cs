using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float sizeConstant;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;
    [SerializeField] float zoomSpeed;
    TestBoss boss;
    Player player;
    [SerializeField] Transform refencePoint;
    Vector3 bossPosition;
    float distance;
    float maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        boss = FindObjectOfType<TestBoss>();
        bossPosition = boss.transform.position;
        maxDistance = bossPosition.x - refencePoint.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        distance = boss.transform.position.x - player.transform.position.x;

        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, sizeConstant * (distance / maxDistance), zoomSpeed);


        if (Camera.main.orthographicSize < minZoom)
		{
            Camera.main.orthographicSize = minZoom;

        }
        else if(Camera.main.orthographicSize > maxZoom)
		{
            Camera.main.orthographicSize = maxZoom;

        }
    }
}
