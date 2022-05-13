using UnityEngine;

public class FlipperController : MonoBehaviour
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float hitStrength = 20000f;
    public float flipperDamper = 100f;
    public KeyCode inputKey;

    HingeJoint hinge;

    // At the start of the game...
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;
    }

    // Before rendering each frame...
    void Update()
    {
        JointSpring spring = new JointSpring();
        spring.spring = hitStrength;
        spring.damper = flipperDamper;

        // flip flipper if input key is pressed
        if (Input.GetKey(inputKey) == true)
        {
            spring.targetPosition = pressedPosition;
        }
        else
        {
            spring.targetPosition = restPosition;
        }

        hinge.spring = spring;
        hinge.useLimits = true;
    }
}
