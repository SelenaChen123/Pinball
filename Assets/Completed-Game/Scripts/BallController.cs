using UnityEngine;

public class BallController : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip bounceClip;

    private Rigidbody rb;

    // At the start of the game...
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.Pause();
    }

    // On collision...
    void OnCollisionEnter(Collision myCollision)
    {
        // only generate collision sound on harder hits
        if (myCollision.relativeVelocity.magnitude > 10)
        {
            audioPlayer.PlayOneShot(bounceClip);
        }
    }
}