using UnityEngine;

namespace MWU.FilmLib
{

    [ExecuteInEditMode]
    public class LookAtTarget : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            // Rotate the object every frame so it keeps looking at the target
            transform.LookAt(target);
        }

    }
}