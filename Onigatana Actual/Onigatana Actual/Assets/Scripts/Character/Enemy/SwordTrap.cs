using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrap : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float arcDegrees;
    Vector3 currentAngle;
    float startingZAxis;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentAngle = transform.eulerAngles;
        startingZAxis = currentAngle.z;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1 / speed)
		{
            timer = 0;
            arcDegrees = -arcDegrees;
        }
        currentAngle.z = Mathf.LerpAngle(currentAngle.z, startingZAxis + arcDegrees, EaseInQuint(timer * speed));
        transform.eulerAngles = currentAngle;
    }

    private float EaseOutCubic(float x)
	{
        return 1 - Mathf.Pow(1 - x, 3);
	}
    private float EaseInQuint(float x)
	{
        return x * x * x * x * x * x * x * x;
	}
}
