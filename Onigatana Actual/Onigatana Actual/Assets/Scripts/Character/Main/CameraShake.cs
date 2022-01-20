using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0f;
	float shakeDurationTimer;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
		shakeDurationTimer = shakeDuration;
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	public IEnumerator ShakeCamera()
	{
		while (shakeDurationTimer > 0) {

			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDurationTimer -= Time.deltaTime;
			yield return null;
		}
			shakeDurationTimer = shakeDuration;
			camTransform.localPosition = originalPos;
		StopCoroutine(ShakeCamera());
		
	}
}