using UnityEngine;

public class RandomizeAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animation anim = GetComponent<Animation>();
        AnimationState state = anim["CableLeaf"];

        // Pick a random normalized time between 0 and 1
        state.normalizedTime = Random.value;

        // Play the animation starting from that time
        anim.Play("CableLeaf");
    }
}
