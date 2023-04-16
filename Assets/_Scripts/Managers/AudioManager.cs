using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    AudioStorage soundStorage;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void PlaySound(SoundId Id)
    {
        AudioSource.PlayClipAtPoint(soundStorage.Get(Id), Vector3.zero);
    }
}
