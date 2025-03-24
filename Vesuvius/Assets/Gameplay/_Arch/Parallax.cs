using UnityEngine;

/*
*  External script courtesy of AdamCYounis @ Youtube
*  https://www.youtube.com/watch?v=tMXgLBwtsvI
*/
namespace Gameplay._Arch
{
    public class Parallax : MonoBehaviour
    {
        public Camera cam;
        public Transform subject;
        public float outOfBoundsYMin = -500;
        public float outOfBoundsYMax = 500;

        Vector2 startPos;
        float startZ;
        Vector2 travel => (Vector2)cam.transform.position - startPos;
        float distanceFromSubject => transform.position.z - subject.position.z;
        float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));
        float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

        [Range(0, 1)]
        public float verticalParallaxFactor = 0.1f; // Adjust to control vertical parallax strength

        void Start() {
            startPos = transform.position;
            startZ = transform.position.z;
        }

        void Update() {
            Vector2 newPos = startPos + new Vector2(travel.x * parallaxFactor, travel.y * verticalParallaxFactor);
            // Clamp vertical position to avoid out-of-bounds view
            float clampedY = Mathf.Clamp(newPos.y, startPos.y + outOfBoundsYMin, startPos.y + outOfBoundsYMax); // Adjust clamping values as needed
            transform.position = new Vector3(newPos.x, clampedY, startZ);
        }
    }
}
