# Noosphere
<img width="460" height="215" alt="Image" src="https://github.com/user-attachments/assets/c44f2053-b80f-457c-ace1-a4ac2fbc24d2" />
<br>타인의 정신세계에서 단서를 조사하고 사건의 진실을 찾아가는 3인칭 3D 추리 어드벤처 게임입니다.

## 프로젝트 개요

| 항목 | 내용 |
| --- | --- |
| 엔진 | Unity 2022.3.50f1 |
| 언어 | C# |
| 플랫폼 | PC (Windows / macOS) |
| 장르 / 분야 | 3인칭 추리 어드벤처 |
| 기간 | 2024.10 ~ 2025.12 |
| 인원 | 6명 (기획 2 / 개발 2 / 아트 1 / 사운드 1) |
| 담당 역할 | 클라이언트 프로그래밍 |
| 성과 | 2025 인디크래프트 챌린저 부문 TOP 2<br>2025.12.22 Steam 정식 출시 |

## 주요 담당 업무

- 데이터 테이블 기반 이벤트 실행과 진행 상태 관리 시스템 설계·구현
- 상호작용 대상 판정, 단서 조사 및 인벤토리 획득·사용 흐름 구현
- 슬롯 저장·이어하기·자동 저장 기능 구현
- 언어 선택 UI와 다국어 설정 저장 기능 구현

## 주요 구현 기능

### 데이터 기반 이벤트 진행
<img width="550" height="300" alt="Image" src="https://github.com/user-attachments/assets/cf085a47-24ac-4928-88d9-cf1d23ef2f60" />
<br>이벤트 ID를 기준으로 실행 가능 여부와 조건을 검사하고, 대화·연출·조사 결과와 후속 이벤트를 연결했습니다. CSV에서 읽은 이벤트 데이터와 진행 상태를 함께 관리하여 반복 실행과 분기 처리를 구성했습니다.

[관련 코드](https://github.com/yoongang02/Noosphere/blob/6fc985b1e9e30a234505e183f0ce743853aa72b2/Assets/02.Scripts/Manager/EventManagerYKM.cs)

### 상호작용과 단서 조사
<img width="400" height="200" alt="Image" src="https://github.com/user-attachments/assets/47b176ed-94d9-4087-b97c-473eba2074ec" />
<br><img width="550" height="300" alt="Image" src="https://github.com/user-attachments/assets/0bac135b-0709-49b7-a5c6-bcacad54c7cc" />
<br>트리거 진입과 플레이어가 바라보는 방향을 함께 고려해 상호작용 대상을 판정했습니다. 대상의 이벤트를 조사 UI로 연결하고, 조사 종료 후 단서 획득과 다음 진행으로 이어지도록 구현했습니다.

[관련 코드](https://github.com/yoongang02/Noosphere/blob/6fc985b1e9e30a234505e183f0ce743853aa72b2/Assets/02.Scripts/Trigger/EventTrigger.cs)

### 챕터별 인벤토리
<img width="550" height="300" alt="Image" src="https://github.com/user-attachments/assets/441a4c3e-f349-4da9-ab03-44003388c174" />
<br>획득한 단서를 챕터별로 보관하고 목록·상세 보기·선택 이동을 연결했습니다. 단서의 사용 가능 여부와 보유 상태를 이벤트 시스템에서 참조하도록 구성했습니다.

[관련 코드](https://github.com/yoongang02/Noosphere/blob/6fc985b1e9e30a234505e183f0ce743853aa72b2/Assets/02.Scripts/Manager/InventoryManager.cs)

### 저장·이어하기와 언어 설정
<img width="550" height="300" alt="Image" src="https://github.com/user-attachments/assets/2e6f22a7-2836-4684-a39f-43699b62cbaa" />
<br>Easy Save로 이벤트 상태, 인벤토리, 플레이어 위치와 현재 씬을 저장하고 이어하기 시 복원했습니다. 수동·자동 저장 UI를 연결하고, Unity Localization의 언어 선택 결과를 저장해 다시 적용하도록 구현했습니다.

[관련 코드](https://github.com/yoongang02/Noosphere/blob/6fc985b1e9e30a234505e183f0ce743853aa72b2/Assets/02.Scripts/Save%20And%20Load/SaveManager.cs)

## 프로젝트 구조

담당 코드가 포함된 핵심 경로입니다.

```text
Assets/02.Scripts/
├── Manager/          # 이벤트·데이터·인벤토리·언어 설정
├── Structures/       # 이벤트·단서·실행 조건 데이터
├── Trigger/          # 상호작용 트리거
├── Player/           # PlayerInteract.cs: 상호작용 연결
├── UI/               # 조사·단서 상세·인벤토리 UI
└── Save And Load/    # 저장 슬롯·자동 저장·이어하기
```

## 관련 링크

- [시연 영상](https://youtu.be/iSn81KbhYRw)
- [스팀 페이지](https://store.steampowered.com/app/3618090/Noosphere/)
