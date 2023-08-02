using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class AudioClipsSO : ScriptableObject
{
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip noAmmoSound;
    public AudioClip[] stepSound;
    public AudioClip inhaleSound;
    public AudioClip exhaleSound;
    public AudioClip[] getHurtSound;
    public AudioClip heartbeatSound;
    public AudioClip deathSound;
    public AudioClip boneCrackSound;
    public AudioClip focusedLightStartup;
}
