// 이 페이지 전용 연결. 다른 파일의 connection과 이름이 겹치지 않게 streamConnection 사용
const streamConnection = new signalR.HubConnectionBuilder()
  .withUrl("/streamHub")
  .withAutomaticReconnect()
  .build();

// 화면 로그 출력 (stream.html의 #log 영역)
function log(text) {
  const el = document.getElementById("log");
  if (!el) return console.log(text);
  el.textContent += text + "\n";      // textContent: XSS 방지
  el.scrollTop = el.scrollHeight;
}

// 서버가 Upload를 마친 뒤 호출하는 핸들러 (start() 전에 등록)
streamConnection.on("UploadDone", (count) => log(`[업로드 완료] 서버가 ${count}개 조각을 받음`));

// ───── 서버 → 클라이언트 스트리밍 ─────
let currentSub = null;   // 현재 구독. 취소하려면 이 객체가 필요하다.

function startCounter(count = 10, delayMs = 500) {
  stopCounter();                       // 이전 구독이 있으면 먼저 정리
  log(`--- 카운터 시작 (${count}개, ${delayMs}ms 간격) ---`);

  // stream(서버메서드명, ...인자) → subscribe로 값이 올 때마다 처리
  currentSub = streamConnection.stream("Counter", count, delayMs).subscribe({
    next:     (n) => log(`값: ${n}`),          // 값이 하나 도착할 때마다
    complete: ()  => { log("--- 완료 ---"); currentSub = null; },   // 서버가 끝냈을 때
    error:    (e) => { log("오류: " + e);     currentSub = null; }  // 오류 또는 연결 끊김
  });
}

function stopCounter() {
  if (currentSub) {
    currentSub.dispose();              // 서버의 CancellationToken이 취소된다.
    currentSub = null;
    log("--- 사용자가 취소 ---");
  }
}

// ───── 클라이언트 → 서버 스트리밍 ─────
async function startUpload(chunks = ["조각-1", "조각-2", "조각-3", "조각-4"]) {
  const subject = new signalR.Subject();     // 보낼 값을 밀어 넣는 통로

  // 서버 메서드 호출과 동시에 스트림이 열린다. 서버 메서드가 끝나야 promise가 완료되므로
  // await 하지 않고 일단 받아 둔다. (여기서 await 하면 아래 next()가 실행되지 않는다)
  const done = streamConnection.invoke("Upload", subject);

  log("--- 업로드 시작 ---");
  for (const chunk of chunks) {
    subject.next(chunk);                     // 조각 하나 전송
    log(`전송: ${chunk}`);
    await new Promise(r => setTimeout(r, 400));   // 눈으로 보이도록 간격을 둠
  }

  subject.complete();                        // "더 보낼 게 없음" → 서버의 await foreach가 끝남
  await done;                                // 서버 메서드 종료까지 대기
  log("--- 업로드 종료 ---");
}

// 연결 상태 로그
streamConnection.onreconnecting(() => log("재연결 중... (진행 중이던 스트림은 끊깁니다)"));
streamConnection.onreconnected(()  => log("재연결됨"));
streamConnection.onclose(()        => log("연결 종료"));

streamConnection.start()
  .then(() => {
    log("streamHub 연결됨");
    document.querySelectorAll("button").forEach(b => b.disabled = false);
  })
  .catch(err => log("연결 실패: " + err));