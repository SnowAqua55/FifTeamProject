# FifTeeam Project

## 프로젝트 이름
2D 보스러시 게임 (게임 이름 추후 제작)
유니티 심화 팀 프로젝트 과제    
## 👨‍🏫 프로젝트 소개
유니티 2D 활용하여 제작된 2D 보스러쉬 게임입니다. 심화 주차 강의 내용인 Cinemachine, FSM 디자인 패턴을 연습하기위해 제작하였습니다.  <br/>
플레이어를 조작하여 스테이지마다 등장하는 보스들을 처치하여 다음 스테이지로 나아가야합니다.
보스는 총 5개의 보스가 등장하며 플레이어 사망 및 최종 보스 클리어 시 게임이 종료됩니다.


## ✔ 조작 방법
   오른쪽 방향키, D : 플레이어 우측으로 이동 <br/>
   왼쪽 방향키, A : 플레이어 왼쪽으로 이동 <br/>
   윗방향 키 : 플레이어 점프 <br/>
   X : 플레이어 공격 <br/>
   Z : 플레이어 회피 <br/>



## 💜 주요기능

- 기능 1   <br/>
  FSM 디자인 패턴을 활용한 보스 구조   <br/>
- 기능 2   <br/>
  Cinemachine등의 유니티 컴포넌트 사용  <br/>
- 기능 3   <br/>
  Rigidbody를 활용한 플레이어 이동 구현  <br/>
- 기능 4   <br/>
  RayCast를 사용한 플레이어 지면 판정
- 기능 4
  다양한 매니저를 구현하여 유지보수 용이 및 일관성 유지 
## ⏲️ 개발기간
- 2025.06.12(목) ~ 2025.06.08(수)


## 📚️ 기술스택

Unity 2022.3.17f1 <br/>
C# <br/>
Git <br/>
Visual Studio <br/>


## ✔ 게임 흐름
타이틀 씬 -> 입장 스테이지 -> 보스 스테이지 -> 보스 격파 -> 다음 스테이지 -> 최종층 클리어 및 플레이어 사망 시 게임 종료
