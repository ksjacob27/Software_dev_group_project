// using System;
//
//
//
// namespace DatabaseNetwork {
//     
//     public class NetworkConnectionToClient : DatabaseManager {
//         public override string address =>
//             Transport.activeTransport.ServerGetClientAddress(_conxId);
//
//         // unbatcher
//         public Unbatcher unbatcher = new Unbatcher();
//
//         public NetworkConnectionToClient(int networkConnectionId)
//             : base(networkConnectionId) {}
//
//         // Send stage three: hand off to transport
//         protected override void SendToTransport(ArraySegment<byte> segment, int channelId = Channels.Reliable) =>
//             Transport.activeTransport.ServerSend(_conxId, segment, channelId);
//
//         /// <summary>Disconnects this connection.</summary>
//         public override void Disconnect() {
//             // set not ready and handle clientscene disconnect in any case
//             // (might be client or host mode here)
//             isReady = false;
//             Transport.activeTransport.ServerDisconnect(_conxId);
//
//             // IMPORTANT: NetworkConnection.Disconnect() is NOT called for
//             // voluntary disconnects from the other end.
//             // -> so all 'on disconnect' cleanup code needs to be in
//             //    OnTransportDisconnect, where it's called for both voluntary
//             //    and involuntary disconnects!
//         }
//     }
// }
