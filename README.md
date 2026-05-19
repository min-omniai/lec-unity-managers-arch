# Managers Architecture

> Unity 6 기반 게임 클라이언트 아키텍처 학습용 베이스 구조

---

## 소개

이 레포지토리는 Unity 프로젝트가 커질 때 발생하는 구조적 문제를 해결하기 위한  
**Managers 아키텍처 패턴**을 학습하기 위한 특강용 베이스 구조입니다.

"왜 이런 구조가 필요한가?"를 이해하는 것이 목표입니다.

---

## 아키텍처 구조

```
@Managers (GameObject, DontDestroyOnLoad)
└── Managers.cs  ← 단일 진입점 싱글톤

SubManagers (Plain C# Class, ISubManager)
├── ResourceManager  ← 리소스 로드 / Instantiate / Destroy 래핑
├── PoolManager      ← 오브젝트 풀링, 재사용 객체 관리
├── AudioManager     ← BGM / SFX 재생 및 정지
├── DataManager      ← 게임 데이터 저장 / 로드
└── UIManager        ← UI 생성, Popup 스택 관리
```

**외부에서는 항상 이렇게 접근합니다.**

```csharp
Managers.Audio.Play("Sounds/Attack");
Managers.UI.ShowPopupUI<UI_GameOver>();
Managers.Resource.Instantiate("Prefabs/Monster");
Managers.Pool.Pop(monsterPrefab);
```

---

## 프로젝트 구조

```
Assets/
├── Scenes/
│   └── TestScene.unity
├── Resources/
│   ├── EventSystem.prefab
│   └── UI/
│       ├── UI_TestScene.prefab
│       └── UI_TestPopup.prefab
└── Scripts/
    ├── Core/
    │   ├── Define.cs
    │   ├── ISubManager.cs
    │   └── Managers.cs
    ├── Managers/
    │   ├── AudioManager.cs
    │   ├── DataManager.cs
    │   ├── PoolManager.cs
    │   ├── ResourceManager.cs
    │   └── UIManager.cs
    ├── Pool/
    │   └── Poolable.cs
    ├── Scenes/
    │   ├── BaseScene.cs
    │   └── GameScene.cs
    └── UI/
        ├── UI_Base.cs
        ├── UI_Popup.cs
        ├── UI_Scene.cs
        └── Test/
            ├── UI_TestPopup.cs
            ├── UI_TestScene.cs
            └── UITest.cs
```

---

## 브랜치 안내

| 브랜치 | 설명 |
|--------|------|
| `main` | 완성된 구조, 바로 실행 가능 |
| ~~`starter`~~ | ~~SubManager 구현이 비워진 버전 — 직접 구현 학습용~~ |

~~학습자는 `starter` 브랜치를 Fork 떠서 직접 SubManager를 구현해보는 것을 권장합니다.~~

```
1. 우측 상단 Fork 버튼 클릭
2. ❌starter 브랜치로 전환
3. ❌TODO 주석을 따라 SubManager 구현
4. main 브랜치와 비교하며 확인
```

---

## 시작하기

**요구사항**
- Unity 6 이상
- Universal Render Pipeline (URP)

**실행 방법**
```
1. 레포지토리 Clone 또는 Fork
2. Unity 6에서 프로젝트 열기
3. TestScene 실행
4. 키 입력으로 테스트
   - 1 키: Popup UI 열기
   - 2 키: Popup UI 닫기
```

---

## 학습 가이드

이 구조를 이해하기 위한 선수지식 학습 순서입니다.

| 순서 | 주제 |
|------|------|
| 01 | [메모리 영역 이해하기 (static / Stack / Heap / GC)](https://share.note.sx/tjkc263t#AsFsPSIkm2puyLz64KkO1QxRUKnTW0AqVDlXS4AtBkg) |
| 02 | [Object와 Instance (class / new / 참조)](https://share.note.sx/2k0rgz9r#g4pq0eQPbKeSAJXqtI3xDoC0ODXYt/hKaDGUnswbPnU) |
| 03 | [MonoBehaviour와 Unity 생명주기](https://share.note.sx/u1r1s0f8#OZkmRTF2CdnHg/KPbptvA1/0MeOBf7Y1o70Atv4yhIE) |
| 04 | [static과 Singleton](https://share.note.sx/ciy3jueo#jLqi91WM9zQpk7o4Kc6hK08GoiTmxC9eB193fje5RXw) |
| 05 | [Managers 구조와 책임 분리](https://share.note.sx/h2fekr0r#Y4ihpy58Dsi5E+/FebScv93FvpUXF//9sb7g1hvTksM) |

---

## 이 구조의 한계

이 구조는 정답이 아닙니다. 프로젝트가 커질수록 아래 한계가 드러납니다.

- 모든 코드가 `Managers`를 직접 참조 → 전역 의존성 증가
- 초기화 순서가 꼬이면 NullReferenceException 발생
- God Object 문제: Managers가 너무 많은 것을 알게 됨

**다음 단계로 나아가기 위한 키워드:**

| 키워드 | 해결하는 문제 |
|--------|---------------|
| `Addressables` | Resources.Load 한계 극복 |
| `UniTask` | Coroutine 복잡성 해결 |
| `VContainer` | 전역 의존성 / God Object 해결 |
| `R3` | 이벤트 / 데이터 흐름 개선 |

---

## 라이선스

MIT License — 자유롭게 활용하세요.
