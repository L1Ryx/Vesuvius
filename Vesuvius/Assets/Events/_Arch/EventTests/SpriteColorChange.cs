using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteColorChange : MonoBehaviour
{
    // refs
    private SpriteRenderer sr;

    // hidden vars
    private Color originalColor;

    // inspect vars
    [SerializeField] private Color newColor;
    [SerializeField] private float changeTime;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    /*
    Changes the object to a color for a specified time
    */
    public void ChangeColor() {
        StartCoroutine(_ChangeColor());
    }

    private IEnumerator _ChangeColor() {
        sr.color = newColor;
        yield return new WaitForSeconds(changeTime);
        sr.color = originalColor;
        yield return null;
    }
}
