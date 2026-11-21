# SignalR 입문 수업 자료

**ASP.NET Core SignalR로 배우는 실시간 웹 통신 — 1단계부터 7단계까지**

| 항목 | 내용 |
|---|---|
| 대상 | 실시간 통신을 처음 배우는 학생 (C#, HTML/JavaScript 기초 수준) |
| 사용 기술 | .NET (ASP.NET Core), SignalR, JavaScript 클라이언트 |
| 실습 환경 | macOS / Windows 공통 (VS Code 또는 Visual Studio) |
| 구성 | 0장 사전 개념 → 1~5단계 실습 코드 해설 → 6~7단계 확장 개념 → 부록 |

---

## 목차

| 구분 | 내용 |
|---|---|
| 수업 안내 | 학습 목표, 시간표, 프로젝트 구조, 진행 방식 |
| 0장 | 사전 개념: 실시간 통신, SignalR, 핵심 용어 |
| 1단계 | 기본 채팅: `ChatHub`, `index.html` |
| 2단계 | 그룹과 재연결: `RoomHub`, `rooms.js` |
| 3단계 | 서버 주도 푸시: `ClockWorker`, `IHubContext` |
| 4단계 | 스트리밍: `StreamHub`, `stream.js` |
| 5단계 | Hub 필터: `LoggingFilter`, `BadWordFilter` |
| 6단계 | 인증: `NotifyHub`, JWT (개요) |
| 7단계 | 스케일 아웃: Redis backplane (개요) |
| 부록 A~G | 통합 코드, 치트시트, 오류 해석표, 확인 문제/정답, 과제, 협업 주의, 한 장 요약 |

---

## 수업 안내

### 학습 목표

이 수업을 마치면 학생은 다음을 할 수 있습니다.

1. HTTP 요청/응답 방식과 실시간 통신 방식의 차이를 설명한다.
2. SignalR의 핵심 용어(Hub, Connection, Group, Client 등)를 설명한다.
3. Hub를 만들고 브라우저와 서버가 서로 메서드를 호출하게 구현한다.
4. 그룹, 서버 주도 푸시, 스트리밍, Hub 필터를 각각 구현한다.
5. 인증과 서버 여러 대 운영(스케일 아웃)이 왜 필요한지 설명한다.

### 단계별 구성과 예시 시간표

| 단계 | 주제 | 만드는 파일 | 핵심 개념 | 예시 시간 |
|---|---|---|---|---|
| 0장 | 사전 개념 | (없음) | Polling, WebSocket, SignalR | 30분 |
| 1단계 | 기본 채팅 | `Hubs/ChatHub.cs`, `index.html` | Hub, Clients.All, invoke/on | 50분 |
| 2단계 | 그룹과 재연결 | `Hubs/RoomHub.cs`, `js/rooms.js` | Groups, ConnectionId 변경 | 40분 |
| 3단계 | 서버 주도 푸시 | `Services/ClockWorker.cs` | IHubContext, BackgroundService | 30분 |
| 4단계 | 스트리밍 | `Hubs/StreamHub.cs`, `js/stream.js` | IAsyncEnumerable, Subject | 40분 |
| 5단계 | Hub 필터 | `Filters/LoggingFilter.cs` | IHubFilter, next, HubException | 30분 |
| 6단계 | 인증 | `Hubs/NotifyHub.cs` | JWT, [Authorize], UserIdentifier | 50분 |
| 7단계 | 스케일 아웃 | (설정 위주) | Backplane, Redis, Sticky Session | 30분 |

> 시간은 예시이며 학생 수준에 따라 조정합니다. 1단계에 시간을 넉넉히 쓰면 이후 단계가 빨라집니다.

### 이 수업에서 만드는 프로젝트 구조

```
SignalR/                              ← 솔루션 폴더 (.gitignore, .sln)
├── SignalR.sln
├── SingnalR01/                       ← 서버 프로젝트 (ASP.NET Core)
│   ├── Program.cs                    ← 서비스 등록 + Hub 주소 연결
│   ├── Hubs/
│   │   ├── ChatHub.cs                ← 1단계
│   │   ├── TargetDemoHub.cs          ← 대상 지정 비교 예제
│   │   ├── RoomHub.cs                ← 2단계
│   │   └── StreamHub.cs              ← 4단계
│   ├── Services/
│   │   └── ClockWorker.cs            ← 3단계
│   ├── Filters/
│   │   └── LoggingFilter.cs          ← 5단계
│   └── wwwroot/                      ← 브라우저가 받는 정적 파일
│       ├── index.html                ← 1단계 채팅 화면
│       ├── rooms.html                ← 2단계 테스트 페이지
│       ├── stream.html               ← 4단계 테스트 페이지
│       └── js/
│           ├── rooms.js
│           └── stream.js
└── SingnalR01.ConsoleClient/         ← (선택) .NET 콘솔 클라이언트
```

### 실행 방법 (공통)

```bash
cd SingnalR01        # 서버 프로젝트 폴더
dotnet run           # 출력된 주소(예: http://localhost:5241)로 브라우저 접속
dotnet watch run     # 코드 저장 시 자동 재시작 (수업 중 편리)
```

포트 번호는 컴퓨터마다 다를 수 있습니다. 터미널에 출력된 `Now listening on:` 주소를 사용합니다.

### 수업 진행 방식 제안

각 단계는 같은 순서로 진행하면 학생이 흐름을 익히기 쉽습니다.

1. **왜 필요한가** (문제 상황 제시)
2. **개념 설명** (그림과 용어)
3. **코드 읽기** (파일 하나씩, 메서드 표 참고)
4. **직접 실행** (탭 2개로 동작 확인)
5. **일부러 틀려 보기** (오류 메시지를 읽는 연습)
6. **확인 문제** (부록의 문제 활용)

---

# 0장. 사전 개념: 실시간 통신이란?

## 0.1 HTTP의 한계

HTTP는 **클라이언트가 요청해야 서버가 응답**하는 구조입니다. 서버가 먼저 말을 걸 수 없습니다.

```
브라우저                       서버
   │ ── "새 메시지 있어?" ──▶  │
   │ ◀── "없어" ────────────  │
   │ ── "새 메시지 있어?" ──▶  │
   │ ◀── "없어" ────────────  │
   │ ── "새 메시지 있어?" ──▶  │
   │ ◀── "있어! (내용)" ─────  │
```

채팅, 알림, 실시간 시세처럼 **서버에서 먼저 데이터가 생기는 서비스**에는 맞지 않습니다.

## 0.2 실시간 통신 방식 비교

| 방식 | 동작 | 장점 | 단점 |
|---|---|---|---|
| **Polling** | 클라이언트가 주기적으로 "새 거 있어?" 요청 | 구현이 쉬움 | 낭비가 크고 지연이 있음 |
| **Long Polling** | 서버가 새 데이터가 생길 때까지 응답을 미루고 있다가 응답 | Polling보다 효율적 | 응답마다 연결을 다시 맺음 |
| **SSE** (Server-Sent Events) | 서버 → 클라이언트 단방향 스트림 | 단순함 | 단방향만 가능 |
| **WebSocket** | 연결 하나로 양방향 통신 | 가장 빠르고 효율적 | 직접 쓰면 구현할 것이 많음 |

WebSocket을 직접 쓰면 연결 관리, 재연결, 그룹, 직렬화, 인증, 서버 확장까지 **모두 직접 만들어야** 합니다.

## 0.3 SignalR이란

> **SignalR = 실시간 양방향 통신을 쉽게 만들어 주는 ASP.NET Core의 라이브러리**

SignalR이 해 주는 일은 다음과 같습니다.

| 기능 | 설명 |
|---|---|
| 전송 방식 자동 선택 | WebSocket → SSE → Long Polling 순으로 가능한 방식을 고름 |
| 연결 관리 | 연결마다 고유 ID 부여, 연결/종료 이벤트 제공 |
| 메서드 호출 모델(RPC) | 서버와 클라이언트가 서로의 메서드를 호출하는 방식으로 코딩 |
| 그룹 / 사용자 단위 전송 | "이 방 사람들에게만", "이 사용자에게만" 보내기 |
| 자동 재연결 | 끊기면 자동으로 다시 연결 시도 |
| 확장 | Redis, Azure SignalR Service로 서버 여러 대 지원 |

## 0.4 SignalR의 호출 모델 (가장 중요한 그림)

SignalR에서는 **양쪽이 서로의 메서드를 호출**합니다.

```
          [ 브라우저 (JavaScript) ]                 [ 서버 (C#) ]

  ① 클라이언트 → 서버 호출
     connection.invoke("SendMessage", ...)  ───▶  Hub의 SendMessage() 실행

  ② 서버 → 클라이언트 호출
     connection.on("ReceiveMessage", ...)   ◀───  Clients.All.SendAsync("ReceiveMessage", ...)
```

- **①** 브라우저가 서버 메서드를 부릅니다. 서버 쪽에는 **같은 이름의 C# 메서드**가 있어야 합니다.
- **②** 서버가 브라우저 함수를 부릅니다. 브라우저 쪽에는 `on(이름, 함수)`로 **같은 이름의 핸들러**를 등록해 두어야 합니다.

> 이 수업의 모든 버그 중 절반은 **이름 불일치**입니다. 문자열로 연결되므로 오타가 있어도 컴파일 오류가 나지 않습니다.

## 0.5 연결이 만들어지는 과정

```
브라우저                                    서버
   │ ① POST /chatHub/negotiate ──────────▶  │  연결 ID, 사용 가능한 전송 방식 협상
   │ ◀──────────────────────────────────── │
   │ ② WebSocket 연결 요청 (Upgrade) ────▶  │  가장 좋은 방식으로 연결
   │ ③ 핸드셰이크 (프로토콜 합의) ────────▶  │  JSON 형식 사용 합의
   │ ◀══════ 이후 메시지 교환 ═══════════▶  │
```

브라우저 개발자 도구(F12) → Network 탭에서 `negotiate` 요청과 WebSocket 연결을 직접 볼 수 있습니다.

## 0.6 핵심 용어 사전

| 용어 | 뜻 | 비유 |
|---|---|---|
| **Hub** | 클라이언트와 서버가 서로 메서드를 호출하는 중심 클래스 | 전화 교환실 |
| **Connection** | 클라이언트 하나가 서버와 맺은 연결 | 통화 한 건 |
| **ConnectionId** | 연결마다 부여되는 임시 고유 ID. **재연결하면 바뀜** | 통화 번호표 |
| **Clients** | 서버가 메시지를 보낼 대상을 고르는 객체 | 수신자 선택 |
| **Group** | 연결들을 이름으로 묶은 집합 | 채팅방 |
| **User** | 로그인한 사용자. 한 사용자가 여러 연결을 가질 수 있음 | 회원 한 명이 폰/PC로 동시 접속 |
| **IHubContext** | Hub 바깥에서 클라이언트에 메시지를 보내는 도구 | 방송실 마이크 |
| **Transport** | 실제 데이터가 오가는 방식 (WebSocket 등) | 통신 수단 |
| **Backplane** | 서버 여러 대가 메시지를 공유하는 통로 | 지점 간 전화 연결망 |

## 0.7 이 수업에서 지킬 약속 5가지

1. 서버 → 클라이언트 핸들러(`on`)는 **`start()`보다 먼저** 등록한다.
2. Hub 클래스에 **상태(필드)를 저장하지 않는다.** 호출마다 새로 만들어지기 때문이다.
3. 화면에 사용자 입력을 넣을 때는 **`textContent`**를 쓴다. (`innerHTML`은 XSS 위험)
4. **재연결하면 ConnectionId가 바뀐다.** 그룹 소속은 사라진다.
5. 메시지는 **전달이 보장되지 않는다.** 중요한 데이터는 DB에 저장한다.

---

# 1단계. 기본 채팅: Hub와 메시지 주고받기

**만드는 파일:** `Hubs/ChatHub.cs`, `Program.cs`, `wwwroot/index.html`

## 1.1 학습 목표

- Hub 클래스를 만들고 `MapHub`으로 주소에 연결한다.
- 브라우저에서 서버 메서드를 호출(`invoke`)하고, 서버가 브라우저 함수를 호출(`SendAsync`)하는 흐름을 이해한다.
- "내 메시지"와 "상대 메시지"를 구분해서 화면에 그린다.

## 1.2 전체 흐름

```
[브라우저 A]                         [서버]                          [브라우저 A, B]

 전송 버튼 클릭
 connection.invoke("SendMessage",
                   user, msg)  ───▶  ChatHub.SendMessage(user, msg)
                                       │
                                       ▼
                                     Clients.All.SendAsync("ReceiveMessage", ...)
                                       │
                                       └────────────────────────▶  on("ReceiveMessage") 실행
                                                                     → 말풍선 추가
```

A가 보낸 메시지는 서버를 거쳐 **A 자신에게도** 돌아옵니다(`Clients.All`이므로). 그래서 화면에서 "내 것인지"를 판별하는 코드가 필요합니다.

## 1.3 서버 코드

### Hubs/ChatHub.cs

```csharp
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class ChatHub : Hub
{
    // 클라이언트가 connection.invoke("SendMessage", user, message)로 호출하는 메서드
    public async Task SendMessage(string user, string message)
    {
        // 빈 메시지는 서버에서도 걸러 준다. (클라이언트 검증만 믿지 않는다)
        if (string.IsNullOrWhiteSpace(message)) return;

        await Clients.All.SendAsync(
            "ReceiveMessage",                      // 클라이언트의 on("ReceiveMessage")와 이름이 같아야 함
            user,
            message,
            Context.ConnectionId,                  // 보낸 사람 식별용
            DateTime.Now.ToString("HH:mm"));       // 표시용 시간
    }
}
```

### Program.cs

```csharp
using SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();            // ① SignalR 서비스를 DI 컨테이너에 등록

var app = builder.Build();

app.UseDefaultFiles();                    // ② "/" 요청 시 wwwroot/index.html 제공
app.UseStaticFiles();                     //    wwwroot의 html, js, css 제공

app.MapHub<ChatHub>("/chatHub");          // ③ ChatHub를 "/chatHub" 주소에 연결

app.Run();
```

### 서버 메서드/속성 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `Hub` | 기본 클래스 | 상속하면 public 메서드가 "클라이언트가 호출할 수 있는 메서드"가 됨 |
| `AddSignalR()` | 서비스 등록 | 없으면 `MapHub`에서 오류. **앱 전체에서 한 번만** 호출 |
| `MapHub<T>(주소)` | 주소 연결 | 클라이언트가 접속할 URL을 정함. Hub마다 하나씩 |
| `Clients` | 수신 대상 선택 | `All`, `Caller`, `Others`, `Group(...)` 등 (2단계 표 참고) |
| `Clients.All` | 전체 대상 | 현재 연결된 모든 클라이언트 (나 포함) |
| `SendAsync(이름, 인자...)` | 클라이언트 함수 호출 | 첫 인자는 클라이언트 핸들러 이름, 나머지는 핸들러의 매개변수 |
| `Context` | 현재 연결 정보 | `ConnectionId`, `User`, `Items`, `GetHttpContext()` 등 |
| `Context.ConnectionId` | 연결 ID | 연결마다 다른 임시 값. 재연결하면 바뀜 |
| `UseDefaultFiles()` | 기본 문서 | `/` 요청을 `index.html`로 연결 |
| `UseStaticFiles()` | 정적 파일 | `wwwroot` 폴더의 파일을 그대로 제공 |

> **주의:** Hub 인스턴스는 **메서드가 호출될 때마다 새로 만들어집니다.** `private int count;` 같은 필드에 값을 저장해도 다음 호출에서 사라집니다.

## 1.4 클라이언트 코드 (index.html의 핵심 스크립트)

```html
<!-- SignalR JavaScript 클라이언트 라이브러리 (먼저 로드) -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js"></script>
<script>
  // ① 연결 객체 생성 (아직 접속하지 않은 상태)
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")            // 서버의 MapHub 주소와 동일해야 함
    .withAutomaticReconnect()       // 끊기면 0초, 2초, 10초, 30초 간격으로 재시도
    .build();

  // ② 서버가 호출할 핸들러 등록 (반드시 start() 전에)
  connection.on("ReceiveMessage", (user, message, senderId, time) => {
    addMessage({
      user, message, time,
      isMe: senderId === connection.connectionId   // 내 메시지인지 판별
    });
  });

  // ③ 연결 시작
  connection.start()
    .then(() => setConnected(true, "연결됨"))
    .catch(err => setConnected(false, "연결 실패"));

  // ④ 서버 메서드 호출
  async function send() {
    const user = userEl.value.trim() || "익명";
    const message = msgEl.value.trim();
    if (!message) return;
    await connection.invoke("SendMessage", user, message);
  }
</script>
```

### 클라이언트 메서드 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `HubConnectionBuilder` | 연결 생성기 | 빌더 패턴으로 옵션을 이어 붙이고 `build()`로 완성 |
| `withUrl(주소)` | 서버 주소 지정 | `MapHub`의 주소와 같아야 함 |
| `withAutomaticReconnect()` | 자동 재연결 | 기본 간격 0, 2, 10, 30초. 모두 실패하면 `onclose` |
| `build()` | 연결 객체 생성 | 이 시점에는 아직 서버에 접속하지 않음 |
| `on(이름, 함수)` | 핸들러 등록 | 서버가 `SendAsync(이름, ...)`를 호출하면 실행됨 |
| `start()` | 연결 시작 | Promise 반환. negotiate → WebSocket → 핸드셰이크 |
| `invoke(이름, 인자...)` | 서버 메서드 호출 | 서버 메서드가 끝날 때까지 기다리는 Promise. 서버 오류도 여기서 받음 |
| `send(이름, 인자...)` | 서버 메서드 호출 | 결과를 기다리지 않음 (fire-and-forget) |
| `connection.connectionId` | 내 연결 ID | 연결 후에 값이 생김 |
| `onreconnecting` / `onreconnected` / `onclose` | 상태 이벤트 | 재연결 시작 / 성공 / 완전 종료 시 실행 |

### invoke와 send 비교

| | `invoke` | `send` |
|---|---|---|
| 반환 | Promise (완료까지 대기) | 전송만 하고 끝 |
| 서버 반환값 | 받을 수 있음 | 받을 수 없음 |
| 서버 오류 | `catch`로 받을 수 있음 | 받을 수 없음 |
| 사용 시점 | 성공 여부가 중요할 때 (채팅 전송 등) | 로그, 상태 알림처럼 결과가 필요 없을 때 |

## 1.5 코드 읽기 포인트

**① 왜 `senderId === connection.connectionId`로 비교할까?**
이름(`user`)으로 비교하면 같은 이름을 쓰는 사람의 메시지가 모두 "내 것"이 됩니다. ConnectionId는 연결마다 다르므로 정확합니다.

**② 왜 서버에서도 빈 메시지를 거를까?**
브라우저 검증은 개발자 도구로 우회할 수 있습니다. **서버는 클라이언트를 믿지 않습니다.**

**③ 왜 `textContent`를 쓸까?**
`innerHTML`에 `<script>`가 담긴 메시지를 넣으면 다른 사람 화면에서 코드가 실행됩니다(XSS). `textContent`는 항상 글자로만 취급합니다.

**④ 한글 입력 시 Enter가 두 번 전송되는 문제**
한글은 글자 조합 중에 Enter를 누르면 마지막 글자가 한 번 더 전송될 수 있습니다. 그래서 `if (e.key === "Enter" && !e.isComposing) send();`로 조합 중 Enter를 무시합니다.

## 1.6 실습

1. `dotnet run`으로 서버를 실행한다.
2. 브라우저 **탭 2개**에서 같은 주소를 연다.
3. 한쪽에서 전송해 양쪽 화면에 나타나는지 확인한다. (내 것은 오른쪽 파랑, 상대 것은 왼쪽 흰색)
4. F12 → Network → WS(WebSocket) 탭에서 오가는 메시지를 관찰한다.

### 일부러 틀려 보기 (오류 읽기 연습)

| 변경 | 예상 결과 | 배우는 점 |
|---|---|---|
| 서버 `MapHub` 주소를 `/chat`으로 변경 | 콘솔에 404 / negotiate 실패 | 클라이언트와 서버 주소가 같아야 함 |
| 클라이언트 `on("ReceiveMsg", ...)`로 오타 | 오류 없이 메시지가 안 보임 | 이름 불일치는 **조용히 실패** |
| `invoke("SendMsg", ...)`로 오타 | `Method does not exist` 오류 | 서버 메서드 이름 불일치는 오류로 알려 줌 |
| `on()`을 `start()` 뒤로 이동 | 연결 직후 메시지를 놓칠 수 있음 | 핸들러는 start 전에 등록 |

## 1.7 자주 만나는 문제

| 증상 | 원인 | 해결 |
|---|---|---|
| 한글이 깨져 보임 | HTML에 문자 인코딩 선언 없음 | `<head>`에 `<meta charset="UTF-8" />` 추가, 파일을 UTF-8로 저장 |
| `signalR is not defined` | 라이브러리 로드 전에 스크립트 실행 | `<script src="...signalr.min.js">`를 먼저 배치 |
| 코드를 고쳤는데 그대로 | 브라우저 캐시 | `Cmd+Shift+R` (Windows: `Ctrl+F5`) |
| C# 수정이 반영 안 됨 | 서버 재시작 안 함 | `dotnet watch run` 사용 |

## 1.8 생각해 보기

1. `Clients.All` 대신 `Clients.Others`를 쓰면 화면은 어떻게 달라질까?
2. 서버 코드에서 `SendAsync`의 인자를 하나 더 늘리면 클라이언트는 무엇을 바꿔야 할까?
3. Hub에 `private int messageCount;`를 두고 `messageCount++` 하면 왜 원하는 대로 동작하지 않을까?

---

# 2단계. 그룹과 재연결: 채팅방 만들기

**만드는 파일:** `Hubs/RoomHub.cs`, `wwwroot/js/rooms.js`, `wwwroot/rooms.html`

## 2.1 학습 목표

- 연결들을 그룹으로 묶어 **일부 사람에게만** 메시지를 보낸다.
- 재연결하면 **ConnectionId가 바뀌어 그룹 소속이 사라지는 이유**를 이해하고 복구 코드를 작성한다.
- `Clients`의 여러 대상 지정 방식을 구분한다.

## 2.2 메시지 대상 지정 정리 (TargetDemoHub 연결)

프로젝트의 `TargetDemoHub.cs`는 아래 대상 지정을 한 곳에서 비교해 보는 예제입니다.

| 대상 | 의미 | 사용 예 |
|---|---|---|
| `Clients.All` | 연결된 모든 클라이언트 (나 포함) | 전체 공지 |
| `Clients.Caller` | 이 메서드를 호출한 본인 | "전송 완료" 확인 |
| `Clients.Others` | 나를 제외한 전체 | 내 화면은 직접 그리고 남에게만 전송 |
| `Clients.Client(connectionId)` | 특정 연결 하나 | 1:1 메시지 |
| `Clients.AllExcept(id...)` | 지정한 연결만 제외한 전체 | 특정인 제외 알림 |
| `Clients.Group(이름)` | 그룹 구성원 | **채팅방** |
| `Clients.GroupExcept(이름, id...)` | 그룹에서 일부 제외 | 방 안에서 나 제외 |
| `Clients.User(userId)` | 특정 사용자의 모든 연결 | 로그인 사용자에게 알림 (6단계) |

## 2.3 그룹의 개념

```
          서버 메모리의 그룹 목록

   "room1" ── [연결 A, 연결 B]
   "room2" ── [연결 C]

   Clients.Group("room1").SendAsync(...)  →  A, B만 받음. C는 못 받음
```

- 그룹은 **서버 메모리에만** 존재합니다. DB에 저장되지 않습니다.
- 연결이 끊기면 그 연결은 모든 그룹에서 **자동으로 빠집니다.**
- 그룹은 `AddToGroupAsync`를 처음 호출할 때 만들어지고, 구성원이 없으면 사라집니다. 별도로 "방 만들기" API가 없습니다.

## 2.4 서버 코드: Hubs/RoomHub.cs

```csharp
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;               // Program.cs의 using SignalR.Hubs; 와 같아야 함

public class RoomHub : Hub
{
    public async Task JoinRoom(string room)
    {
        // 현재 연결을 room 그룹에 넣는다
        await Groups.AddToGroupAsync(Context.ConnectionId, room);

        // 해당 그룹에만 입장 알림
        await Clients.Group(room).SendAsync("System", $"{Context.ConnectionId} 입장");
    }

    public async Task LeaveRoom(string room)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        await Clients.Group(room).SendAsync("System", $"{Context.ConnectionId} 퇴장");
    }

    public Task SendToRoom(string room, string message)
        => Clients.Group(room).SendAsync("RoomMessage", room, message);
}
```

Program.cs에는 `MapHub` 한 줄만 추가합니다. `AddSignalR()`은 다시 쓰지 않습니다.

```csharp
app.MapHub<RoomHub>("/roomHub");
```

### 서버 메서드 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `Groups.AddToGroupAsync(connectionId, group)` | 그룹 가입 | 연결을 그룹에 추가 |
| `Groups.RemoveFromGroupAsync(connectionId, group)` | 그룹 탈퇴 | 연결을 그룹에서 제거. 퇴장 알림을 **먼저** 보내면 나도 받고, **나중에** 보내면 나는 못 받음 |
| `Clients.Group(group)` | 그룹 대상 | 그룹 구성원에게만 전송 |

## 2.5 클라이언트 코드: wwwroot/js/rooms.js

```javascript
// 이 페이지 전용 연결. 이름을 roomConnection으로 구분한다.
const roomConnection = new signalR.HubConnectionBuilder()
  .withUrl("/roomHub")
  .withAutomaticReconnect()
  .build();

// 내가 가입한 방 목록. 재연결하면 서버가 그룹 소속을 잊으므로 클라이언트가 기억한다.
const joinedRooms = new Set();

// 서버 → 클라이언트 핸들러 (start 전에 등록)
roomConnection.on("System", (text) => console.log("[시스템]", text));
roomConnection.on("RoomMessage", (room, message) => console.log(`[${room}]`, message));

roomConnection.onreconnecting(() => console.log("재연결 중..."));

roomConnection.onreconnected(async () => {
  console.log("재연결 성공 → 방 재가입");
  for (const room of joinedRooms) {
    await roomConnection.invoke("JoinRoom", room);   // 새 ConnectionId로 다시 가입
  }
  // 끊긴 동안 놓친 메시지는 SignalR이 보관하지 않는다 → 필요하면 REST로 다시 조회(catch-up)
});

roomConnection.onclose(() => console.log("연결 종료"));

async function joinRoom(room) {
  await roomConnection.invoke("JoinRoom", room);
  joinedRooms.add(room);
}
async function leaveRoom(room) {
  await roomConnection.invoke("LeaveRoom", room);
  joinedRooms.delete(room);
}
async function sendToRoom(room, message) {
  await roomConnection.invoke("SendToRoom", room, message);
}

roomConnection.start()
  .then(() => console.log("roomHub 연결됨"))
  .catch(console.error);
```

### rooms.html (테스트 페이지)

```html
<!DOCTYPE html>
<html lang="ko">
<head><meta charset="UTF-8" /><title>Room 테스트</title></head>
<body>
  <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.7/signalr.min.js"></script>
  <script src="/js/rooms.js"></script>   <!-- signalR가 먼저 로드된 뒤여야 함 -->
</body>
</html>
```

브라우저 콘솔(F12)에서 직접 호출해 확인합니다.

```javascript
await joinRoom("room1");
await sendToRoom("room1", "안녕하세요");     // [room1] 안녕하세요 출력
```

## 2.6 재연결과 ConnectionId (핵심 개념)

```
[연결 중]      ConnectionId = AAA   그룹: room1 소속
     │
     │  네트워크 끊김 / 서버 재시작
     ▼
[재연결 중]    (메시지 수신 불가)
     │
     ▼
[재연결 성공]  ConnectionId = BBB   ← 새 ID! 그룹 소속 없음
     │
     │  onreconnected에서 JoinRoom 다시 호출
     ▼
[복구 완료]    ConnectionId = BBB   그룹: room1 소속
```

재연결 후 서버 입장에서는 **완전히 새로운 연결**입니다. 그래서 클라이언트가 가입한 방 목록(`joinedRooms`)을 기억했다가 다시 가입시킵니다.

> 참고: .NET 8부터 **Stateful Reconnect** 기능으로 ConnectionId를 유지하고 끊긴 동안의 메시지를 버퍼링할 수 있지만, 기본 동작은 위 그림과 같습니다.

## 2.7 실습

1. `http://localhost:포트/rooms.html`을 탭 2개로 연다.
2. 두 탭의 콘솔에서 `await joinRoom("room1")`을 실행한다.
3. 한쪽에서 `await sendToRoom("room1", "hi")`를 실행하면 두 탭 모두에 출력된다.
4. 한쪽 탭만 `await joinRoom("room2")`를 실행하고 `sendToRoom("room2", "비밀")`을 보내면 다른 탭에는 **오지 않는다.**
5. 서버를 `Ctrl+C`로 끄고 빠르게 다시 `dotnet run` → 콘솔에 "재연결 성공 → 방 재가입"이 찍히는지 확인한다.

> 서버를 완전히 껐다 켜면 기본 재시도(0, 2, 10, 30초)가 모두 끝나 `onclose`로 끝날 수 있습니다. 서버를 빨리 다시 켜거나 `.withAutomaticReconnect([0, 2000, 5000, 10000, 20000])`처럼 간격을 늘려 테스트합니다.

## 2.8 자주 만나는 문제 (실제 수업에서 나온 사례)

| 증상 | 원인 | 해결 |
|---|---|---|
| `'RoomHub' 형식 또는 네임스페이스 이름을 찾을 수 없습니다 (CS0246)` | 파일 확장자가 `.cs`가 아님, 또는 `namespace`와 `using` 불일치 | 파일명을 `RoomHub.cs`로 확인, `namespace SignalR.Hubs;`와 `using SignalR.Hubs;` 비교 |
| `Uncaught ReferenceError: connection is not defined` | 선언은 `roomConnection`으로 바꿨는데 아래 코드에 `connection.`이 남음 | 모든 `connection.`을 `roomConnection.`으로 통일 |
| negotiate 404 | `MapHub<RoomHub>("/roomHub")` 누락 또는 주소 오타 | Program.cs와 `withUrl` 주소 비교 |
| 재연결 후 방 메시지가 끊김 | `onreconnected`에서 재가입 누락 | `joinedRooms` 반복하며 `JoinRoom` 재호출 |
| 같은 페이지에서 `Identifier 'connection' has already been declared` | 한 페이지에 `const connection`을 두 번 선언 | Hub마다 `chatConnection`, `roomConnection`처럼 이름 구분 |

## 2.9 생각해 보기

1. 그룹 이름이 같은 두 사용자가 서로 다른 Hub(`ChatHub`, `RoomHub`)에 가입했다면 같은 그룹일까?
2. 재연결 후 `joinedRooms`를 복구하지 않으면 사용자에게 어떤 증상이 나타날까?
3. 사용자가 "room1"에 마음대로 들어가도 되는가? 실제 서비스라면 `JoinRoom`에서 무엇을 검사해야 할까?

---

# 3단계. 서버 주도 푸시: Hub 바깥에서 메시지 보내기

**만드는 파일:** `Services/ClockWorker.cs` (+ `Program.cs`, `index.html` 수정)

## 3.1 학습 목표

- 클라이언트가 요청하지 않아도 **서버가 먼저** 메시지를 보내는 방법을 이해한다.
- Hub 바깥(백그라운드 서비스, REST API)에서 `IHubContext`로 클라이언트에 보낸다.
- `AddHostedService`로 백그라운드 작업을 등록한다.

## 3.2 문제 상황

1~2단계에서는 **항상 클라이언트가 먼저 Hub 메서드를 호출**했습니다. 하지만 실제 서비스에서는 이런 일이 많습니다.

- 서버가 1초마다 시세/시간을 밀어 준다. (백그라운드 작업)
- 주문 처리가 끝나면 REST API 안에서 화면에 알린다. (컨트롤러/API)
- 메시지 큐에서 이벤트를 받으면 사용자에게 알린다. (외부 이벤트)

이 코드들은 **Hub 메서드 안이 아닙니다.** 그래서 Hub 바깥에서 쓸 도구인 `IHubContext`가 필요합니다.

```
 [Hub 안]                                  [Hub 바깥]
 클라이언트가 invoke 호출                   BackgroundService / REST API / 기타 서비스
   → Hub 메서드 실행                          → IHubContext<ChatHub>를 주입받아 사용
   → Clients.Caller, Others 사용 가능         → Clients.All, Group, User, Client(id)만 사용 가능
                                              ("호출자"가 없으므로 Caller/Others 없음)
```

## 3.3 서버 코드: Services/ClockWorker.cs

```csharp
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;

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
```

### Program.cs 수정

```csharp
using SignalR.Hubs;
using SignalR.Services;                              // ← 추가

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<ClockWorker>();    // ← 추가 (이 줄이 없으면 실행되지 않음)

var app = builder.Build();
// ... 이하 동일
```

`MapHub`은 추가하지 않습니다. ClockWorker는 **이미 `/chatHub`에 연결된 클라이언트에게** 보내므로 새 주소가 필요 없습니다.

### 서버 메서드/클래스 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `IHubContext<THub>` | Hub 바깥에서 쓰는 전송 도구 | DI(생성자 주입)로 받는다. Hub 인스턴스를 직접 만들지 않는다 |
| `_hub.Clients.All.SendAsync(...)` | 전체 전송 | Hub 안과 같은 방식으로 호출 |
| `BackgroundService` | 백그라운드 작업 기본 클래스 | `ExecuteAsync`를 구현하면 앱 시작 시 자동 실행 |
| `ExecuteAsync(CancellationToken)` | 작업 본문 | 앱 종료 시 토큰이 취소되어 루프를 빠져나옴 |
| `AddHostedService<T>()` | 백그라운드 서비스 등록 | **빠뜨리면 오류 없이 조용히 실행되지 않음** |
| `Task.Delay(ms, ct)` | 비동기 대기 | `Thread.Sleep`과 달리 스레드를 막지 않음. 취소 가능 |
| `CancellationToken` | 취소 신호 | 앱 종료, 클라이언트 연결 종료 등을 알려 줌 |

## 3.4 클라이언트 코드: index.html 수정

**① 헤더에 시계 영역 추가** (기존 `id="status"`는 반드시 유지)

```html
<div class="header">
  <strong>SignalR 채팅</strong>
  <span class="status">
    서버 시간 <span id="clock">--:--:--</span> · <span id="status">연결 중...</span>
  </span>
</div>
```

**② 핸들러 추가** (`on("ReceiveMessage", ...)` 아래, `start()` 위)

```javascript
// 서버의 ClockWorker가 1초마다 호출
connection.on("Tick", (time) => {
  document.getElementById("clock").textContent = time;   // 채팅 목록이 아니라 한 곳을 덮어쓴다
});

// REST API가 호출 (3.5 참고)
connection.on("OrderCompleted", (id) => {
  addSystem(`주문 #${id} 처리 완료`);
});
```

시간을 채팅 목록에 `addSystem()`으로 찍으면 1초마다 메시지가 쌓여 채팅이 시계로 가득 찹니다. 그래서 **헤더의 한 곳을 덮어쓰는** 방식을 씁니다.

## 3.5 REST API에서 푸시하기

Hub 바깥에서 보내는 두 번째 방법입니다. `Program.cs`의 `app.Run();` 위에 추가합니다.

```csharp
// IHubContext<ChatHub>를 매개변수로 받으면 DI가 자동으로 넣어 준다.
app.MapPost("/api/orders/{id}/complete", async (int id, IHubContext<ChatHub> hub) =>
{
    // 실제 서비스라면 여기서 DB 저장 등 비즈니스 로직을 먼저 수행
    await hub.Clients.All.SendAsync("OrderCompleted", id);
    return Results.Ok(new { id, status = "completed" });
});
```

터미널에서 호출합니다. (포트는 실제 값으로 교체)

```bash
curl -X POST http://localhost:5241/api/orders/1/complete
```

브라우저를 열어 둔 채 실행하면 채팅 화면에 "주문 #1 처리 완료"가 즉시 나타납니다.

```
curl ──▶ [REST API] ──(IHubContext)──▶ [SignalR] ──▶ 브라우저들의 on("OrderCompleted")
```

## 3.6 실습 체크리스트

| 확인 | 기대 결과 |
|---|---|
| `dotnet run` 후 접속 | 헤더의 서버 시간이 1초마다 바뀜 |
| 탭 2개 비교 | 두 탭의 시계가 거의 동시에 갱신 |
| `curl`로 POST 호출 | 열린 모든 탭에 "주문 #1 처리 완료" |
| 서버 재시작 | 자동 재연결 후 시계가 다시 움직임 |

## 3.7 자주 만나는 문제

| 증상 | 원인 |
|---|---|
| 시간이 `--:--:--`로 고정 | `AddHostedService<ClockWorker>()` 누락, 또는 `on("Tick")` 이름 오타 |
| `ChatHub`를 찾을 수 없음 | `using SignalR.Hubs;`가 ChatHub의 실제 네임스페이스와 다름 |
| `IHubContext`에 빨간 줄 | `using Microsoft.AspNetCore.SignalR;` 누락 |
| "연결됨" 글자가 안 바뀜 | 안쪽 `id="status"`를 지워 버림 |

## 3.8 생각해 보기

1. Hub 안에서는 `Clients.Caller`를 쓸 수 있는데, ClockWorker에서는 왜 쓸 수 없을까?
2. `AddHostedService`를 빼먹으면 왜 오류가 나지 않을까?
3. 시계 정보를 DB에 저장하고 싶다면 어디에서 해야 할까? (Hub? ClockWorker?)

---

# 4단계. 스트리밍: 값을 여러 번에 나눠 보내기

**만드는 파일:** `Hubs/StreamHub.cs`, `wwwroot/js/stream.js`, `wwwroot/stream.html`

## 4.1 학습 목표

- 일반 호출(`invoke`)과 스트리밍(`stream`)의 차이를 설명한다.
- 서버 → 클라이언트 스트리밍을 `IAsyncEnumerable`로 구현한다.
- 클라이언트 → 서버 스트리밍을 `Subject`로 구현한다.
- 취소(`dispose`)가 서버까지 전달되는 원리를 이해한다.

## 4.2 일반 호출과 스트리밍 비교

```
[일반 invoke]     요청 ──▶  ........ 처리 ........ ──▶ 결과 1번

[서버→클라 스트림] 요청 ──▶  값1 ─▶ 값2 ─▶ 값3 ─▶ ... ─▶ 완료    (서버가 여러 번 보냄)

[클라→서버 스트림] 조각1 ─▶ 조각2 ─▶ 조각3 ─▶ 완료 ──▶ 서버가 하나씩 읽음  (클라이언트가 여러 번 보냄)
```

| 용도 | 예 |
|---|---|
| 서버 → 클라이언트 | 진행률(%) 표시, 실시간 시세, 긴 작업의 중간 결과 |
| 클라이언트 → 서버 | 파일/센서 데이터를 나눠서 업로드 |

## 4.3 서버 코드: Hubs/StreamHub.cs

```csharp
using System.Runtime.CompilerServices;   // [EnumeratorCancellation]에 필요
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class StreamHub : Hub
{
    // ───── 서버 → 클라이언트 스트리밍 ─────
    public async IAsyncEnumerable<int> Counter(
        int count,
        int delayMs,
        [EnumeratorCancellation] CancellationToken ct)   // 클라이언트가 취소하면 이 토큰이 취소됨
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
        // 메서드가 끝나면 클라이언트의 complete 콜백이 호출됨
    }

    // ───── 클라이언트 → 서버 스트리밍 ─────
    public async Task Upload(IAsyncEnumerable<string> chunks)
    {
        var count = 0;

        // 클라이언트가 subject.next()를 할 때마다 한 번씩 반복, complete()하면 끝남
        await foreach (var chunk in chunks)
        {
            count++;
            Console.WriteLine($"[Upload] {count}번째 조각: {chunk}");
        }

        await Clients.Caller.SendAsync("UploadDone", count);   // 끝난 뒤 호출자에게 결과 통보
    }
}
```

Program.cs에 한 줄을 추가합니다.

```csharp
app.MapHub<StreamHub>("/streamHub");
```

### 서버 개념 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `IAsyncEnumerable<T>` 반환 | 스트리밍 메서드 표시 | SignalR이 이 반환 형식을 보고 스트림으로 인식 |
| `yield return` | 값 하나 전송 | 값이 만들어질 때마다 즉시 클라이언트로 전달 |
| `[EnumeratorCancellation]` | 취소 토큰 연결 | **빠뜨리면 클라이언트가 취소해도 서버가 계속 돔** |
| `ct.ThrowIfCancellationRequested()` | 취소 확인 | 취소 요청이 있으면 예외로 루프 종료 |
| `Math.Clamp(값, 최소, 최대)` | 범위 제한 | 입력 검증. 클라이언트가 `count = 100000000`을 보내도 막음 |
| 인자 `IAsyncEnumerable<T>` | 클라→서버 스트림 수신 | `await foreach`로 순서대로 읽음 |
| `await foreach` | 비동기 반복 | 값이 올 때마다 한 번씩 실행, 스트림이 끝나면 종료 |

## 4.4 클라이언트 코드: wwwroot/js/stream.js

```javascript
const streamConnection = new signalR.HubConnectionBuilder()
  .withUrl("/streamHub")
  .withAutomaticReconnect()
  .build();

function log(text) {
  const el = document.getElementById("log");
  if (!el) return console.log(text);
  el.textContent += text + "\n";
  el.scrollTop = el.scrollHeight;
}

// 서버가 Upload를 마치면 호출 (start 전에 등록)
streamConnection.on("UploadDone", (count) => log(`[업로드 완료] 서버가 ${count}개 조각을 받음`));

// ───── 서버 → 클라이언트 스트리밍 ─────
let currentSub = null;   // 취소하려면 구독 객체가 필요

function startCounter(count = 10, delayMs = 500) {
  stopCounter();   // 이전 구독이 있으면 정리
  log(`--- 카운터 시작 (${count}개, ${delayMs}ms 간격) ---`);

  currentSub = streamConnection.stream("Counter", count, delayMs).subscribe({
    next:     (n) => log(`값: ${n}`),                                  // 값 하나 도착
    complete: ()  => { log("--- 완료 ---"); currentSub = null; },       // 서버가 끝냄
    error:    (e) => { log("오류: " + e);   currentSub = null; }        // 오류 또는 연결 끊김
  });
}

function stopCounter() {
  if (currentSub) {
    currentSub.dispose();    // 서버의 CancellationToken이 취소됨
    currentSub = null;
    log("--- 사용자가 취소 ---");
  }
}

// ───── 클라이언트 → 서버 스트리밍 ─────
async function startUpload(chunks = ["조각-1", "조각-2", "조각-3", "조각-4"]) {
  const subject = new signalR.Subject();                       // 보낼 값을 밀어 넣는 통로

  // 여기서 await 하면 안 된다. 서버 메서드가 complete()를 기다리며 끝나지 않기 때문이다.
  const done = streamConnection.invoke("Upload", subject);

  for (const chunk of chunks) {
    subject.next(chunk);                                       // 조각 하나 전송
    log(`전송: ${chunk}`);
    await new Promise(r => setTimeout(r, 400));
  }

  subject.complete();                                          // "더 보낼 게 없음"
  await done;                                                  // 서버 메서드 종료까지 대기
}

streamConnection.onreconnecting(() => log("재연결 중... (진행 중이던 스트림은 끊깁니다)"));
streamConnection.onreconnected(()  => log("재연결됨"));
streamConnection.onclose(()        => log("연결 종료"));

streamConnection.start()
  .then(() => {
    log("streamHub 연결됨");
    document.querySelectorAll("button").forEach(b => b.disabled = false);
  })
  .catch(err => log("연결 실패: " + err));
```

### 클라이언트 메서드 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `connection.stream(이름, 인자...)` | 스트림 구독 시작 | **`invoke`가 아니다.** 스트리밍 메서드는 반드시 `stream`으로 호출 |
| `.subscribe({next, complete, error})` | 콜백 등록 | 값 도착 / 정상 종료 / 오류 |
| `subscription.dispose()` | 구독 취소 | 서버의 `CancellationToken`이 취소됨 |
| `new signalR.Subject()` | 업로드 통로 | `next(값)`으로 보내고 `complete()`로 끝냄 |
| `subject.next(값)` | 값 전송 | 서버의 `await foreach`가 한 번 반복됨 |
| `subject.complete()` | 전송 종료 | **빠뜨리면 서버 `Upload`가 끝나지 않음** |

## 4.5 취소가 서버까지 전달되는 과정

```
클라이언트: currentSub.dispose()
     │  (취소 메시지 전송)
     ▼
서버: ct가 취소됨
     │
     ▼
서버: ct.ThrowIfCancellationRequested() 또는 Task.Delay(…, ct)에서 예외 발생 → 루프 종료
```

## 4.6 실습

`http://localhost:포트/stream.html`에서 버튼으로 확인합니다.

| 동작 | 화면 로그 | 서버 터미널 |
|---|---|---|
| "서버→클라 시작" | 0.5초마다 `값: 1`, `값: 2` ... `--- 완료 ---` | 없음 |
| 중간에 "취소" | `--- 사용자가 취소 ---` 후 값이 멈춤 | 없음 |
| "클라→서버 업로드" | `전송: 조각-1` ... `[업로드 완료] 서버가 4개 조각을 받음` | `[Upload] 1번째 조각: 조각-1` ... |

### 일부러 틀려 보기

| 변경 | 결과 |
|---|---|
| `stream("Counter")` 대신 `invoke("Counter")` | 스트리밍 메서드를 일반 호출했다는 오류 |
| `subject.complete()` 줄 삭제 | 서버 `Upload`가 끝나지 않아 "업로드 완료"가 안 뜸 |
| `[EnumeratorCancellation]` 삭제 | 취소해도 서버가 계속 값을 만듦 (빌드 경고 발생 가능) |
| `const done = invoke(...)`를 `await invoke(...)`로 변경 | 이후 `next()`가 실행되지 않아 멈춤 |

## 4.7 자주 만나는 문제

| 증상 | 원인 |
|---|---|
| `CancellationToken`, `EnumeratorCancellation`에 빨간 줄 | `using System.Runtime.CompilerServices;` 누락 |
| 스트림 도중 서버를 재시작했더니 이어서 오지 않음 | 정상. 스트림은 연결에 묶여 있어 **재연결로 복구되지 않고** `error`로 끝남. 필요하면 `error`에서 다시 `startCounter()` 호출 |
| `StreamHub`를 찾을 수 없음 | 파일 확장자 `.cs`, `namespace SignalR.Hubs;` 확인 |

## 4.8 생각해 보기

1. 진행률 표시를 스트리밍 대신 `Clients.Caller.SendAsync("Progress", 10)`처럼 여러 번 보내도 될까? 둘의 차이는?
2. `Math.Clamp`로 `count`를 제한하는 이유를 한 문장으로 설명해 보자.
3. 스트림 도중 사용자가 탭을 닫으면 서버의 `ct`는 어떻게 될까?

---

# 5단계. Hub 필터: 모든 호출에 공통 로직 끼우기

**만드는 파일:** `Filters/LoggingFilter.cs` (+ 실습 `Filters/BadWordFilter.cs`)

## 5.1 학습 목표

- Hub 필터가 필요한 이유(공통 로직 중복 제거)를 설명한다.
- `IHubFilter`로 로깅 필터를 만들고 `AddFilter`로 등록한다.
- `next(context)`를 호출하지 않으면 **호출이 차단**된다는 점을 실습한다.
- `HubException`과 일반 예외의 차이를 이해한다.

## 5.2 문제 상황

모든 Hub 메서드에서 "호출 로그 남기기", "시간 재기", "예외 기록"을 하고 싶다면 메서드마다 같은 코드를 복사해야 할까요? 필터는 이런 **공통 코드를 한 곳**에 모읍니다. ASP.NET Core의 **미들웨어와 비슷한 개념**입니다.

```
클라이언트 invoke("SendMessage")
        │
        ▼
 [LoggingFilter]   호출 전: 로그, 시간 측정 시작
        │  await next(context)
        ▼
 [BadWordFilter]   금칙어 검사 (실습)  ── 위반이면 next를 호출하지 않고 예외 → 여기서 차단
        │  await next(context)
        ▼
 [ChatHub.SendMessage]  실제 메서드 실행
        │
        ▼
 [LoggingFilter]   호출 후: 소요 시간 로그, 예외 기록
        │
        ▼
 클라이언트로 결과 반환
```

## 5.3 서버 코드: Filters/LoggingFilter.cs

```csharp
using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Filters;

public class LoggingFilter : IHubFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    // 필터도 DI로 만들어지므로 ILogger 등을 주입받을 수 있다.
    public LoggingFilter(ILogger<LoggingFilter> logger) => _logger = logger;

    // ───── 1) 클라이언트가 Hub 메서드를 호출할 때마다 실행 ─────
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,                          // 메서드명, 인자, 연결 정보
        Func<HubInvocationContext, ValueTask<object?>> next)   // 다음 단계 = 실제 Hub 메서드
    {
        var sw = Stopwatch.StartNew();

        // 채팅 내용은 개인정보일 수 있으므로 인자 값은 남기지 않고 개수만 기록
        _logger.LogInformation("[호출] {Hub}.{Method} (인자 {ArgCount}개) conn={ConnId}",
            context.Hub.GetType().Name,
            context.HubMethodName,
            context.HubMethodArguments.Count,
            context.Context.ConnectionId);

        try
        {
            var result = await next(context);   // 이 줄을 호출하지 않으면 메서드가 실행되지 않음
            _logger.LogInformation("[완료] {Method} {Elapsed}ms",
                context.HubMethodName, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[오류] {Method} {Elapsed}ms",
                context.HubMethodName, sw.ElapsedMilliseconds);
            throw;   // 삼키지 말고 다시 던져서 SignalR이 클라이언트에 오류를 전달하게 한다
        }
    }

    // ───── 2) 연결될 때 실행 (선택 구현) ─────
    public async Task OnConnectedAsync(
        HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        _logger.LogInformation("[연결] {Hub} conn={ConnId}",
            context.Hub.GetType().Name, context.Context.ConnectionId);
        await next(context);
    }

    // ───── 3) 연결이 끊길 때 실행 (선택 구현) ─────
    public async Task OnDisconnectedAsync(
        HubLifetimeContext context, Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        _logger.LogInformation("[종료] {Hub} conn={ConnId} 사유={Reason}",
            context.Hub.GetType().Name,
            context.Context.ConnectionId,
            exception?.Message ?? "정상 종료");
        await next(context, exception);
    }
}
```

### Program.cs: 기존 `AddSignalR()`를 교체

```csharp
using SignalR.Filters;     // ← 추가
using SignalR.Hubs;
using SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(options =>
{
    options.AddFilter<LoggingFilter>();   // 모든 Hub에 적용
});
builder.Services.AddHostedService<ClockWorker>();
// ... 이하 동일
```

`AddSignalR()`를 **한 번 더 호출하는 것이 아니라** 기존 줄을 교체합니다.

### 필터 개념 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `IHubFilter` | 필터 인터페이스 | 구현하면 Hub 호출을 가로챌 수 있음 |
| `InvokeMethodAsync` | 메서드 호출 가로채기 | 클라이언트가 Hub 메서드를 호출할 때마다 실행 |
| `OnConnectedAsync` / `OnDisconnectedAsync` | 연결/종료 가로채기 | 선택 구현. 인터페이스에 기본 구현이 있어 생략 가능 |
| `HubInvocationContext` | 호출 정보 | `HubMethodName`, `HubMethodArguments`, `Context`, `Hub` |
| `next(context)` | 다음 단계 실행 | **호출하지 않으면 차단.** 다음 필터 또는 실제 메서드로 넘어감 |
| `options.AddFilter<T>()` | 필터 등록 | 모든 Hub에 적용. **등록한 순서가 실행 순서** (먼저 등록 = 바깥쪽) |
| `Stopwatch` | 시간 측정 | `ElapsedMilliseconds`로 소요 시간 확인 |
| `ILogger<T>` | 로그 기록 | 터미널(콘솔)에 `info`, `fail` 등으로 출력 |

## 5.4 실행 결과

채팅 메시지를 보내면 **서버 터미널**에 다음과 같이 찍힙니다.

```
info: SignalR.Filters.LoggingFilter[0]  [연결] ChatHub conn=AbC123...
info: SignalR.Filters.LoggingFilter[0]  [호출] ChatHub.SendMessage (인자 2개) conn=AbC123...
info: SignalR.Filters.LoggingFilter[0]  [완료] SendMessage 3ms
info: SignalR.Filters.LoggingFilter[0]  [종료] ChatHub conn=AbC123... 사유=정상 종료
```

`rooms.html`, `stream.html`을 열어도 `RoomHub`, `StreamHub`의 로그가 똑같이 찍힙니다. **필터 한 번 등록으로 모든 Hub에 적용**된다는 점이 핵심입니다.

## 5.5 실습: 금칙어를 막는 필터

`next`를 호출하지 않으면 Hub 메서드가 실행되지 않는다는 점을 체험합니다.

### Filters/BadWordFilter.cs

```csharp
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
        if (context.HubMethodName == "SendMessage"
            && context.HubMethodArguments.Count >= 2
            && context.HubMethodArguments[1] is string message
            && BadWords.Any(w => message.Contains(w, StringComparison.OrdinalIgnoreCase)))
        {
            // HubException의 메시지는 클라이언트에 그대로 전달된다.
            throw new HubException("금칙어가 포함된 메시지는 보낼 수 없습니다.");
        }

        return await next(context);   // 통과하면 정상 실행
    }
}
```

### Program.cs: 등록 순서 = 실행 순서

```csharp
builder.Services.AddSignalR(options =>
{
    options.AddFilter<LoggingFilter>();    // 바깥쪽: 모든 호출을 기록 (차단된 호출도 [오류]로 기록됨)
    options.AddFilter<BadWordFilter>();    // 안쪽: 금칙어 검사
});
```

금칙어를 보내면 index.html의 `send()` 안의 `catch (err)`가 `전송 실패: ...`를 표시하고, **다른 사람에게는 전달되지 않습니다.**

### HubException vs 일반 Exception

| 던진 예외 | 클라이언트에 보이는 메시지 | 용도 |
|---|---|---|
| `HubException("메시지")` | 메시지가 **그대로** 전달됨 | 사용자에게 알려도 되는 오류 (검증 실패 등) |
| 일반 `Exception` | "오류 발생" 같은 **일반 문구**로 가려짐 | 내부 정보 노출 방지. 서버 로그에서 확인 |

> `EnableDetailedErrors = true`로 설정하면 상세 메시지가 보이지만 **개발 환경에서만** 사용합니다.

## 5.6 자주 만나는 문제

| 증상 | 원인 |
|---|---|
| 로그가 안 찍힘 | `AddFilter`를 `AddSignalR()` 옵션 안에 쓰지 않았거나, `AddSignalR()`를 두 번 호출해 한쪽 설정이 무시됨 |
| `HubInvocationContext`에 빨간 줄 | `using Microsoft.AspNetCore.SignalR;` 누락 |
| 차단했는데 클라이언트가 계속 기다림 | 차단 시 `HubException`을 던지거나 값을 반환해야 함. 아무 처리 없이 빠져나가면 안 됨 |
| 스트리밍 메서드의 소요 시간이 매우 짧게 나옴 | 필터는 스트림을 **만드는 시점**까지만 측정. 값이 하나씩 나가는 시간은 포함되지 않음 |
| 로그에 채팅 내용/토큰이 남음 | 인자 값을 로그에 쓰지 말고 메서드명/개수만 남긴다 |

## 5.7 생각해 보기

1. `LoggingFilter`와 `BadWordFilter`의 등록 순서를 바꾸면 금칙어 차단 시 로그가 어떻게 달라질까?
2. 필터 대신 `ChatHub.SendMessage` 안에서 금칙어를 검사하면 어떤 단점이 있을까?
3. 필터에서 `catch`한 예외를 `throw;` 없이 삼키면 클라이언트에는 어떻게 보일까?

---

# 6단계. 인증: 로그인한 사용자에게 알림 보내기

**만들 파일:** `Hubs/NotifyHub.cs` (+ `Program.cs` 인증 설정)

> **진행 상태 안내:** 6~7단계는 아직 실습 전 단계입니다. 아래 코드는 **개념 설명용 개요 코드**이며, 수업에서 직접 실행하며 확인한 뒤 사용합니다. 특히 JWT 설정은 .NET 버전에 따라 세부 동작(클레임 이름 매핑 등)이 달라질 수 있습니다.

## 6.1 학습 목표

- 인증(Authentication)과 인가(Authorization)의 차이를 설명한다.
- `ConnectionId`와 `UserIdentifier`의 차이, 그리고 사용자 단위 전송(`Clients.User`)을 이해한다.
- 브라우저 WebSocket에서 JWT가 **쿼리스트링**으로 전달되는 이유를 설명한다.

## 6.2 왜 인증이 필요한가

지금까지의 채팅은 누구나 접속해서 아무 이름이나 쓸 수 있었습니다. 실제 서비스에서는 다음이 필요합니다.

| 필요 | 설명 |
|---|---|
| 사용자 식별 | "이 연결은 누구인가?" (ConnectionId는 임시값이라 사람을 가리키지 못함) |
| 접근 제어 | 로그인한 사람만, 또는 관리자만 특정 메서드를 호출 |
| 사용자 단위 전송 | 한 사용자가 폰/PC/탭 여러 곳에 접속해도 **한 번에** 알림 전달 |

| 용어 | 뜻 | 질문 |
|---|---|---|
| **인증** (Authentication) | 신원 확인 | "당신은 누구입니까?" |
| **인가** (Authorization) | 권한 확인 | "당신은 이 일을 해도 됩니까?" |

## 6.3 ConnectionId vs UserIdentifier

```
        사용자 alice (UserIdentifier = "alice")
        ├── 노트북 탭 1   ConnectionId = AAA
        ├── 노트북 탭 2   ConnectionId = BBB
        └── 스마트폰      ConnectionId = CCC

  Clients.Client("AAA")    → 탭 1에만 전달
  Clients.User("alice")    → AAA, BBB, CCC 모두에게 전달  ← 사용자 단위
```

| | ConnectionId | UserIdentifier |
|---|---|---|
| 의미 | 연결 하나 | 로그인한 사용자 |
| 변화 | 재연결하면 바뀜 | 로그인 사용자가 같으면 유지 |
| 기본 출처 | SignalR이 자동 생성 | 토큰/쿠키의 `NameIdentifier` 클레임 |
| 로그인 필요 | 아니오 | 예 |

## 6.4 인증 방식 두 가지

| 방식 | 적합한 경우 | SignalR에서의 동작 |
|---|---|---|
| **쿠키 인증** | 같은 도메인의 웹앱 | 브라우저가 쿠키를 자동 전송. 클라이언트 추가 코드 없음 |
| **JWT 토큰** | SPA, 모바일, 다른 도메인 | 토큰을 직접 붙여야 함 (아래 참고) |

### JWT가 쿼리스트링으로 전달되는 이유

일반 API는 `Authorization: Bearer 토큰` 헤더를 씁니다. 하지만 **브라우저의 WebSocket API는 요청 헤더를 설정할 수 없습니다.** 그래서 SignalR JS 클라이언트는 WebSocket 연결 시 토큰을 `?access_token=...`으로 붙이고, 서버가 그 값을 읽어 토큰으로 인정하도록 설정합니다.

```
 클라이언트                                       서버
  accessTokenFactory: () => token
  ──▶ wss://host/notifyHub?access_token=eyJ...  ──▶  OnMessageReceived에서
                                                      Query["access_token"]을 토큰으로 사용
                                                      → 검증 → Context.User, UserIdentifier 설정
```

## 6.5 서버 코드 (개요)

### Hubs/NotifyHub.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

[Authorize]   // 클래스 전체: 로그인하지 않으면 연결 자체가 거부됨
public class NotifyHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // 같은 사용자가 탭 3개를 열면 UserIdentifier는 같고 ConnectionId는 3개
        Console.WriteLine($"접속: {Context.UserIdentifier} / {Context.ConnectionId}");
        await base.OnConnectedAsync();   // 반드시 base 호출
    }

    // 특정 사용자의 모든 기기/탭에 전달
    public Task SendToUser(string userId, string message)
        => Clients.User(userId).SendAsync("Notify", message);

    [Authorize(Roles = "Admin")]   // 메서드 단위 인가: Admin 역할만
    public Task Broadcast(string message)
        => Clients.All.SendAsync("Notice", message);
}
```

### Program.cs에 추가할 설정 (개요)

```csharp
// 필요한 패키지 (사용 중인 .NET 버전에 맞는 버전을 설치)
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// 수업용 키. 실제 서비스에서는 코드에 넣지 말고 user-secrets / 환경 변수로 관리한다.
var jwtKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("demo-secret-key-at-least-32-characters-long!"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,            // 수업 단순화를 위해 생략
            ValidateAudience = false,
            ValidateLifetime = true,           // 만료 검사
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtKey
        };

        // WebSocket 연결은 헤더를 못 쓰므로 쿼리스트링의 토큰을 읽는다
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    ctx.HttpContext.Request.Path.StartsWithSegments("/notifyHub"))  // 허브 경로에만 허용
                {
                    ctx.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// 토큰의 사용자 ID 클레임 이름은 설정/버전에 따라 달라질 수 있어 직접 지정하는 방법
builder.Services.AddSingleton<IUserIdProvider, SubUserIdProvider>();

// ... var app = builder.Build(); 이후, 순서 중요: 인증 → 인가 → MapHub
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotifyHub>("/notifyHub");

// 수업용 로그인 API: 비밀번호 검증이 없는 데모이므로 실제 서비스에서는 사용 금지
app.MapPost("/api/login", (LoginRequest req) =>
{
    var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, req.UserId) };
    var creds = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        claims: claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: creds);
    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});

app.Run();

// 최상위 문(statement) 뒤, 파일 맨 아래에 선언
record LoginRequest(string UserId);

public class SubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? connection.User?.FindFirst("sub")?.Value;
}
```

### 클라이언트 코드 (개요)

```javascript
// ① 로그인해서 토큰을 받는다 (수업용: 아이디만 전송)
const res = await fetch("/api/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({ userId: "alice" })
});
const { token } = await res.json();

// ② 토큰을 붙여 연결한다
const notifyConnection = new signalR.HubConnectionBuilder()
  .withUrl("/notifyHub", {
    accessTokenFactory: () => token    // 연결/재연결 시마다 호출되어 토큰을 제공
  })
  .withAutomaticReconnect()
  .build();

notifyConnection.on("Notify", (msg) => console.log("[알림]", msg));
await notifyConnection.start();

// ③ 다른 사용자에게 보내기
await notifyConnection.invoke("SendToUser", "bob", "안녕, bob!");
```

### 개념 설명

| 이름 | 역할 | 설명 |
|---|---|---|
| `[Authorize]` | 인가 특성 | 클래스에 붙이면 Hub 전체, 메서드에 붙이면 그 메서드만 보호 |
| `[Authorize(Roles = "Admin")]` | 역할 기반 인가 | 특정 역할만 호출 가능 |
| `Context.User` | 현재 사용자 | `ClaimsPrincipal`. 클레임(이름, 역할 등) 조회 |
| `Context.UserIdentifier` | 사용자 ID | 기본값은 `NameIdentifier` 클레임. `Clients.User()`가 이 값으로 찾음 |
| `Clients.User(id)` | 사용자 대상 전송 | 그 사용자의 **모든 연결**에 전달 |
| `IUserIdProvider` | 사용자 ID 규칙 변경 | 이메일 등 다른 클레임을 ID로 쓰고 싶을 때 구현 |
| `accessTokenFactory` | 토큰 제공 함수 | 함수로 지정하면 재연결 때도 최신 토큰 사용 |
| `OnMessageReceived` | 토큰 위치 지정 | 쿼리스트링의 `access_token`을 토큰으로 사용 |
| `UseAuthentication` → `UseAuthorization` | 미들웨어 | 순서가 바뀌면 동작하지 않음. `MapHub`보다 앞에 위치 |

## 6.6 실습 계획

1. JwtBearer 패키지를 설치하고 위 설정을 `Program.cs`에 추가한다.
2. 탭 A에서 `alice`, 탭 B에서 `bob`으로 로그인해 `notifyConnection`을 만든다.
3. A에서 `SendToUser("bob", "hi")`를 호출하면 **B에서만** 알림이 뜨는지 확인한다.
4. 토큰 없이 연결을 시도하면 **401 Unauthorized**로 거부되는지 확인한다.
5. `Broadcast`를 일반 사용자가 호출하면 거부되는지 확인한다. (Roles 클레임이 없으므로)
6. 같은 사용자(`alice`)로 탭 2개를 열고, 다른 사용자가 `SendToUser("alice", ...)`를 호출해 **두 탭 모두** 받는지 확인한다.

## 6.7 보안 주의사항

| 항목 | 주의점 |
|---|---|
| 쿼리스트링 토큰 | 서버/프록시 로그에 남을 수 있음 → HTTPS 필수, 로그 마스킹, 짧은 토큰 수명 |
| 토큰 만료 | 연결이 한 번 맺어지면 **토큰이 만료돼도 연결은 유지**됨. 필요하면 서버에서 연결을 끊는 로직 추가 |
| 비밀 키 | 코드나 GitHub에 올리지 않는다. `dotnet user-secrets` 또는 환경 변수 사용 |
| 입력 검증 | Hub 메서드는 **공개 RPC 엔드포인트**. 모든 인자를 신뢰하지 않는다 |
| 방 입장 검사 | `JoinRoom`에서 이 사용자가 그 방에 들어갈 권한이 있는지 검사 |
| CORS | 다른 도메인에서 쓸 때 `AllowCredentials()`와 **명시적 오리진** 필요 (와일드카드 불가) |
| 메시지 크기 | `MaximumReceiveMessageSize`(기본 32KB)로 과도한 페이로드 방어 |
| 호출 빈도 | 한 연결이 무한정 호출하지 못하도록 Hub 필터(5단계)로 제한 |

## 6.8 생각해 보기

1. 같은 사용자가 두 탭을 열었을 때 `Clients.User`와 `Clients.Client`의 결과가 어떻게 다를까?
2. 브라우저 `localStorage`에 JWT를 저장하는 것의 장단점은?
3. 5단계의 필터에서 `context.Context.UserIdentifier`를 로그에 남기면 어떤 점이 좋아질까?

---

# 7단계. 스케일 아웃: 서버가 여러 대일 때

**관련 설정:** `Program.cs`의 `AddStackExchangeRedis` (코드량은 적고 개념이 중요한 단계)

> **진행 상태 안내:** 이 단계도 개념 중심입니다. Redis(Docker)가 필요하며, 수업 환경에 맞춰 실습 여부를 정합니다.

## 7.1 학습 목표

- 서버가 여러 대일 때 SignalR에서 **왜 문제가 생기는지** 설명한다.
- Backplane(Redis)과 Sticky Session의 역할을 구분한다.
- 서버 메모리에 저장한 상태가 서버 간에 공유되지 않는다는 점을 이해한다.

## 7.2 문제 상황: 연결은 서버 한 대에 붙어 있다

SignalR 연결은 **특정 서버 프로세스에 붙어 있는 상태(stateful)**입니다.

```
 사용자 A ──── [서버 1]                      사용자 B ──── [서버 2]

 서버 1에서 Clients.All.SendAsync(...) 실행
   → 서버 1에 붙은 A에게만 도착
   → 서버 2에 붙은 B는 모름                ← 문제 ①: 메시지 전달
```

| 문제 | 설명 |
|---|---|
| ① 메시지 전달 | 서버 1이 `Clients.All`을 호출해도 서버 2의 클라이언트에는 닿지 않음 |
| ② 연결 라우팅 | negotiate 요청과 이후 연결 요청이 **서로 다른 서버**로 가면 연결 실패 (WebSocket 외 전송 방식에서) |

## 7.3 해결 ①: Backplane (Redis)

Backplane은 서버들이 **메시지를 서로 전달하는 통로**입니다. Redis의 Pub/Sub 기능을 사용합니다.

```
 사용자 A ── [서버 1] ──┐                        ┌── [서버 2] ── 사용자 B
                        ├──▶  Redis (Pub/Sub)  ◀──┤
 서버 1이 메시지를 Redis에 발행 ──▶ 서버 2가 구독해서 받아 자기 클라이언트에 전달
```

```bash
dotnet add package Microsoft.AspNetCore.SignalR.StackExchangeRedis
```

```csharp
using StackExchange.Redis;   // RedisChannel

builder.Services.AddSignalR(options =>
{
    options.AddFilter<LoggingFilter>();
})
.AddStackExchangeRedis("localhost:6379", o =>
{
    // 같은 Redis를 여러 앱이 쓸 때 채널 이름 충돌 방지
    o.Configuration.ChannelPrefix = RedisChannel.Literal("SignalRDemo");
});
```

기존 `AddSignalR(...)` 뒤에 `.AddStackExchangeRedis(...)`를 **이어 붙입니다.**

### 수업용 데모 구성

```bash
# 터미널 1: Redis 실행 (Docker)
docker run -d --name signalr-redis -p 6379:6379 redis:7

# 터미널 2: 서버 1
dotnet run --urls http://localhost:5001

# 터미널 3: 서버 2
dotnet run --urls http://localhost:5002
```

- 브라우저 탭 1은 `localhost:5001`, 탭 2는 `localhost:5002`로 접속한다.
- **Redis 설정 전:** 탭 1에서 보낸 채팅이 탭 2에 안 보인다.
- **Redis 설정 후:** 서로 다른 서버에 연결돼 있어도 채팅이 전달된다.

### 데모에서 관찰할 점

| 관찰 | 이유 |
|---|---|
| 시계(`Tick`)가 1초에 **두 번** 갱신됨 | 서버 2대 각각의 `ClockWorker`가 `Clients.All`로 보내고, backplane이 이를 모든 서버의 클라이언트에 전달하기 때문 |
| `static` 딕셔너리 같은 서버 메모리 상태는 공유되지 않음 | Redis는 **메시지 전달**만 하고, 서버 변수를 동기화하지 않음 |
| Redis를 중지하면 그 사이 메시지가 유실됨 | Backplane은 전달을 보장하지 않음 |

## 7.4 해결 ②: Sticky Session 또는 WebSocket 전용

| 방법 | 설명 | 주의 |
|---|---|---|
| **Sticky Session** (세션 어피니티) | 같은 클라이언트의 요청을 항상 같은 서버로 보냄 (로드밸런서 설정) | Long Polling/SSE 사용 시 필요 |
| **WebSocket 전용 + `skipNegotiation`** | negotiate 단계를 생략하고 WebSocket만 사용 → 연결이 한 번만 맺어짐 | WebSocket을 쓸 수 없는 환경으로의 **폴백을 포기** |

```javascript
new signalR.HubConnectionBuilder()
  .withUrl("/chatHub", {
    transport: signalR.HttpTransportType.WebSockets,   // WebSocket만 사용
    skipNegotiation: true                              // negotiate 생략 (WebSockets 전용일 때만 가능)
  })
```

## 7.5 대안: Azure SignalR Service

| 항목 | 설명 |
|---|---|
| 개념 | 연결 관리를 클라우드 서비스에 위임. 앱 서버는 로직만 처리 |
| 장점 | 대규모 동시 연결, Sticky Session/Backplane 부담 감소 |
| 코드 변경 | `AddAzureSignalR()` 정도로 전환 가능 |
| 모드 | Default(앱 서버가 Hub 실행), Serverless(Azure Functions와 결합) |

### 선택 가이드

| 상황 | 추천 |
|---|---|
| 서버 1대, 학습/소규모 | 추가 설정 없음 |
| 자체 서버 여러 대 | Redis backplane + (Sticky Session 또는 WebSocket 전용) |
| 대규모 연결, 인프라 운영 부담 최소화 | Azure SignalR Service |

## 7.6 메시지 전달 보장: 반드시 알아야 할 한계

**SignalR은 기본적으로 메시지 전달을 보장하지 않습니다.**

- 연결이 끊긴 동안 보낸 메시지는 받지 못합니다. (2단계에서 확인)
- 중요한 데이터는 **DB에 저장 → SignalR로 "알림"만 → 재연결 시 REST로 다시 조회**하는 **Catch-up 패턴**을 씁니다.

```
 [서버]  ① 주문 DB 저장 ──▶ ② SignalR로 "새 주문 있음" 알림
 [클라]  알림 수신 → REST로 목록 조회            (끊긴 동안 놓쳤다면)
         재연결(onreconnected) → "마지막으로 받은 시점 이후"를 REST로 조회
```

## 7.7 운영 체크리스트

| 항목 | 내용 |
|---|---|
| 타임아웃 | `KeepAliveInterval`(기본 15초), `ClientTimeoutInterval`(기본 30초, KeepAlive의 최소 2배). 로드밸런서/프록시 유휴 시간(예: 60초)보다 KeepAlive를 짧게 |
| 리버스 프록시 | Nginx는 `Upgrade`/`Connection` 헤더와 `proxy_read_timeout` 설정, IIS는 WebSocket 기능 설치, Azure App Service는 WebSockets 옵션 켜기 |
| 성능 | 큰 객체 대신 ID/변경분만 전송, 고빈도 데이터는 서버에서 묶어서(배치/스로틀) 전송, 필요하면 MessagePack |
| 동시성 | Hub 메서드에서 `.Result`, `.Wait()` 등 블로킹 금지. 항상 `async/await` |
| 로그 | `Microsoft.AspNetCore.SignalR`, `Microsoft.AspNetCore.Http.Connections` 카테고리를 Debug로 올려 연결 문제 분석 |
| 상태 저장 | 서버 `static` 변수는 서버 1대일 때만 유효. 여러 대이면 Redis/DB 사용 |

```csharp
builder.Services.AddSignalR(o =>
{
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);        // 서버 → 클라이언트 ping 간격
    o.ClientTimeoutInterval = TimeSpan.FromSeconds(30);    // 이 시간 동안 메시지 없으면 끊김으로 간주
    o.MaximumReceiveMessageSize = 64 * 1024;               // 기본 32KB
    o.EnableDetailedErrors = builder.Environment.IsDevelopment();   // 운영에서는 false
});
```

## 7.8 생각해 보기

1. 서버 2대에 Redis backplane을 붙이면 `ClockWorker`의 시간이 중복 전송된다. 어떻게 하면 한 곳에서만 보내도록 바꿀 수 있을까?
2. 2단계의 그룹(`Groups`) 정보는 서버 메모리에 있다. backplane이 있으면 그룹 메시지는 서버 경계를 넘어 전달될까?
3. 결제 완료 알림처럼 **절대 놓치면 안 되는** 데이터는 SignalR만으로 충분할까? 이유와 대안은?

---

# 부록

## 부록 A. 1~5단계 통합 Program.cs

지금까지의 모든 단계를 한 파일에 합친 최종 형태입니다.

```csharp
using Microsoft.AspNetCore.SignalR;   // IHubContext
using SignalR.Filters;                // LoggingFilter, BadWordFilter
using SignalR.Hubs;                   // ChatHub, RoomHub, StreamHub
using SignalR.Services;               // ClockWorker

var builder = WebApplication.CreateBuilder(args);

// ── 서비스 등록 (builder.Build() 이전) ─────────────────────────
builder.Services.AddSignalR(options =>          // 1단계: SignalR 등록 (앱 전체에서 한 번만)
{
    options.AddFilter<LoggingFilter>();         // 5단계: 먼저 등록 = 바깥쪽
    options.AddFilter<BadWordFilter>();         // 5단계: 나중에 등록 = 안쪽
});
builder.Services.AddHostedService<ClockWorker>();   // 3단계: 백그라운드 서비스 등록

var app = builder.Build();

// ── 미들웨어 ──────────────────────────────────────────────────
app.UseDefaultFiles();
app.UseStaticFiles();

// ── Hub 주소 연결 (클라이언트 withUrl 주소와 같아야 함) ─────────
app.MapHub<ChatHub>("/chatHub");                // 1단계
app.MapHub<RoomHub>("/roomHub");                // 2단계
app.MapHub<StreamHub>("/streamHub");            // 4단계
// app.MapHub<TargetDemoHub>("/targetHub");     // (선택) 대상 지정 비교 예제

// ── REST에서 푸시 (3단계) ──────────────────────────────────────
app.MapPost("/api/orders/{id}/complete", async (int id, IHubContext<ChatHub> hub) =>
{
    await hub.Clients.All.SendAsync("OrderCompleted", id);
    return Results.Ok(new { id, status = "completed" });
});

app.Run();
```

### 주소 · Hub · 클라이언트 연결 지도

| 주소 | 서버 Hub | 클라이언트 파일 | 연결 변수 | 서버가 호출하는 핸들러 |
|---|---|---|---|---|
| `/chatHub` | `ChatHub` | `index.html` | `connection` | `ReceiveMessage`, `Tick`, `OrderCompleted` |
| `/roomHub` | `RoomHub` | `js/rooms.js` | `roomConnection` | `System`, `RoomMessage` |
| `/streamHub` | `StreamHub` | `js/stream.js` | `streamConnection` | `UploadDone` |

> 각 줄의 **세 군데 이름**(서버 `MapHub` 주소, 클라이언트 `withUrl` 주소, 핸들러 이름)이 어긋나는 것이 가장 흔한 버그입니다.

---

## 부록 B. 서버 API 치트시트

| 분류 | 이름 | 한 줄 설명 | 단계 |
|---|---|---|---|
| 등록 | `AddSignalR()` | SignalR 서비스 등록 | 1 |
| 등록 | `MapHub<T>(주소)` | Hub를 URL에 연결 | 1 |
| 등록 | `AddHostedService<T>()` | 백그라운드 서비스 등록 | 3 |
| 등록 | `options.AddFilter<T>()` | Hub 필터 등록 | 5 |
| Hub | `Hub` | Hub 기본 클래스 | 1 |
| Hub | `Context.ConnectionId` | 현재 연결 ID | 1 |
| Hub | `Context.User` / `UserIdentifier` | 현재 사용자 / 사용자 ID | 6 |
| Hub | `Context.Items` | 연결 하나 범위의 저장소 | - |
| Hub | `Context.GetHttpContext()` | 쿼리스트링, 헤더 접근 | - |
| Hub | `OnConnectedAsync()` / `OnDisconnectedAsync()` | 연결/종료 시 실행 (override) | 5, 6 |
| 전송 | `Clients.All` | 전체 | 1 |
| 전송 | `Clients.Caller` / `Others` | 호출자 / 호출자 제외 | 2 |
| 전송 | `Clients.Client(id)` | 특정 연결 | 2 |
| 전송 | `Clients.Group(name)` | 그룹 | 2 |
| 전송 | `Clients.User(id)` | 사용자의 모든 연결 | 6 |
| 전송 | `SendAsync(이름, 인자...)` | 클라이언트 핸들러 호출 | 1 |
| 그룹 | `Groups.AddToGroupAsync(connId, group)` | 그룹 가입 | 2 |
| 그룹 | `Groups.RemoveFromGroupAsync(connId, group)` | 그룹 탈퇴 | 2 |
| Hub 밖 | `IHubContext<T>` | Hub 바깥에서 전송 | 3 |
| 스트림 | `IAsyncEnumerable<T>` 반환 | 서버 → 클라이언트 스트림 | 4 |
| 스트림 | `IAsyncEnumerable<T>` 인자 | 클라이언트 → 서버 스트림 | 4 |
| 스트림 | `[EnumeratorCancellation]` | 취소 토큰 연결 | 4 |
| 필터 | `IHubFilter.InvokeMethodAsync` | 메서드 호출 가로채기 | 5 |
| 오류 | `HubException` | 메시지가 클라이언트에 전달되는 예외 | 5 |
| 인증 | `[Authorize]` | 인증/인가 필요 | 6 |
| 인증 | `IUserIdProvider` | 사용자 ID 규칙 지정 | 6 |
| 확장 | `AddStackExchangeRedis()` | Redis backplane | 7 |

## 부록 C. 클라이언트(JavaScript) API 치트시트

| 분류 | 이름 | 한 줄 설명 | 단계 |
|---|---|---|---|
| 생성 | `new signalR.HubConnectionBuilder()` | 연결 빌더 | 1 |
| 생성 | `.withUrl(주소, 옵션)` | 서버 주소와 옵션 | 1 |
| 생성 | `.withAutomaticReconnect([간격...])` | 자동 재연결 | 1, 2 |
| 생성 | `.build()` | 연결 객체 생성 | 1 |
| 수신 | `connection.on(이름, 함수)` | 서버 호출 핸들러 등록 (start 전에) | 1 |
| 시작 | `connection.start()` | 연결 시작 (Promise) | 1 |
| 호출 | `connection.invoke(이름, 인자...)` | 서버 메서드 호출, 결과 대기 | 1 |
| 호출 | `connection.send(이름, 인자...)` | 서버 메서드 호출, 결과 안 기다림 | 1 |
| 상태 | `connection.connectionId` | 내 연결 ID | 1 |
| 이벤트 | `onreconnecting(fn)` / `onreconnected(fn)` / `onclose(fn)` | 재연결 시작/성공/종료 | 1, 2 |
| 스트림 | `connection.stream(이름, 인자...)` | 서버 스트림 구독 | 4 |
| 스트림 | `.subscribe({next, complete, error})` | 스트림 콜백 | 4 |
| 스트림 | `subscription.dispose()` | 구독 취소 | 4 |
| 스트림 | `new signalR.Subject()` + `next` / `complete` | 클라이언트 → 서버 업로드 | 4 |
| 인증 | `withUrl(주소, { accessTokenFactory })` | JWT 제공 | 6 |
| 전송 | `withUrl(주소, { transport, skipNegotiation })` | 전송 방식 지정 | 7 |

---

## 부록 D. 오류 메시지 해석표 (수업 중 실제 발생 사례 포함)

| 오류 / 증상 | 어디서 | 원인 | 해결 |
|---|---|---|---|
| 한글이 깨져 보임 | 브라우저 | `<meta charset="UTF-8">` 없음, 파일이 UTF-8이 아님 | meta 추가, 파일을 UTF-8로 저장, `Cmd+Shift+R` |
| 터미널 한글 깨짐 | 콘솔 | Windows 콘솔 코드 페이지 | `Console.OutputEncoding = Encoding.UTF8`, Windows Terminal 사용 |
| `CS0246: 형식 또는 네임스페이스 이름을 찾을 수 없습니다` | Visual Studio / VS Code | 확장자가 `.cs` 아님, `namespace`와 `using` 불일치, 오타 | 파일명, namespace, using 철자 비교 후 `dotnet build` |
| `Uncaught ReferenceError: connection is not defined` | 브라우저 콘솔 | 변수명을 일부만 바꿈 | 파일 안의 변수명 통일 (`roomConnection` 등) |
| `signalR is not defined` | 브라우저 콘솔 | 라이브러리 `<script>`가 뒤에 있음 | 라이브러리 먼저 로드 |
| `Identifier 'connection' has already been declared` | 브라우저 콘솔 | 한 페이지에 같은 이름 두 번 선언 | Hub별 다른 이름 사용 |
| negotiate `404` | 브라우저 Network | `MapHub` 누락 또는 주소 불일치 | Program.cs와 `withUrl` 비교 |
| `Method does not exist` | invoke 호출 시 | 서버 메서드 이름/존재 오류 | Hub 메서드명 확인 |
| 메시지가 안 보이는데 오류 없음 | 화면 | 클라이언트 `on` 이름과 서버 `SendAsync` 이름 불일치 | 대소문자 포함 이름 비교 |
| 연결 직후 메시지를 놓침 | 화면 | `on()`을 `start()` 뒤에 등록 | `start()` 전에 등록 |
| 재연결 후 방 메시지 안 옴 | 화면 | 그룹 재가입 누락 | `onreconnected`에서 `JoinRoom` 재호출 |
| 시계가 안 움직임 | 화면 | `AddHostedService` 누락 | 등록 확인 |
| 로그가 안 찍힘 | 터미널 | `AddFilter` 미등록 / `AddSignalR` 중복 | `AddSignalR(options => ...)` 한 곳에서 등록 |
| 스트림 취소해도 서버가 계속 동작 | 서버 | `[EnumeratorCancellation]` 누락 | 특성 추가 |
| 업로드가 끝나지 않음 | 화면 | `subject.complete()` 누락 / `await invoke` 위치 오류 | 4단계 코드와 비교 |
| 401 Unauthorized | 브라우저 | 토큰 누락/만료/검증 설정 오류 | 토큰, `OnMessageReceived` 확인 |
| CORS 오류 | 브라우저 | 다른 오리진에서 접속 | `WithOrigins(...).AllowCredentials()` |
| `dotnet run` 포트 충돌 | 터미널 | 이미 같은 포트 사용 중 | `--urls`로 다른 포트 지정 |

---

## 부록 E. 확인 문제

### 객관식 / 단답형

**Q1.** 서버가 클라이언트의 `on("ReceiveMessage")` 핸들러를 호출할 때 쓰는 서버 측 메서드는?
① `invoke` ② `SendAsync` ③ `MapHub` ④ `AddSignalR`

**Q2.** Hub 클래스의 필드에 값을 저장해도 다음 호출에서 사라지는 이유는?

**Q3.** `connection.on(...)`은 `start()` 전/후 중 언제 등록해야 하는가? 이유는?

**Q4.** 다음 중 "호출한 본인을 제외한 모두"에게 보내는 대상은?
① `Clients.All` ② `Clients.Caller` ③ `Clients.Others` ④ `Clients.Group`

**Q5.** 재연결 후 그룹 소속이 사라지는 이유는?

**Q6.** Hub 바깥(BackgroundService, REST API)에서 클라이언트에 메시지를 보낼 때 쓰는 인터페이스는?

**Q7.** `AddHostedService<ClockWorker>()`를 빼먹으면 어떻게 되는가?

**Q8.** 서버 → 클라이언트 스트리밍 메서드의 반환 형식은 무엇이며, 클라이언트는 `invoke`와 `stream` 중 무엇으로 호출하는가?

**Q9.** 스트림을 취소했을 때 서버 루프가 멈추게 하려면 매개변수에 어떤 특성을 붙여야 하는가?

**Q10.** Hub 필터에서 `await next(context)`를 호출하지 않으면 어떻게 되는가?

**Q11.** `HubException`과 일반 `Exception`을 던졌을 때 클라이언트가 받는 메시지의 차이는?

**Q12.** 브라우저에서 JWT가 헤더가 아닌 쿼리스트링(`access_token`)으로 전달되는 이유는?

**Q13.** 서버가 2대일 때 서버 1의 `Clients.All`이 서버 2의 클라이언트에 닿지 않는 문제를 해결하는 대표 방법은?

**Q14.** `Clients.User("alice")`와 `Clients.Client("AAA")`의 차이는?

**Q15.** SignalR이 메시지 전달을 보장하지 않으므로 중요한 데이터(주문 등)는 어떤 패턴으로 처리하는가?

### 정답

| 문항 | 정답 |
|---|---|
| Q1 | ② `SendAsync` |
| Q2 | Hub 인스턴스는 클라이언트가 메서드를 호출할 때마다 **새로 생성**되기 때문. 공유 상태는 `static` 컬렉션이나 DI로 주입한 싱글톤 서비스, DB에 둔다 |
| Q3 | **`start()` 전.** 연결 직후 서버가 보내는 메시지를 놓치지 않기 위해 |
| Q4 | ③ `Clients.Others` |
| Q5 | 재연결하면 **새 ConnectionId**가 발급되고, 서버는 이를 새로운 연결로 보기 때문 |
| Q6 | `IHubContext<THub>` |
| Q7 | 컴파일과 실행은 정상이지만 **백그라운드 서비스가 시작되지 않음** (오류 메시지 없음) |
| Q8 | 반환 형식은 `IAsyncEnumerable<T>` (또는 `ChannelReader<T>`). 클라이언트는 **`stream`** 으로 호출 |
| Q9 | `[EnumeratorCancellation] CancellationToken` |
| Q10 | 다음 필터와 **실제 Hub 메서드가 실행되지 않는다** (호출 차단). 차단 시에는 `HubException` 등으로 응답해야 함 |
| Q11 | `HubException`은 메시지가 **그대로** 전달됨. 일반 `Exception`은 내부 정보 보호를 위해 **일반 문구**로 가려짐 (`EnableDetailedErrors` 설정 시 예외) |
| Q12 | 브라우저의 WebSocket API가 **요청 헤더를 설정할 수 없기** 때문 |
| Q13 | **Backplane** (Redis 등) 또는 Azure SignalR Service |
| Q14 | `User`는 해당 사용자의 **모든 연결**(여러 탭/기기), `Client`는 **연결 하나** |
| Q15 | **DB에 저장 → SignalR로 알림 → 클라이언트가 재연결 시 REST로 놓친 데이터 재조회** (Catch-up 패턴) |

### "생각해 보기" 해설

| 단계 | 문항 | 해설 |
|---|---|---|
| 1 | 1 | `Others`로 보내면 서버가 나에게는 돌려주지 않으므로 **내 메시지가 내 화면에 나타나지 않는다.** 내 화면은 직접 그려야 한다 |
| 1 | 2 | 클라이언트 `on` 핸들러의 매개변수도 늘려야 새 값을 사용할 수 있다. 바꾸지 않으면 새 인자는 무시된다 |
| 1 | 3 | Hub는 호출마다 새 인스턴스라 `messageCount`가 매번 0에서 시작한다. `static` 또는 싱글톤 서비스가 필요하다 |
| 2 | 1 | 같은 그룹이 아니다. 그룹은 **Hub 단위**로 관리된다 |
| 2 | 2 | 연결은 정상으로 보이는데 방 메시지만 오지 않는 증상이 나타나 원인을 찾기 어렵다 |
| 2 | 3 | 아니다. 인증된 사용자인지, 그 방에 들어갈 권한이 있는지, 방 이름이 유효한지 검사해야 한다 |
| 3 | 1 | Hub 바깥에는 "메서드를 호출한 클라이언트"가 없기 때문이다 |
| 3 | 2 | `ClockWorker`는 평범한 클래스일 뿐이고, 등록해야 비로소 호스트가 인스턴스를 만들어 실행한다 |
| 3 | 3 | Hub는 클라이언트 요청을 받을 때만 실행된다. 주기적 저장은 별도 서비스(저장소 클래스)를 만들어 ClockWorker 등에서 호출한다 |
| 4 | 1 | 스트림은 **요청과 연결된 값의 흐름**으로 시작/완료/오류/취소가 명확하다. 단순 반복 `SendAsync`는 이런 구분과 취소 연동이 없다 |
| 4 | 2 | 클라이언트가 매우 큰 값을 보내 서버 자원을 오래 점유하지 못하도록 **입력을 검증**하기 위해서다 |
| 4 | 3 | 연결이 끊기면 `ct`가 취소되어 서버 루프가 종료된다 |
| 5 | 1 | `BadWordFilter`가 바깥쪽이 되어, 차단되면 안쪽의 `LoggingFilter`는 **실행되지 않아 로그가 남지 않는다** |
| 5 | 2 | 모든 Hub 메서드에 같은 검사를 복사해야 하고, 누락과 불일치가 생기기 쉽다 |
| 5 | 3 | 예외를 삼키면 정상 반환처럼 보여 **클라이언트는 실패를 알 수 없다** |
| 6 | 1 | `User`는 alice의 **모든 탭**에, `Client`는 지정한 **연결 하나**에만 전달된다 |
| 6 | 2 | `localStorage`는 새로고침 후에도 유지되어 편하지만 XSS로 탈취될 수 있다. 메모리 변수는 안전하지만 새로고침 시 사라진다. HttpOnly 쿠키가 대안이다 |
| 6 | 3 | 누가 어떤 호출을 했는지 추적(감사 로그)할 수 있다. 단, 개인정보 로깅 정책을 지켜야 한다 |
| 7 | 1 | 한 인스턴스에서만 워커를 켜는 설정 플래그, 분산 잠금, 별도 워커 서비스 분리 등으로 해결한다 |
| 7 | 2 | 전달된다. Redis backplane은 그룹 메시지도 서버 경계를 넘어 전달한다 |
| 7 | 3 | 충분하지 않다. DB 저장 + 알림 + 재연결 시 재조회(Catch-up)를 쓰고, 필요하면 메시지 큐를 함께 쓴다 |

---

## 부록 F. 과제 (선택)

| 수준 | 과제 | 사용 개념 |
|---|---|---|
| 기본 | 채팅 입장/퇴장 시 "OOO님이 입장했습니다" 시스템 메시지 표시 | `OnConnectedAsync`, `OnDisconnectedAsync`, `Context.Items` |
| 기본 | 현재 접속자 수를 헤더에 표시 | `static` 카운터(`Interlocked`), `Clients.All.SendAsync` |
| 기본 | `RoomHub`를 `index.html` 화면에 통합해 방별 채팅 만들기 | 2단계 + 1단계 UI |
| 중급 | `Counter` 스트림으로 진행률 바(progress bar) 만들기 | 4단계 |
| 중급 | 1초에 5회 이상 호출하면 차단하는 **호출 빈도 제한 필터** | 5단계, `ConcurrentDictionary` |
| 중급 | 금칙어 목록을 `appsettings.json`에서 읽는 `BadWordFilter` | 5단계, `IConfiguration` |
| 심화 | 로그인한 두 사용자 간 1:1 귓속말 | 6단계, `Clients.User` |
| 심화 | 채팅 내용을 DB에 저장하고 재연결 시 놓친 메시지를 REST로 가져오기 (Catch-up) | 2, 7단계 |
| 심화 | Redis backplane으로 서버 2대 구성 후 채팅이 서로 전달되는지 확인 | 7단계 |

---

## 부록 G. 맥/윈도우 협업 시 주의 (GitHub 공유 수업용)

| 항목 | 내용 |
|---|---|
| `.gitignore` | `bin/`, `obj/`, `.vs/`, `.idea/`, `.DS_Store`, `Thumbs.db`, 비밀 설정 파일은 커밋하지 않는다 |
| `.gitattributes` | `* text=auto` 및 소스 파일 `eol=lf`로 줄바꿈 통일 (맥 LF / 윈도우 CRLF 차이로 인한 전체 변경 방지) |
| 인코딩 | 모든 파일을 **UTF-8**로 저장 (한글 깨짐 방지). `.editorconfig`에 `charset = utf-8` 권장 |
| 파일명 대소문자 | 맥/윈도우는 구분하지 않지만 리눅스 서버는 구분한다. 대소문자만 다른 파일명을 만들지 않는다 |
| 프로젝트명/폴더명 | 공유 전에 철자를 확정한다. 협업 시작 후 이름을 바꾸면 충돌이 생긴다 |
| HTTPS 인증서 | 각자 PC에서 `dotnet dev-certs https --trust` 실행 |
| 포트 | 각자 다를 수 있으므로 문서에 포트를 고정하지 않고 터미널 출력 주소를 확인한다 |
| 비밀 값 | JWT 키 등은 `dotnet user-secrets` 사용. 저장소에 올리지 않는다 |
| 커밋 전 확인 | `git status`로 `bin/`, `obj/`가 목록에 없는지 확인한다 |

---

## 한 장 요약

| 질문 | 한 줄 답 |
|---|---|
| SignalR이란? | .NET의 실시간 양방향 통신 프레임워크 (연결·재연결·그룹·인증·확장 제공) |
| 호출 모델은? | 클라이언트 → 서버는 `invoke`, 서버 → 클라이언트는 `SendAsync` + `on` |
| Hub란? | 클라이언트가 호출할 수 있는 서버 메서드의 모음. 호출마다 새 인스턴스 |
| 메시지 대상 지정은? | `Clients.All / Caller / Others / Client / Group / User` |
| 그룹은 어디에 저장? | 서버 메모리. 재연결하면 사라지므로 클라이언트가 복구 |
| Hub 바깥에서 보내려면? | `IHubContext<THub>` |
| 서버가 먼저 보내려면? | `BackgroundService` + `IHubContext` |
| 값을 나눠 보내려면? | `IAsyncEnumerable` + `stream`, 업로드는 `Subject` |
| 공통 로직은? | `IHubFilter` + `AddFilter` |
| 로그인 사용자 처리? | `[Authorize]`, `UserIdentifier`, JWT는 쿼리스트링 `access_token` |
| 서버가 여러 대면? | Redis backplane + Sticky Session(또는 WebSocket 전용), 또는 Azure SignalR Service |
| 메시지는 보장되나? | 아니오. DB 저장 + Catch-up 패턴 |
