# AntiCheat

[![Thunderstore Version](https://img.shields.io/thunderstore/v/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/versions/)  
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/)

[**简体中文(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/README.md) | [**English(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-en.md) | [**Русский**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-ru.md) | **한국어(Support)**

**리썰 컴퍼니**용 안티치트 (호스트 전용)
기본 언어는 중국어 간체입니다. 설정 파일에서 Language를 "한국어"로 변경하여 영어로 전환할 수 있습니다.

## 소개

- **모드 소통 QQ 그룹**: 263868521
- **Discord**: [https://discord.gg/ZdWr2rKR](https://discord.gg/zem2eFFBHj)

이 모드는 핵 유저로 인한 방 문제를 해결하고, 일반 방 참여자들의 게임 경험을 보장하기 위해 제작되었습니다.  
모드를 사용할 때 간혹 "오탐지" 문제가 발생할 수 있습니다. 하지만 이는 **안티치트** 모드 자체의 문제라고 단정할 수 없습니다.  
문제를 재현하기 위해 **(BepInExPack, LCKR, AntiCheat)** 외의 모든 모드를 삭제한 상태에서 테스트해 보세요.  
오탐지가 사라진다면, 이는 모드 간 호환성 문제일 가능성이 높습니다.

## 사용 방법

1. [BepInExPack](https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack) 모드를 설치합니다.
2. **AntiCheat.dll** 파일을 `Lethal Company\BepInEx\plugins\` 경로에 넣습니다.
3. 게임을 실행합니다.

## 방 생성

안티치트를 사용할 때, 방 이름에 **[AC]**를 추가하여 안티치트가 활성화된 방임을 다른 플레이어들에게 알리는 것을 권장합니다.  
예: **[v49/AC] 나는 미소녀**

## 주요 기능

### 탐지

- **강퇴 방지 탐지**  
  플레이어가 강퇴 방지 기능을 활성화한 상태로 게임 방에 접속했는지 확인합니다.

- **레버 관련 탐지**

  - **강제 레버 탐지**  
    새로 입장한 플레이어가 레버를 조작했는지 확인합니다.
  - **착륙 레버 탐지**  
    우주선이 착륙 후 플레이어의 레버 조작 여부를 탐지합니다. (설정 파일에서 조건 변경 가능)
  - **비행 레버 탐지**  
    우주선이 궤도에 있을 때 플레이어의 레버 조작 여부를 탐지합니다. (설정 파일에서 조건 변경 가능)

- **비정상 물품 위치 탐지**  
  우주선 물품의 좌표가 우주선 범위 내에 있는지 확인합니다.

- **아이템 사용 쿨타임 이상 탐지**  
  삽 사용 속도 및 샷건 발사 속도를 확인합니다.

- **빠른 조명 조작 탐지**  
  플레이어가 조명을 비정상적으로 빠르게 전환했는지 확인합니다.

- **터미널 소음 공예 탐지**  
  플레이어가 터미널에서 대량의 소음을 유발했는지 확인합니다.

- **아이템 파괴 탐지**  
  파괴 불가한 아이템이 파괴되었는지 확인합니다.

- **발언 조작 탐지**  
  다른 플레이어의 이름으로 발언을 시도했는지 확인합니다.

- **비정상적인 마스크 생성 탐지**  
  플레이어가 마스크를 사용해 가짜 캐릭터를 생성했는지 확인합니다.

- **비정상적인 선물 개봉 탐지**  
  플레이어의 선물 상자 개봉 횟수를 탐지합니다.

- **비정상적인 거미줄 생성 탐지**  
  플레이어가 거미를 소환해 다량의 거미줄을 생성했는지 확인합니다.

- **비정상적인 기관총 폭주 탐지**  
  플레이어가 멀리서 기관총을 폭주 상태로 만들었는지 확인합니다.

- **투명화 탐지**
  플레이어가 투명 상태인지 확인합니다.

- **비정상적인 제트팩 폭발 탐지**
  플레이어가 다른 플레이어의 제트팩을 폭발시켰는지 확인합니다.

- **비정상적인 지뢰 폭발 탐지**  
  플레이어가 지뢰를 비정상적인 방법으로 폭발시켰는지 확인합니다.

- **몬스터 조작 탐지**  
  플레이어가 몬스터를 조작하거나 몬스터 메커니즘을 변경했는지 확인합니다.

- **미니맵 모드 탐지**  
  플레이어가 미니맵 모드를 설치했는지 확인합니다.

- **적 처치 탐지**  
  플레이어가 먼 거리에서 적을 처치했는지 확인합니다.

- **원거리 아이템 습득 탐지**  
  플레이어가 먼 거리에서 아이템을 집었는지 확인합니다.

- **삽 데미지 이상 탐지**  
  플레이어가 삽 사용 시 비정상적인 데미지를 기록했는지 확인합니다.

- **이름 이상 탐지**  
  이름이 **Nameless** 또는 **Unknown**으로 표시된 플레이어의 이름을 탐지합니다.

- **공짜 구매 탐지**  
  아이템, 우주선 장식, 유료 행성 내비게이션을 공짜로 이용했는지 확인합니다.

- **무한 탄약 탐지**  
  샷건을 탄약없이 무한히 사용하는지 확인합니다.

- **보스 소환 탐지**  
  플레이어가 보스를 공격하는 함수를 호출했는지 확인합니다.

- **원거리 터미널 사용 탐지**  
  플레이어가 터미널을 원격으로 사용했는지 확인합니다.

- **몬스터로 인한 사망 탐지**  
  플레이어가 몬스터의 사망 함수를 호출했는지 확인합니다.

- **중복 접속 탐지**  
  이미 방에 있는 플레이어가 여러번 접속했는지 확인합니다.

- **속도 이상 탐지** (해결 예정)  
  플레이어가 비정상적인 속도로 이동했는지 확인합니다.

- **비행 탐지** (해결 예정)  
  플레이어가 비정상적으로 비행을 하는지 확인합니다.

### 기타 기능

- **방 이름 접두어**  
  기본값은 **AC**로 설정되어 있으며, 이를 통해 방이 안티치트 모드를 사용 중임을 알릴 수 있습니다. 설정에서 접두어를 변경하거나 삭제할 수 있습니다.

- **입장 알림 커스터마이즈**  
  기본 게임의 입장 알림이 혼란스러워 이를 대체했으며, 플레이어 입장 메시지를 설정에서 커스터마이즈할 수 있습니다.

## 기여

<a href="https://github.com/chuxiaaaa/AntiCheat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=chuxiaaaa/AntiCheat" />
</a>

### 지원 언어

- 게임 언어

  - 중국어: chuxiaaaa & CoolLKKPS
  - 영어: DeathWrench & CoolLKKPS & glyphical
  - 한국어: P-Asta

- 문서 언어
  - 중국어: chuxiaaaa & CoolLKKPS
  - 영어: NilaierMusic & CoolLKKPS
  - 러시아어: NilaierMusic
  - 한국어: P-Asta

### 모드 테스트 참여자

- 超级骇人鲸
- Alan Backer
- 沈阳最速傳說疾走の猛虎！貴物刀一郎
- 柒小鸭 yz
- 喜欢睡觉の极茶龙
- 东南枝
- Melissa
- 我不吃牛肉

유능한 개발자들의 기여를 언제든 환영합니다!

## 피드백

오탐지가 발생했지만 모드가 원인이 아님을 확인했다면, **이슈**를 보내서 문제를 재현할 방법을 공유해 주세요.  
이를 통해 문제 해결에 큰 도움이 됩니다!
