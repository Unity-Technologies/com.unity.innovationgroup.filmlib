using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MWU.Shared.Utilities
{
    /// <summary>
    /// Debug draw bones in the editor
    /// </summary>
    [ExecuteInEditMode]
    public class ShowBones : MonoBehaviour
    {
        private Transform rootNode;
        private List<Transform> childNodes = new List<Transform>();
        private Vector3 rootPos = Vector3.zero;
        public List<Transform> rootNodes = new List<Transform>();

        private void Start()
        {
            childNodes.Clear();
            PopulateChildren();
        }

        private void OnDrawGizmos()
        {
            PopulateChildren();

            foreach (var child in childNodes)
            {
                if (child.transform.position != rootPos)
                {
                    // if our parent isn't at the root position, then draw the skeleton
                    if( child.parent.position != rootPos && child.position != rootPos)
                    {
                        Gizmos.DrawLine(child.position, child.parent.position);
                    }

                    if( child.name.Contains("_LOC"))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(child.position, new Vector3(0.1f, .1f, .1f));
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(child.position, new Vector3(.025f, .025f, .025f));
                    }
                }
            }
        }

        private void PopulateChildren()
        {
            rootPos = transform.position;
            childNodes.Clear();
            foreach( var node in rootNodes)
            {
                childNodes.AddRange(node.GetComponentsInChildren<Transform>().ToList());
            }
        }
    }
}