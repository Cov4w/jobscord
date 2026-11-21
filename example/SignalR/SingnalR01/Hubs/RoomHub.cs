using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;
public class RoomHub : Hub
{
    public async Task JoinRoom(string room)
    {
        // 현재 연결(Context.ConnectionId)을 room이라는 이름의 그룹에 넣는다.
        // 그룹은 서버 메모리에만 존재하고, 연결이 끊기면 자동으로 빠진다.
        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        // 해당 그룹에 속한 연결들에게만 입장 알림을 보낸다.
        await Clients.Group(room).SendAsync("System", $"{Context.ConnectionId} 입장");
    }

    public async Task LeaveRoom(string room)
    {
        // 그룹에서 제거. 제거하기 전에 알리면 나도 퇴장 메시지를 받고, 뒤에 알리면 받지 못한다.
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).SendAsync("System", $"{Context.ConnectionId} 퇴장");
    }

    // 특정 방에만 메시지를 전달
    public Task SendToRoom(string room, string message)
        => Clients.Group(room).SendAsync("RoomMessage", room, message);
}