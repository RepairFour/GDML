using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GStompLvl1 : GalahadAction
{
    bool performAction = false;
    [SerializeField] GameObject stompHitBox;
    [SerializeField] float hitBoxSize;
    [SerializeField] float stompSpeed;
    [SerializeField] int dmg;
    [SerializeField] float chargeUpTimerMax;
    float chargeUpTimer = 0;
    Vector3 orginalScale;
    NavMeshAgent nav;
    Material mat;
    Color originalColor;
    StompStats stompStats;
	public override void Perform()
	{
        performAction = true;
        stompHitBox.SetActive(true);
        mat = GetComponent<MeshRenderer>().material;
        originalColor = mat.color;
        stompStats = stompHitBox.GetComponent<StompStats>();
        stompStats.dmg = dmg;
    }

	// Start is called before the first frame update
	void Start()
    {
        orginalScale = stompHitBox.transform.localScale;
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(performAction)
		{
            chargeUpTimer += Time.deltaTime;
            mat.color = Color.yellow;
            if (chargeUpTimer >= chargeUpTimerMax)
            {
                nav.destination = transform.position;
                stompHitBox.transform.localScale += new Vector3(Time.deltaTime * stompSpeed, 0, Time.deltaTime * stompSpeed);
                mat.color = originalColor;
                if (stompHitBox.transform.localScale.x > hitBoxSize)
                {
                    stompHitBox.SetActive(false);
                    stompHitBox.transform.localScale = orginalScale;
                    finished = true;
                    performAction = false;
                    stompStats.hitPlayer = false; //reset var for next time
                }
            }
		}
    }
}
