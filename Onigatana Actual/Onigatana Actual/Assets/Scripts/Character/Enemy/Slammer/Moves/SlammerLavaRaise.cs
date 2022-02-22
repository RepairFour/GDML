using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerLavaRaise : SlammerMove
{
    [SerializeField] float lavaHeight;
    [SerializeField] float risingSpeed;
    [SerializeField] int waves;
    int waveCounter = 0;
    float risingTimer = 0;
    Vector3 endPos;
    Vector3 startPos;
    bool lowerLava;

	private void Start()
	{
        startPos = hitbox.transform.position;
        endPos = new Vector3(startPos.x, startPos.y + lavaHeight, startPos.z);
    }
	public override bool Activate()
	{
        hitbox.SetActive(true);
        risingTimer += Time.deltaTime;
        if (waveCounter < waves)
        {
            if (!lowerLava)
            {
                hitbox.transform.position = Vector3.Lerp(hitbox.transform.position, endPos, risingTimer * risingSpeed);
                if (Mathf.Abs((hitbox.transform.position - endPos).magnitude) < 0.1f)
                {
                    lowerLava = true;
                    risingTimer = 0;
                }
            }
            else
            {
                hitbox.transform.position = Vector3.Lerp(hitbox.transform.position, startPos, risingTimer * risingSpeed);
                if (Mathf.Abs((hitbox.transform.position - startPos).magnitude) < 0.1f)
                {
                    lowerLava = false;
                    risingTimer = 0;
                    waveCounter++;
                }
            }
        }
        else
        {            
            waveCounter = 0;
            hitbox.SetActive(false);
            return false;
        }
        return true;
    }
}
