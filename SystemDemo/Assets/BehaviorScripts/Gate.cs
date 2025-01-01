using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public string gateID; // Unique identifier for this gate
    public float targetHeight = 20f;
    public float gateSlideSpeed = 3f;

    [Header("Gate State")]
    public GateData gateData;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;

        if (gateData == null)
        {
            Debug.LogError($"GateData is not assigned to Gate {gateID}.");
            return;
        }

        bool isLocked = gateData.GetGateLockedState(gateID);
        if (isLocked)
        {
            Debug.Log($"Gate {gateID} is locked at start.");
        }
        else
        {
            Debug.Log($"Gate {gateID} is unlocked at start. Moving to open position.");
            MoveToOpenPosition();
        }
    }

    private void MoveToOpenPosition()
    {
        transform.position = initialPosition + new Vector3(0, targetHeight, 0);
    }

    public void UnlockGate()
    {
        if (gateData.SetGateLockedState(gateID, false))
        {
            Debug.Log($"Gate {gateID} is now unlocked in GateData.");
            StartCoroutine(SlideGateUp());
        }
    }

    private System.Collections.IEnumerator SlideGateUp()
    {
        Vector3 targetPosition = initialPosition + new Vector3(0, targetHeight, 0);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                gateSlideSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log($"Gate {gateID} is now fully open.");
    }
}

