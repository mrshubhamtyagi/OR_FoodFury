using System;
using UnityEngine;

namespace FoodFury
{
    public class FinishingPoint : MonoBehaviour
    {
        public event Action OnReached;
        public float delay;
        public float timeout;
        public bool isInside = false;

        public Color defaultColor = Color.yellow;
        public Color enterColor = Color.green;

        private Renderer pointRenderer;

        private void Awake() => pointRenderer = GetComponent<Renderer>();


        void OnEnable()
        {
            timeout = delay;
            isInside = false;
            pointRenderer.material.color = defaultColor;
        }


        private void Update()
        {
            if (!isInside) return;

            if (timeout < 0)
            {
                isInside = false;
                OnReached?.Invoke();
            }
            else timeout -= Time.deltaTime;

        }


        private void OnTriggerEnter(Collider other)
        {
            if (delay == 0) OnReached?.Invoke();
            else
            {
                isInside = true;
                pointRenderer.material.color = enterColor;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isInside = false;
            timeout = delay;
            pointRenderer.material.color = defaultColor;
        }
    }

}