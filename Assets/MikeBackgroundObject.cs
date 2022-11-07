using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike
{
    public class MikeBackgroundObject : MonoBehaviour
    {
        Sprite sprite;

        float depth;
        float Depth { get { return depth + (depth == 0 ? 1 : 0); } }

        SortingLayer layer;
        
        int orderInLayer;
        MikeParallaxBackground parallaxManager;

        Vector2 SpriteWorldSize { get => new(sprite.texture.width / sprite.pixelsPerUnit, sprite.texture.height / sprite.pixelsPerUnit); }

        Vector2 CamPos { get => Camera.main.transform.position; }
        Vector2 CameraSize
        {
            get
            {
                return new Vector2(Camera.main.orthographicSize * 2, (Camera.main.orthographicSize * Camera.main.pixelWidth / Camera.main.pixelHeight) * 2);
            }
        }

        Vector2 VisibilityDistance { get => CameraSize / 2 + SpriteWorldSize / 2; }

        Vector2Int MinObjectsInCamera { get => new(Mathf.CeilToInt(CameraSize.x / SpriteWorldSize.x), Mathf.CeilToInt(CameraSize.y / SpriteWorldSize.y)); }

        List<SpriteRenderer> backgrounds = new();


        //------------

        private void Start()
        {
            SpawnInitialBackgrounds();
        }

        private void LateUpdate()
        {
            transform.position = CamPos / Depth;

            CoverVisibleAreas();

            DeactivateUsedBackground();
        }

        private void CoverVisibleAreas()
        {
            
        }


        //------------

        void DeactivateUsedBackground()
        {
            foreach (SpriteRenderer background in backgrounds)
            {
                if (!IsVisible(background.transform.position))
                {
                    background.enabled = false;
                }
            }
        }

        bool IsVisible(Vector2 pos)
        {
            Vector2 diffAbs = new(Mathf.Abs(pos.x - CamPos.x), Mathf.Abs(pos.y - CamPos.y));

            return diffAbs.x <= VisibilityDistance.x && diffAbs.y <= VisibilityDistance.y;
        }

        public void Set(MikeParallaxBackground.ParallaxLayer layerData, int orderInLayer)
        {

        }

        void SpawnInitialBackgrounds()
        {
            int minX = -Mathf.FloorToInt(MinObjectsInCamera.x / 2);
            int maxX = Mathf.CeilToInt(MinObjectsInCamera.x / 2);

            int minY = -Mathf.FloorToInt(MinObjectsInCamera.y / 2);
            int maxY = Mathf.CeilToInt(MinObjectsInCamera.y / 2);


            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    SpriteRenderer background = CreateNewBackground();
                }
            }
        }

        /// <summary>
        /// Gets the first background that is NOT enabled, enables it and retuns it. 
        /// If no background was found, a new one is created, added and returned!
        /// </summary>
        /// <returns>First unused <see cref="SpriteRenderer"/> from <see cref="backgrounds"/></returns>
        SpriteRenderer GetUnusedBackground()
        {
            foreach (SpriteRenderer bg in backgrounds)
            {
                if (!bg.enabled) { bg.enabled = true; return bg; }
            }

            return CreateNewBackground();
        }

        SpriteRenderer CreateNewBackground()
        {
            return CreateNewBackground(Vector2.zero);
        }
        SpriteRenderer CreateNewBackground(Vector2 position)
        {
            SpriteRenderer bg = new GameObject("Background").AddComponent<SpriteRenderer>();

            bg.transform.SetParent(transform, false);
            bg.transform.position = position;
            bg.sprite = sprite;
            bg.sortingLayerID = layer.id;
            bg.sortingOrder = orderInLayer;

            backgrounds.Add(bg);
            return bg;
        }
    }
}
