using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DynamicPixels.GameService.Models;
using DynamicPixels.GameService.Models.outputs;
using DynamicPixels.GameService.Utils.HttpClient;
using DynamicPixels.GameService.Utils.WebsocketClient;
using Newtonsoft.Json;

namespace DynamicPixels.GameService.Services.MultiPlayer.Room
{
    public class Room
    {
        private ISocketAgent _socketAgent;

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public int MinPlayer { get; set; }
        public int MaxPlayer { get; set; }
        public int? MinXp { get; set; }
        public int? MaxXp { get; set; }
        public bool IsPermanent { get; set; }
        public RoomStatus Status { get; set; }
        public bool IsTurnBasedGame { get; set; }
        public bool IsLocked { get; set; }
        public GameOrderType? GameOrderType { get; set; }
        public string Metadata { get; set; }
        public int CreatorId { get; set; }
        public List<RoomPlayer> Players { get; set; }


        public event EventHandler<RoomPlayer> OnAfterUserJoinedToRoom;
        public event EventHandler<Room> OnCreateRoom;
        public event EventHandler<int> OnLeaveRoom;
        public event EventHandler<int> OnDisposeRoom;
        public event EventHandler OnDisconnect;
        public event EventHandler<Request> OnMessageReceived;


        public void Config(ISocketAgent socketAgent)
        {
            _socketAgent = socketAgent;
            _socketAgent.OnMessageReceived += OnMessage;
            _socketAgent.OnDisconnect += Disconnect;
        }

        public async Task Open()
        {
            var sendingBody = new { State = nameof(RoomStatus.Open) };
            await WebRequest.Put(UrlMap.UpdateStatusUrl(Id), JsonConvert.SerializeObject(sendingBody));
            Status = RoomStatus.Open;
        }

        public async Task Lock()
        {
            var sendingBody = new { State = nameof(RoomStatus.Lock) };
            await WebRequest.Put(UrlMap.UpdateStatusUrl(Id), JsonConvert.SerializeObject(sendingBody));
            Status = RoomStatus.Lock;
        }

        public Task SendToUser(int receiverId, string message)
        {
            var payload = new SendToRoomInputDto
            {
                RoomId = Id,
                Message = message
            };

            var packet = new Request
            {
                Method = "room:send-to-user",
                ReceiverId = receiverId,
                Payload = JsonConvert.SerializeObject(payload)
            };

            return _socketAgent.Send(packet);
        }

        public Task Broadcast(string message)
        {
            var payload = new SendToRoomInputDto
            {
                RoomId = Id,
                Message = message
            };

            var packet = new Request()
            {
                Method = "room:broadcast",
                Payload = JsonConvert.SerializeObject(payload)
            };

            return _socketAgent.Send(packet);
        }

        public Task Leave()
        {
            return WebRequest.Delete(UrlMap.LeaveRoomUrl(Id));
        }

        private void UserJoinedListenerAction(string message)
        {
            var user = JsonConvert.DeserializeObject<RoomPlayer>(message);

            Players.Add(user);

            OnAfterUserJoinedToRoom?.Invoke(this, user);
        }

        private void UserLeftListenerAction(string message)
        {
            var userId = Convert.ToInt32(message);

            Players.RemoveAll(a => a.UserId == userId);

            OnLeaveRoom?.Invoke(this, userId);
        }

        private void OnMessage(object source, Request packet)
        {
            switch (packet.Method)
            {
                case MessageType.SendToUserMessage:
                case MessageType.BroadcastToRoomMessage:
                    OnMessageReceived?.Invoke(this, packet);
                    break;
                case MessageType.UserJoinedToRoomMessage:
                    UserJoinedListenerAction(packet.Payload);
                    break;
                case MessageType.UserLeftRoomMessage:
                    UserLeftListenerAction(packet.Payload);
                    break;
            }
        }

        private void Disconnect(object sender, EventArgs e)
        {
            if (OnDisconnect != null)
                OnDisconnect(sender, null);
        }
    }

    public struct RoomPlayer
    {
        public int UserId { get; set; }
        public string Metadata { get; set; }
        public List<string> Tags { get; set; }
    }

    public enum RoomStatus
    {
        Initial = 0,
        Open = 1,
        Lock = 2
    }

    public enum GameOrderType
    {
        Random = 1,
        RoundRobin = 2,
        UserDefined = 3
    }

    internal static class MessageType
    {
        public const string BroadcastToRoomMessage = "room:broadcast";
        public const string SendToUserMessage = "room:send-to-user";
        public const string RoomCreatedMessage = "room:created";
        public const string RoomDeletedMessage = "room:deleted";
        public const string UserJoinedToRoomMessage = "room:user-joined";
        public const string UserLeftRoomMessage = "room:leave";
    }
    internal class SendToRoomInputDto
    {
        public int RoomId { get; set; }
        public string Message { get; set; }
    }
}