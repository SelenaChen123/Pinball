using UnityEngine;
using TMPro;

public class BumperController : MonoBehaviour
{
    public int displayNumber = 1;
    public Material positiveOff;
    public Material positiveOn;
    public Material negativeOff;
    public Material negativeOn;
    public TextMeshPro displayNumberText;
    public AudioSource bumperSound;

    MeshRenderer meshRenderer;
    bool collided = false;
    float collidedTimer = 0;
    bool alreadyCollided = false;
    float alreadyCollidedTimer = 0f;
    float offsetTime = 1.5f;

    // At the start of the game...
    private void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        bumperSound = GetComponent<AudioSource>();
        displayNumberText.text = "" + (this.gameObject.tag == "Positive" ? "+" : "-") + displayNumber.ToString();
    }

    // Before rendering each frame...
    void Update()
    {
        // assign material depending on whether bumper hit or not
        Material[] materials = meshRenderer.materials;

        if ((collided) && (collidedTimer < offsetTime))
        {
            if (this.gameObject.tag == "Positive")
            {
                materials[0] = positiveOn;
            }
            else
            {
                materials[0] = negativeOn;
            }

            collidedTimer += Time.deltaTime;
        }
        else
        {
            if (this.gameObject.tag == "Positive")
            {
                materials[0] = positiveOff;
            }
            else
            {
                materials[0] = negativeOff;
            }

            collided = false;
        }

        // prevent instantaneous multiple collisions on the same bumper
        if (alreadyCollided)
        {
            alreadyCollidedTimer += Time.deltaTime;
            if (alreadyCollidedTimer > offsetTime)
            {
                alreadyCollidedTimer = 0f;
                alreadyCollided = false;
            }
        }

        meshRenderer.materials = materials;
    }

    // On collision...
    void OnCollisionEnter(Collision myCollision)
    {
        if (myCollision.gameObject.tag == "Ball")
        {
            // add display amount to the answer talley in the PinballGame script
            if (!alreadyCollided)
            {
                GameObject.Find("Pinball Table").GetComponent<PinballGame>().collided(this.gameObject.tag == "Positive" ? displayNumber : 0 - displayNumber);
                alreadyCollided = true;
            }

            // trigger hit light (to change material assigned to bumper object so bumper "lights up") and reset timer 
            collided = true;
            collidedTimer = 0;

            bumperSound.volume = 0.2f;
            bumperSound.Play();
        }
    }
}
