using Microsoft.AspNetCore.SignalR;

namespace SignalR.Filters;

public class BadWordFilter : IHubFilter
{
    // 실습용 목록. 실제 서비스에서는 설정 파일이나 DB에서 읽는다.
    private static readonly string[] BadWords = { "욕설", "badword" };

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        // ChatHub.SendMessage(string user, string message)만 검사
        if (context.HubMethodName == "SendMessage"
            && context.HubMethodArguments.Count >= 2
            && context.HubMethodArguments[1] is string message
            && BadWords.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase)))
        {
            // HubException의 메시지는 운영 환경에서도 클라이언트에 그대로 전달된다.
            // (일반 Exception은 보안상 "오류 발생" 정도의 일반 문구로 가려진다.)
            throw new HubException("금칙어가 포함된 메시지는 보낼 수 없습니다.");
        }

        return await next(context);   // 통과하면 정상 실행
    }
}