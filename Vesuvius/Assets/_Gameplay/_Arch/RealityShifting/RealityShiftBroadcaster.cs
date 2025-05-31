using UnityEngine;
using UnityEngine.SceneManagement;

//Middle man broadcaster to gather shiftables in scene and then shift them.
//Used to avoid race conditions when accessing the reality state
public class RealityShiftBroadcaster : MonoBehaviour
{
    private IRealityShiftable[] realityShiftables;
    public GameState gameState;

    public float crossfadeDuration = 2f;

    void Awake()
    {
        realityShiftables = GetComponentsInChildren<IRealityShiftable>();
    }

    //Set all reality shiftable components to the correct reality at start of scene.
    void Start()
    {
        foreach(var realityShiftable in realityShiftables)
        {
            realityShiftable.RealityShiftInstantly(gameState.isAltReality);
        }  
    }

    public void Shift()
    {
        gameState.isAltReality = !gameState.isAltReality;
        foreach(var realityShiftable in realityShiftables)
        {
            realityShiftable.RealityShiftCrossFade(gameState.isAltReality,crossfadeDuration);
        }  
    }
}
