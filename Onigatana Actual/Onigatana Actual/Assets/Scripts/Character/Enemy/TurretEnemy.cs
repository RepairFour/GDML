using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    [Tooltip("How many attacks per second")]
    [SerializeField] float attackSpeed;
    [SerializeField] int damage;
    [Tooltip("How many times can the laser damage the player")]
    [SerializeField] int damgeTicks;
    [Tooltip("This is in seconds")]
    [SerializeField] float laserDuration;
    [SerializeField] List<Transform> lasers;
    [SerializeField] List<LineRenderer> lineRenderers;
    float timerMax;
    float timer = 0;
    bool firing = false;
    int damageCount = 0;
    List<Vector3> hitLocations = new List<Vector3>();

    [SerializeField] Material eyeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        timerMax = 1 / attackSpeed;
        eyeMaterial.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        if (!firing)
        {
            timer += Time.deltaTime;
            eyeMaterial.color = Color.Lerp(Color.white, Color.red,timer/2);
            lasers[0].GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.Lerp(Color.clear, Color.red, timer/2));
            if (timer > timerMax)
            {
                firing = true;
                timer = 0;
                damageCount = 0;
                timerMax = 1 / attackSpeed; //update this now - it might of changed
            }
        }
		else
		{
            timer += Time.deltaTime;
            if (timer < laserDuration)
            {
                FireLasers();
            }
            else
            {
                firing = false;
                timer = 0;
                foreach (var lineRender in lineRenderers)
                {
                    lineRender.enabled = false;
                }
            }
		}
    }

    private void FireLasers()
	{        
        
        //raycast
        foreach (var laser in lasers)
        {
            RaycastHit hit;
            if (Physics.Raycast(laser.position, laser.forward, out hit))
            {
                var player = hit.collider.gameObject.GetComponent<PlayerStats>();
                if (player != null && damageCount < damgeTicks)
                {
                    player.Hurt(damage);
                    ++damageCount;
                }
                hitLocations.Add(hit.point);
            }
        }
        //render line
        int i = 0;
        foreach(var lineRender in lineRenderers)
		{
            lineRender.enabled = true;
            lineRender.SetPosition(0, lasers[i].position);
            if (hitLocations.Count > 0)
            {           
                lineRender.SetPosition(1, hitLocations[i]);
            }
			else
			{
                lineRender.SetPosition(1, lasers[i].forward * 1000000);
            }
            ++i;
		}
    }
}
