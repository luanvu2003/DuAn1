using UnityEngine;
using System.Collections;

namespace NetDinamica.AppFast
{
    public class IncreaseSortingOrder : MonoBehaviour
    {

        static int renderOrderOffset = 0;

        // Use this for initialization
        void Start()
        {
            renderOrderOffset += 1000;

            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.sortingOrder += renderOrderOffset;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}