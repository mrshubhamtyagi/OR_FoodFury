using System.Collections;
using System.Collections.Generic;
using Fusion;
using FusionHelpers;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;

namespace FoodFury
{
    public class OrderMPEventBased : NetworkBehaviourWithState<OrderMPEventBased.NetworkState>
    {
        [Networked] public override ref NetworkState State => ref MakeRef<NetworkState>();

        public struct NetworkState : INetworkStruct
        {
            public TickTimer respawnTimer;
            public int activePowerupIndex;
        }

        [SerializeField] private Renderer _renderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _rechargeCircle;

        const float RESPAWN_TIME = 3f;

    }
}
