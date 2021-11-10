using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void OpenSurvey()
	{
		Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScjRHMYLliSdp2VSjjZTNBqx94IVDUQekLMtG37yyzGliN98Q/viewform");
	}

}
