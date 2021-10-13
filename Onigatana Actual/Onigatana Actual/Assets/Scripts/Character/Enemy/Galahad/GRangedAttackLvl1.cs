using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRangedAttackLvl1 : GalahadAction
{
    bool performAction = false;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;
    [SerializeField] int dmg;
    [SerializeField] float chargeUpTimerMax;
    float chargeUpTimer = 0;
    Transform playerTrans;
    Material mat;
    Color objectColor;
	public override void Perform()
	{
        performAction = true;
	}

	// Start is called before the first frame update
	void Start()
    {
        playerTrans = FindObjectOfType<PlayerStats>().transform;
        projectile.GetComponent<EnemyProjectile>().dmg = dmg;
        mat = GetComponent<MeshRenderer>().material;
        objectColor = mat.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(performAction) //implement returning functionality
		{
            chargeUpTimer += Time.deltaTime;
            mat.color = Color.magenta;
            if (chargeUpTimer >= chargeUpTimerMax)
            {
                Vector3 pos = transform.position;
                pos.y += 7;
                var proj = Instantiate(projectile, pos, projectile.transform.rotation);
                proj.GetComponent<Rigidbody>().AddForce((playerTrans.position - pos).normalized * projectileSpeed);
                finished = true;
                performAction = false;
                mat.color = objectColor;
            }
		}
    }
}
