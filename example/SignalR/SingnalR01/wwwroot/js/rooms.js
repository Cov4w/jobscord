// 이 페이지 전용 연결. index.html의 connection과 이름이 겹치지 않게 roomConnection 사용
const roomConnection = new signalR.HubConnectionBuilder()
  .withUrl("/roomHub")
  .withAutomaticReconnect()
  .build();

// 내가 가입한 방 목록 (재연결하면 서버가 그룹 소속을 잊기 때문에 클라이언트가 기억)
const joinedRooms = new Set();

// 서버 → 클라이언트 핸들러는 start() 전에 등록
roomConnection.on("System", (text) => console.log("[시스템]", text));
roomConnection.on("RoomMessage", (room, message) => console.log(`[${room}]`, message));

roomConnection.onreconnecting(() => console.log("재연결 중..."));

roomConnection.onreconnected(async () => {
  console.log("재연결 성공 → 방 재가입");
  for (const room of joinedRooms) {
    await roomConnection.invoke("JoinRoom", room);
  }
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