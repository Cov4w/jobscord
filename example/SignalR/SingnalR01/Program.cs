using SignalR.Hubs;
using SignalR.Services; 
using SignalR.Filters;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// SignalR 서비스를 DI 컨테이너에 등록한다. 이 줄이 없으면 MapHub에서 오류
builder.Services.AddSignalR(options =>
{
    options.AddFilter<LoggingFilter>();   // 모든 Hub에 적용
    options.AddFilter<BadWordFilter>();
});
builder.Services.AddHostedService<ClockWorker>();

var app = builder.Build();

app.UseDefaultFiles();   // "/" 요청시 wwwroot/index.html 자동 제공
app.UseStaticFiles();

// ChatHub를 "/chatHub" 주소에 연결
// 클라리언트는 이 URL로 접속
app.MapHub<ChatHub>("/chatHub");
app.MapHub<RoomHub>("/roomHub"); 
app.MapHub<StreamHub>("/streamHub");

// IHubContext<ChatHub>를 매개변수로 받으면 DI가 자동으로 넣어준다.
app.MapPost("/api/orders/{id}/complete", async (int id, IHubContext<ChatHub> hub) =>
{
    // 실제 서비스라면 여기서 DB 저장 등 비즈니스 로직을 먼저 수행
    await hub.Clients.All.SendAsync("OrderCompleted", id);
    return Results.Ok(new { id, status = "completed" });
});

app.Run();