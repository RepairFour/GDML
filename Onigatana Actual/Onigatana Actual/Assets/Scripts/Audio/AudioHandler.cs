using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioItem
{
    public string name;
    public AudioClip audio;
}
public class AudioHandler : MonoBehaviour
{
	[SerializeField] AudioSource musicAudio;
	[SerializeField] List<AudioSource> audioChannel;
	[SerializeField] List<AudioItem> audioItems;
    static public AudioHandler instance;

	Animator musicFadeIn;

	List<bool> canOverrideAudio;

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		canOverrideAudio = new List<bool>();
		for(int i = 0; i < audioChannel.Count; ++i)
		{
			canOverrideAudio.Add(true);
		}
		musicFadeIn = GetComponentInChildren<Animator>();
	}

	public void PlaySound(string _name, float volume = 0.5f, bool overrideable = true, int channel = 0)
	{
		if (channel < audioChannel.Count && channel >= 0)
		{
			if (canOverrideAudio[channel] || !audioChannel[channel].isPlaying)
			{
				canOverrideAudio[channel] = true;
				foreach (var item in audioItems)
				{
					if (item.name == _name)
					{
						audioChannel[channel].volume = volume;
						audioChannel[channel].clip = item.audio;
						audioChannel[channel].Play();
						canOverrideAudio[channel] = overrideable;
						break;
					}
				}
			}
		}
		else
		{
			Debug.Log("Channel out of range");
		}
	}
	public void PlaySoundUI(string _name)
	{
		foreach (var item in audioItems)
		{
			if (item.name == _name)
			{
				audioChannel[2].volume = 0.5f;
				audioChannel[2].clip = item.audio;
				audioChannel[2].Play();
				break;
			}
		}
	}

	public void ChangeMusic(string _name, bool loop = true)
	{
		foreach (var item in audioItems)
		{
			if (item.name == _name)
			{
				if (musicAudio.clip != item.audio)
				{
					//musicAudio.volume = volume;
					musicAudio.clip = item.audio;
					musicAudio.loop = loop;
					musicAudio.Play();
					musicFadeIn.SetTrigger("ChangeMusic");
				}
				break;
			}
		}
		
	}

}
