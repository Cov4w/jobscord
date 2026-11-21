//최소 채팅 예제
using Microsoft.AspNetCore.SignalR; //Hub, Clients, Groups 등이 들어 있는 네임스페이스

namespace SignalR.Hubs;
// Hub를 상속하면 "클라이언트가 호출할 수 있는 서버 메서드"를 모아 둔 클래스가 된다.
// 주의: Hub 인스턴스는 클라이언트가 메서드를 호출할 때마다 새로 만들어진다(Transient).
//       그래서 필드에 값을 저장해도 다음 호출 때는 사라진다.
public class ChatHub: Hub
{
    public async Task SendMessage(string user, string message)
    {
        // 클라이언트가 connection.invoke("SendMessage", user, message)로 호출하는 메서드.
        // 빈 메시지는 서버에서도 한 번 더 걸러 준다. (클라이언트 검증만 믿지 않기)
        if (string.IsNullOrWhiteSpace(message)) return;

        // Clients.All      : 현제 연결된 모든 클라이언트
        // SendAsync(...)   : 클라이언트의 "ReceiveMessage" 핸들러를 호출한다.
        //                    첫 번째 인자는 클라이언트 쪽 핸들러 이름이고,
        //                    그 뒤 인자들이(user, message)이 핸들러의 매개변수로 전달된다.
        await Clients.All.SendAsync(
            "ReceiveMessage",
            user,
            message,
            Context.ConnectionId,                  // 보낸 사람 식별용
            DateTime.Now.ToString("HH:mm"));       // 표시용 시간
    }
}
