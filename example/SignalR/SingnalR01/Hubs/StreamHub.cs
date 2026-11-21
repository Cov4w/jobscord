using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs; 

public class StreamHub : Hub
{
    // ───── 서버 → 클라이언트 스트리밍 ─────
    // IAsyncEnumerable<T>를 반환하면 SignalR이 "스트리밍 메서드"로 인식한다.
    public async IAsyncEnumerable<int> Counter(
        int count,
        int delayMs,
        // 클라이언트가 dispose()하거나 연결이 끊기면 이 토큰이 취소된다.
        [EnumeratorCancellation] CancellationToken ct)
    {
        // 클라이언트가 보낸 값은 믿지 않는다. 범위를 제한해서 서버 과부하를 막는다.
        count   = Math.Clamp(count, 1, 1000);
        delayMs = Math.Clamp(delayMs, 10, 5000);

        for (var i = 1; i <= count; i++)
        {
            ct.ThrowIfCancellationRequested();   // 취소됐으면 즉시 중단
            yield return i;                      // 값 하나를 클라이언트로 전송
            await Task.Delay(delayMs, ct);       // 다음 값까지 대기
        }
        // 메서드가 끝나면 클라이언트의 complete 콜백이 호출된다.
    }

    // ───── 클라이언트 → 서버 스트리밍 ─────
    // 인자를 IAsyncEnumerable<T>로 받으면 클라이언트가 보내는 값을 순서대로 읽을 수 있다.
    public async Task Upload(IAsyncEnumerable<string> chunks)
    {
        var count = 0;

        // 클라이언트가 subject.next()를 할 때마다 한 번씩 반복되고,
        // subject.complete()를 하면 반복이 끝난다.
        await foreach (var chunk in chunks)
        {
            count++;
            Console.WriteLine($"[Upload] {count}번째 조각: {chunk}");
        }

        // 스트림이 끝난 뒤 호출자에게 결과 통보
        await Clients.Caller.SendAsync("UploadDone", count);
    }
}