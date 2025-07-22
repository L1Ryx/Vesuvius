using _ScriptableObjects.PlayerInfo;
using UnityEngine;

public class GemFragment : MonoBehaviour
{
    public PlayerInfo playerInfo;

    public void CollectGemFragment()
    {
        playerInfo.AddGemFragment();
    }
}
