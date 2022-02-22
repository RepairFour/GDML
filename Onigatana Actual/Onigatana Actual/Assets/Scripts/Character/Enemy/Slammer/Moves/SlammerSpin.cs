using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammerSpin : SlammerMove
{
    [SerializeField] float startSpinSpeed = 90f;
    [SerializeField] float spinSpeedIncrease = 10;
    [SerializeField] float maxSpinSpeed = 450f;
    [SerializeField] float spinDuration = 12f;

    float spinTimer = 0;
    float speedUptimer = 0;
    public override bool Activate()
    {
        hitbox.SetActive(true);
        speedUptimer += Time.deltaTime;
        if (speedUptimer >= 1)
        {
            speedUptimer = 0;
            startSpinSpeed += spinSpeedIncrease;
            if (startSpinSpeed > maxSpinSpeed)
            {
                startSpinSpeed = maxSpinSpeed;
            }
        }
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * startSpinSpeed);

        spinTimer += Time.deltaTime;
        if (spinTimer >= spinDuration)
        {
            hitbox.SetActive(false);
            spinTimer = 0;
            return false;
        }
        return true;
    }
}
