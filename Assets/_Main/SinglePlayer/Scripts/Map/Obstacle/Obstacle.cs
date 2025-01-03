using System;
using UnityEngine;

namespace FoodFury
{
    public class Obstacle : MonoBehaviour
    {
        public event Action OnObstacleBecameInvisible;

        //[SerializeField] private Enums.ObstacleType type = Enums.ObstacleType.Still;
        [SerializeField] private Enums.ObstacleVarient varient = Enums.ObstacleVarient.Tree;
        [SerializeField] private bool becameVisible = false;

        [Header("-----Offset")]
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;

        [Header("-----ChildToReset")]
        [SerializeField] private Transform[] childsReset;
        private Vector3[] childsDefaultPosition = null;
        private Quaternion[] childsDefaultRotation = null;

        [Header("-----Animation")]
        [SerializeField] private ObstacleAnimation anim;


        private void InitDefaultChildrenSetup()
        {
            childsDefaultPosition = new Vector3[childsReset.Length];
            childsDefaultRotation = new Quaternion[childsReset.Length];
            for (int i = 0; i < childsReset.Length; i++)
            {
                childsDefaultPosition[i] = childsReset[i].localPosition;
                childsDefaultRotation[i] = childsReset[i].localRotation;
            }
        }


        public void Setup(Vector3 _tilePosition, Enums.Direction _riderDirection)
        {
            becameVisible = false;
            Debug.Log($"RiderDirection [{_riderDirection}] | TilePosition [{_tilePosition}]");
            Vector3 _finalPosition = Vector3.zero;
            Vector3 _finalRotation = Vector3.zero;

            switch (varient)
            {
                case Enums.ObstacleVarient.Tree:
                    switch (_riderDirection)
                    {
                        case Enums.Direction.Forward:
                            _finalPosition = new Vector3(positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, 90, 0);
                            break;

                        case Enums.Direction.Backward:
                            _finalPosition = new Vector3(-positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, -90, 0);
                            break;

                        case Enums.Direction.Left:
                            _finalPosition = new Vector3(0, positionOffset.y, positionOffset.x);
                            _finalRotation = new Vector3(0, 0, 0);
                            break;

                        case Enums.Direction.Right:
                            _finalPosition = new Vector3(0, positionOffset.y, -positionOffset.x);
                            _finalRotation = new Vector3(0, 180, 0);
                            break;
                    }
                    break;

                case Enums.ObstacleVarient.JCB:
                    switch (_riderDirection)
                    {
                        case Enums.Direction.Forward:
                            _finalPosition = new Vector3(positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, -45, 0);
                            break;

                        case Enums.Direction.Backward:
                            _finalPosition = new Vector3(-positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, 135, 0);
                            break;

                        case Enums.Direction.Left:
                            _finalPosition = new Vector3(0, positionOffset.y, positionOffset.x);
                            _finalRotation = new Vector3(0, 45, 0);
                            break;

                        case Enums.Direction.Right:
                            _finalPosition = new Vector3(0, positionOffset.y, -positionOffset.x);
                            _finalRotation = new Vector3(0, -135, 0);
                            break;
                    }
                    break;

                case Enums.ObstacleVarient.WoodTruck:
                    switch (_riderDirection)
                    {
                        case Enums.Direction.Forward:
                            _finalPosition = new Vector3(positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, 90, 0);
                            break;

                        case Enums.Direction.Backward:
                            _finalPosition = new Vector3(-positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, -90, 0);
                            break;

                        case Enums.Direction.Left:
                            _finalPosition = new Vector3(0, positionOffset.y, positionOffset.x);
                            _finalRotation = new Vector3(0, 0, 0);
                            break;

                        case Enums.Direction.Right:
                            _finalPosition = new Vector3(0, positionOffset.y, -positionOffset.x);
                            _finalRotation = new Vector3(0, 180, 0);
                            break;
                    }
                    break;

                case Enums.ObstacleVarient.CourierVan:
                    switch (_riderDirection)
                    {
                        case Enums.Direction.Forward:
                            _finalPosition = new Vector3(positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, 0, 0);
                            break;

                        case Enums.Direction.Backward:
                            _finalPosition = new Vector3(-positionOffset.x, positionOffset.y, 0);
                            _finalRotation = new Vector3(0, 180, 0);
                            break;

                        case Enums.Direction.Left:
                            _finalPosition = new Vector3(0, positionOffset.y, positionOffset.x);
                            _finalRotation = new Vector3(0, -90, 0);
                            break;

                        case Enums.Direction.Right:
                            _finalPosition = new Vector3(0, positionOffset.y, -positionOffset.x);
                            _finalRotation = new Vector3(0, 90, 0);
                            break; ;
                    }
                    break;
            }

            if (childsDefaultPosition == null || childsDefaultRotation == null)
                InitDefaultChildrenSetup();
            else
            {
                for (int i = 0; i < childsReset.Length; i++)
                {
                    childsReset[i].localPosition = childsDefaultPosition[i];
                    childsReset[i].localRotation = childsDefaultRotation[i];
                }
            }


            transform.SetPositionAndRotation(_tilePosition + _finalPosition, Quaternion.Euler(_finalRotation + rotationOffset));
            Invoke("ManualDespawn", 5);
        }

        private void OnBecameVisible()
        {
            if (anim != null) anim.PlaySingleAnimation();
            becameVisible = true;
            CancelInvoke("ManualDespawn");
        }

        private void OnBecameInvisible()
        {
            if (!becameVisible) return;

            OnObstacleBecameInvisible?.Invoke();
        }

        private void ManualDespawn()
        {
            if (becameVisible) return;

            OnObstacleBecameInvisible?.Invoke();
        }

        private void OnDestroy() => CancelInvoke("ManualDespawn");
    }

}