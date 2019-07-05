using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MWU.FilmLib
{
    public interface IBaseRecording
    {
        void SetTargetObject(GameObject targetObject);
        void SetTargetObject(GameObject targetObject, AvatarMask mask);
    }
}