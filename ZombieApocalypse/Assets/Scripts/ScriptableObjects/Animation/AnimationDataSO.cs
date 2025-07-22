using UnityEngine;


[CreateAssetMenu()]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        None,
        Idle,
        Walk
    }

    public AnimationType animationType;
    
    public float frameTimerMax;
    public Mesh[] meshArray;
}
