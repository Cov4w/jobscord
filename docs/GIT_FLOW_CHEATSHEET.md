# Git Flow 실전 명령어 치트시트

Discord Clone 프로젝트(ASP.NET Core + SignalR + WPF MVVM)의 원활한 협업을 위한 Git Flow 작업 가이드입니다.

---

## 0. 브랜치 전략 및 네이밍 규칙

| 브랜치 | 기준 / 머지 대상 | 설명 및 예시 |
| :--- | :--- | :--- |
| `main` | - | 최종 배포 및 시연용 안정 버전 (**직접 푸시 금지**) |
| `develop` | `main` | **기본(Default) 브랜치.** 모든 기능이 통합되는 개발 기준점 (**직접 푸시 금지**) |
| `feat/server-*` | `develop` → `develop` | 백엔드 기능 개발 (`feat/server-signalr-hub`, `feat/server-stt`) |
| `feat/client-*` | `develop` → `develop` | WPF UI / MVVM 기능 개발 (`feat/client-chat-ui`, `feat/client-login`) |
| `feat/shared-*` | `develop` → `develop` | 공통 DTO / 모델 수정 (`feat/shared-message-dto`) |
| `fix/*` | `develop` → `develop` | 개발 중 발견된 버그 수정 (`fix/client-scroll-bug`) |

---

## 1. 최초 프로젝트 세팅 (최초 1회)

레포지토리를 클론하면 기본 브랜치(`develop`)로 설정되어 있습니다.

```bash
# 레포지토리 클론
git clone <레포지토리_URL>
cd <프로젝트_폴더명>

# 현재 브랜치가 develop인지 확인
git branch
```

---

## 2. 기능 개발 사이클 (매 작업마다 반복)

### ① `develop` 최신화 및 작업 브랜치 생성
새 작업을 시작하기 전에는 **반드시 `develop`을 최신 상태로 받은 뒤** 브랜치를 생성합니다.

```bash
# 1. develop 브랜치로 이동
git switch develop

# 2. 원격 develop의 최신 변경사항 가져오기
git pull origin develop

# 3. 내 작업 브랜치 생성 및 이동
git switch -c feat/server-기능명
```

### ② 작업 및 커밋
작업 단위가 끝날 때마다 의미 있는 단위로 커밋을 남깁니다.

```bash
# 변경된 파일 확인
git status

# 변경된 파일 스테이징 (전체 추가 시 . 사용)
git add .

# 커밋 메시지 작성
git commit -m "feat: SignalR 채팅 허브 구현"
```

### ③ 원격 레포지토리로 푸시

```bash
# 원격에 내 브랜치 올리기 (최초 1회는 -u 옵션 사용)
git push -u origin feat/server-기능명

# 이후 같은 브랜치에 추가 푸시할 때
git push
```

### ④ GitHub에서 PR(Pull Request) 생성
1. GitHub 레포지토리로 이동해 **Compare & pull request** 버튼 클릭
2. 머지 방향 확인: **`base: develop` ← `compare: feat/server-기능명`**
3. 변경 사항 및 테스트 내용 작성 후 리뷰어 지정 → PR 생성

---

## 3. 머지 완료 후 뒷정리

GitHub에서 PR이 `develop`에 머지되면(원격 브랜치는 자동 삭제 설정 기준), 로컬에 남은 브랜치도 깔끔하게 정리합니다.

```bash
# 1. develop 브랜치로 이동
git switch develop

# 2. 머지된 최신 코드 반영
git pull origin develop

# 3. 작업이 끝난 로컬 브랜치 삭제
git branch -d feat/server-기능명

# (선택) 원격에서 삭제된 브랜치 목록 로컬에도 동기화
git fetch -p
```

---

## 4. 작업 중 `develop`이 업데이트되었을 때 (충돌 방지)

내 브랜치에서 작업하는 동안 팀원의 코드가 `develop`에 먼저 머지되어 최신 코드를 반영해야 할 때 사용합니다.

```bash
# 내 작업 브랜치(feat/...)에 있는 상태에서 실행
git fetch origin
git merge origin/develop

# 충돌(Conflict)이 발생하면 코드 수정 후 아래 명령어 실행
git add .
git commit -m "chore: develop 최신 코드 병합 및 충돌 해결"
git push
```

---

## 5. 팀 필수 규칙 (주의 사항)

- `main`과 `develop` 브랜치에는 **직접 `git commit` / `git push`를 금지**합니다.
- 하나의 브랜치에는 **하나의 기능(이슈)**만 작업하고, PR 단위는 리뷰하기 쉽게 작게 유지합니다.
- `Shared` 프로젝트의 공통 DTO나 모델을 수정한 경우, 머지 직후 팀원들에게 공유해 `git pull origin develop`을 먼저 받도록 안내합니다.