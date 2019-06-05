using MWU.FilmLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MWU.FilmLib
{
    public class PseudoSkel
    {
        public Transform source;
        public Transform dest;
    }

    [ExecuteInEditMode]
    public class PseudoSkeleton : MonoBehaviour
    {
        [Header("The parent Game Object that you want to attach a pseudo-skeleton to")]
        public Transform parentTransform;

        [Header("Add any 'top-level' objects to this list and we will make a skeleton node for it")]
        public List<Transform> meshList = new List<Transform>();

        public List<PseudoSkel> pseudoSkel = new List<PseudoSkel>();

        public bool debugDrawPseudoSkel = false;

        private Bounds combinedBounds;

        /// <summary>
        /// Setup the skeleton
        /// </summary>
        [ContextMenu("Initialize Pseudo Skeleton")]
        private void Init()
        {
            Cleanup();
            var pseudo = new GameObject();
            pseudo.name = "PseudoSkeleton";
            pseudo.transform.parent = transform;

            if( meshList.Count > 0)
            {
                foreach( var node in meshList)
                {
                    var go = new GameObject();
                    go.name = node.name;
                    go.transform.parent = pseudo.transform;

                    var newNode = new PseudoSkel();
                    newNode.source = node;
                    newNode.dest = go.transform;
                    pseudoSkel.Add(newNode);

                    UpdatePosition(newNode);                    
                }
            }
        }

        private void Update()
        {
            foreach( var node in pseudoSkel)
            {
                UpdatePosition(node);
            }
        }

        private void UpdatePosition(PseudoSkel node)
        {
            if (node == null)
                return;

            // figure out the bounding size of the children
            var parentMesh = node.source.GetComponent<MeshRenderer>();
            var meshes = node.source.GetComponentsInChildren<MeshRenderer>();
            combinedBounds = meshes[0].bounds;

            //Debug.Log("node: " + node.source.name + " has " + meshes.Length + " submeshes");
            foreach (var mr in meshes)
            {
                if (mr != parentMesh)
                {
                    Debug.Log("mesh: " + mr.name + " center: " + mr.bounds.center + " size: " + mr.bounds.size);
                    if (mr.bounds.size != Vector3.zero && mr.bounds.center != Vector3.zero)
                    {
                        combinedBounds.Encapsulate(mr.bounds);
                    }
                    else
                    {
                        Debug.Log("ignoring mesh: " + mr.name);
                    }
                }
            }

            // Debug.Log("node: " + node.source.name + " center: " + combinedBounds.center + " bounds: " + combinedBounds.size);

            // fix for domain reload being dumb
            if( node.dest.transform != null)
            {
                node.dest.transform.localPosition = combinedBounds.center;
                var box = node.dest.GetOrAddComponent<BoxCollider>();
                box.size = combinedBounds.size;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!debugDrawPseudoSkel)
                return;

            foreach( var node in pseudoSkel)
            {
                var coll = node.dest.GetComponent<BoxCollider>();
                if( coll)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(coll.bounds.center, coll.bounds.size);
                }
            }
        }


        private void Cleanup()
        {
            pseudoSkel.Clear();

            // cleanup pseudo if it exists already
            var pseudo = GetComponentsInChildren<Transform>().ToList();
            for (var j = 0; j < pseudo.Count; j++)
            {
                if (pseudo[j] != null)
                {
                    if (pseudo[j].name.Contains("PseudoSkeleton"))
                    // don't destroy ourself, but everything else
                    //if( propList[j] != transform && propList[j] != null)
                    {
                        DestroyImmediate(pseudo[j].gameObject);
                    }
                }
            }
        }
        
    }
}