using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mike
{
    // I'm leaving this to you future Mike
    // as I am to tired to figure this out
    // UGH... why do I need to make things optmized and super modular
    // I miss the old days when i would just hardcode everything
    // And then give up cuz my code was spaghetti :C
    public class MikeParallaxBackground : MonoBehaviour
    {
        [System.Serializable]
        struct ParalaxLayer
        {
            [HideInInspector] public Transform transform;

            [SerializeField] Sprite bgSprite;
            [SerializeField] float depth;

            Transform layerParent;
            public Transform LayerParent
            {
                get
                {
                    if (layerParent == null)
                    {
                        layerParent = new GameObject("BackgroundHolder" + depth).transform;
                        layerParent.SetParent(transform);
                        layerParent.localPosition = Vector3.zero;
                        layerParent.localScale = Vector3.one;
                        layerParent.rotation = Quaternion.identity;
                    }

                    return layerParent;
                }
            }

            float CameraHeight { get => Camera.main.orthographicSize; }
            float CameraWidth { get => CameraHeight * Camera.main.pixelWidth / Camera.main.pixelHeight; }

            //IDK if I'll use this later or not. I'll just leave it just in case
            float CameraBoundUp { get => Camera.main.transform.position.y + CameraHeight; }
            float CameraBoundRight { get => Camera.main.transform.position.x + CameraWidth; }
            float CameraBoundDown { get => Camera.main.transform.position.y - CameraHeight; }
            float CameraBoundLeft { get => Camera.main.transform.position.x - CameraWidth; }

            float BGWidth { get => bgSprite.texture.width / bgSprite.pixelsPerUnit; }

            Vector2 CamPos { get => Camera.main.transform.position; }

            //--------------

            List<SpriteRenderer> bgs;
            List<SpriteRenderer> Bgs { get { if (bgs == null) { bgs = new(); } return bgs; } set => bgs = value; }

            Vector2 GetDifference(Vector2 pos)
            {
                return (CamPos - pos);
            }

            public void Initialize(Transform transform)
            {
                lastCoveredPos = Vector2.positiveInfinity;
                this.transform = transform;

                layerParent = new GameObject("Layer").transform;
                layerParent.SetParent(transform, false);
            }

            SpriteRenderer GetFreeBG()
            {
                if(Bgs == null) { Bgs = new(); }

                for (int i = 0; i < Bgs.Count; i++)
                {
                    if (!Bgs[i].enabled) 
                    { 
                        Bgs[i].enabled = true; 
                        return Bgs[i]; 
                    }
                }

                return CreateNewBG();
            }

            SpriteRenderer CreateNewBG()
            {
                SpriteRenderer newBG = new GameObject("BG").AddComponent<SpriteRenderer>();
                newBG.transform.SetParent(LayerParent, false);
                newBG.sprite = bgSprite;
                Bgs.Add(newBG);

                return newBG;
            }

            void DeactivateBGs()
            {
                for (int i = 0; i < Bgs.Count; i++)
                {
                    if(Mathf.Abs(GetDifference(Bgs[i].transform.position).x) > BGWidth + CameraWidth)
                    {
                        Bgs[i].enabled = false;
                    }
                }
            }

            Vector2 SetLayerParentPosition(bool x, bool y)
            {
                if (x)
                {
                    float newX = CamPos.x * -depth;
                    layerParent.position = new Vector3(newX, layerParent.position.y, 0);
                }

                return layerParent.position;
            }

            Vector2 lastCoveredPos;
            Vector2 lastCamPosition;
            public void UpdatePosition(bool x, bool y)
            {
                if(Mathf.Sign(lastCamPosition.x) != Mathf.Sign(lastCoveredPos.x)) { lastCoveredPos = CamPos; }

                Vector2 difference = GetDifference(lastCoveredPos) * depth;

                SetLayerParentPosition(x, y);
                DeactivateBGs();

                if (Mathf.Abs(difference.x) >= BGWidth / 2)
                {
                    lastCoveredPos = CamPos;
                    GetFreeBG().transform.position = CamPos + new Vector2(BGWidth / 2 + CameraWidth, 0) * Mathf.Sign(difference.x);
                }

                lastCamPosition = CamPos;
            }
        }


        //-----------------


        [SerializeField] ParalaxLayer[] layers;
        [SerializeField] SortingLayer sortingLayer;

        [SerializeField] bool xAxis;
        [SerializeField] bool yAxis;


        //----------------


        private void Start()
        {
            Initialize();
        }
        void LateUpdate()
        {
            UpdateBGs();
        }


        //--------------


        private void Initialize()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Initialize(transform);
            }
        }

        void UpdateBGs()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].UpdatePosition(xAxis, yAxis);
            }
        }
    }
}
