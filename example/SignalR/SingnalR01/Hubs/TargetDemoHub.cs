using Microsoft.AspNetCore.SignalR;


namespace SignalR.Hubs;
public class TargetDemoHub : Hub
{
    public async Task Demo(string text, string otherConnectionId)
    {
        // 연결된 모든 클라이언트 (나 포함)
        await Clients.All.SendAsync("Msg", text);
        
        // 이 메서드를 호출한 클라이언트 본인에게만
        await Clients.Caller.SendAsync("Msg", text);

        // 호출자를 뺀 나머지 전체 (내가 보낸 채팅을 내 화면에 중복 표시하고 싶지 않을 때)
        await Clients.Others.SendAsync("Msg", text);
        
        // 특정 ConnectionId 하나에게만 (1:1 메시지)
        await Clients.Client(otherConnectionId).SendAsync("Msg", text);

        // 지정한 연결만 제외하고 전체에게
        await Clients.AllExcept(Context.ConnectionId).SendAsync("Msg", text);
   }
}

