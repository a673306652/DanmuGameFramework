using System.Collections.Generic;

namespace Modules.Rigging
{
    using System;
    using UnityEngine;

    public class MeshSockets : MonoBehaviour
    {
        [Header("Pre-register Sockets")]
        [SerializeField]
        private MeshSocket[] _SocketRegistries;

        private Dictionary<SocketId, MeshSocket>
            socketMap = new Dictionary<SocketId, MeshSocket>();

        public void Initialize()
        {
            RegisterSockets (_SocketRegistries);
        }

        /// <summary>
        /// Register a socket.
        /// </summary>
        /// <param name="socket"></param>
        public void RegisterSocket(MeshSocket socket)
        {
            socketMap[socket.SocketId] = socket;
        }

        /// <summary>
        /// Register sockets.
        /// </summary>
        /// <param name="sockets"></param>
        public void RegisterSockets(IEnumerable<MeshSocket> sockets)
        {
            foreach (var socket in sockets)
            {
                socketMap[socket.SocketId] = socket;
            }
        }

        /// <summary>
        /// Attach mesh to socket.
        /// </summary>
        /// <param name="objectTransform"></param>
        /// <param name="socketId"></param>
        public void Attach(Transform objectTransform, SocketId socketId)
        {
            socketMap[socketId].Attach(objectTransform);
        }
        
        public MeshSocket GetSocket(SocketId socketId)
        {
            return socketMap[socketId];
        }

        public enum SocketId
        {
            Spine,
            LeftHand,
            RightHand,
            Head,
            Eye,
            UpperBack,
            LowerBack
        }
    }
}
