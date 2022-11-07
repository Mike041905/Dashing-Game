using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike
{
    [ExecuteAlways]
    public class MikeParallaxBackground : MonoBehaviour
    {
        public struct ParallaxLayer
        {
            public Sprite sprite;

            public SortingLayer sortingLayer;
            public float depth;

            public ParallaxLayer(Sprite sprite, SortingLayer sortingLayer, float depth)
            {
                this.sprite = sprite;
                this.sortingLayer = sortingLayer;
                this.depth = depth;
            }
        }


        [SerializeField] ParallaxLayer[] layers = new ParallaxLayer[1] { new(null, SortingLayer.layers[0], 0) };
        public ParallaxLayer[] Layers { get => layers; set { layers = value; SetBackgroundObjects(); } }


        List<MikeBackgroundObject> backgrounds = new();


        //------------------------


        private void Awake()
        {
            SetBackgroundObjects();
        }


        //---------------------


        // Issue: This sets orderInLayer based on index and not actual layer
        // TODO: Fix this!
        void SetBackgroundObjects()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                MikeBackgroundObject background;

                if (i < backgrounds.Count)
                {
                    background = backgrounds[i];
                }
                else
                {
                    background = new GameObject("BackgroundLayer").AddComponent<MikeBackgroundObject>();
                    background.transform.SetParent(transform, false);
                }

                background.Set(Layers[i], i);
                backgrounds.Add(background);
            }
        }
    }
}
