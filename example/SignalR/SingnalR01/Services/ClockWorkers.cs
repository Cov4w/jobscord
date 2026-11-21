using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;                  // ChatHub의 네임스페이스 (ChatHub가 네임스페이스 없이 선언돼 있으면 이 줄 삭제)

namespace SignalR.Services;

// BackgroundService: 앱이 실행되는 동안 계속 도는 백그라운드 작업
public class ClockWorker : BackgroundService
{
    private readonly IHubContext<ChatHub> _hub;

    // Hub 클래스 자체가 아니라 IHubContext<ChatHub>를 주입받는다.
    public ClockWorker(IHubContext<ChatHub> hub) => _hub = hub;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // 앱 종료 신호(ct)가 올 때까지 반복
        while (!ct.IsCancellationRequested)
        {
            // chatHub에 연결된 모든 클라이언트의 "Tick" 핸들러 호출
            await _hub.Clients.All.SendAsync("Tick", DateTime.Now.ToString("HH:mm:ss"), ct);

            await Task.Delay(1000, ct);   // 1초 대기 (종료 신호가 오면 즉시 중단)
        }
    }
}