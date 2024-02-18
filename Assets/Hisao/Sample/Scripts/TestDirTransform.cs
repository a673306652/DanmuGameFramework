using System.Collections;
using System.Collections.Generic;
using Hisao;
using UnityEngine;

namespace HisaoDemo
{
    public class TestDirTransform : MonoBehaviour
    {

        public Transform array1;
        public Transform array2;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var u = Input.GetKey(KeyCode.W) ? 1 : 0;
            var R = Input.GetKey(KeyCode.D) ? 1 : 0;
            var D = Input.GetKey(KeyCode.S) ? 1 : 0;
            var L = Input.GetKey(KeyCode.A) ? 1 : 0;
            var dir = new Vector3(R-L, u - D, 0);

            array1.up = dir;
            array2.up = dir.GlobalDir2LocalDir2D(Vector2.right);
        
        }
    }
}

