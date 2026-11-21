using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Filters;

// IHubFilter: 허브 메서드 호출과 연결/종료 시점을 가로채는 인터페이스
public class LoggingFilter : IHubFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    // 필터도 DI로 생성되므로 생성자에서 ILogger 등을 주입받을 수 있다.
    public LoggingFilter(ILogger<LoggingFilter> logger) => _logger = logger;

    // ───── 1) 클라이언트가 Hub 메서드를 호출할 때마다 실행 ─────
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,                          // 호출 정보 (메서드명, 인자, 연결 등)
        Func<HubInvocationContext, ValueTask<object?>> next)   // 다음 단계 = 실제 Hub 메서드
    {
        var sw = Stopwatch.StartNew();   // 소요 시간 측정 시작

        // 인자 값(채팅 내용 등)은 개인정보일 수 있으므로 로그에는 개수만 남긴다.
        _logger.LogInformation("[호출] {Hub}.{Method} (인자 {ArgCount}개) conn={ConnId}",
            context.Hub.GetType().Name,
            context.HubMethodName,
            context.HubMethodArguments.Count,
            context.Context.ConnectionId);

        try
        {
            // 실제 Hub 메서드 실행. 이 줄을 호출하지 않으면 메서드가 실행되지 않는다.
            var result = await next(context);

            _logger.LogInformation("[완료] {Method} {Elapsed}ms",
                context.HubMethodName, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[오류] {Method} {Elapsed}ms",
                context.HubMethodName, sw.ElapsedMilliseconds);
            throw;   // 삼키지 말고 다시 던져서 SignalR이 클라이언트에 오류를 전달하게 한다.
        }
    }

    // ───── 2) 클라이언트가 연결될 때 실행 (선택 구현) ─────
    public async Task OnConnectedAsync(
        HubLifetimeContext context,
        Func<HubLifetimeContext, Task> next)
    {
        _logger.LogInformation("[연결] {Hub} conn={ConnId}",
            context.Hub.GetType().Name, context.Context.ConnectionId);
        await next(context);   // 실제 Hub의 OnConnectedAsync 실행
    }

    // ───── 3) 연결이 끊길 때 실행 (선택 구현) ─────
    public async Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        _logger.LogInformation("[종료] {Hub} conn={ConnId} 사유={Reason}",
            context.Hub.GetType().Name,
            context.Context.ConnectionId,
            exception?.Message ?? "정상 종료");   // exception이 null이면 정상 종료
        await next(context, exception);
    }
}