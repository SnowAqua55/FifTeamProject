public interface IBossState
{
    void Enter(BossBase boss);  // 상태 진입 시 실행: 변수 초기화, 애니메이션 등
    void Update();          // 매 프레임 실행: 상태 내부 로직
    void FixedUpdate(); // 물리 기반 이동
    void Exit();            // 상태 종료 시 실행: 정리 작업 등
}