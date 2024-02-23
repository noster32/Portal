using UnityEngine;

public static class SoundUtility
{
    public static float CalculateCollisionVolume(Rigidbody rigidbody)
    {
        float vel = rigidbody.velocity.magnitude;
        float volume;
        volume = 0.4f * ((vel = vel / 5f - 1) * vel * vel + 1) + 0f;

        return volume;
    }

    public static void PlayRandomSound(AudioSource audioSource, AudioClip[] audioClips)
    { 
        var rand = new System.Random();

        int num = rand.Next(audioClips.Length);
        audioSource.PlayOneShot(audioClips[num]);
    }

}
