ParticleSystem
===

# Particle System

## 속성

- Duration : 지속 시간 ( 단위 : 초)
  
- Loop : 반복적으로 실행되도록 한다. False 라면 Duration 기간 후에 종료된다.

- Prewarm : 파티클을 미리 뿌려둔 상태에서 시작한다. 만약 먼지나 눈 처럼 이미 내려져 있는 상태를 두고 싶을 때 사용한다.

- Start Delay : Start Delay 시간 이후에 파티클이 시작된다. PreWarm이 True라면 비활성화 된다. 이미 파티클이 놓여 잇는 상태이기 때문에 Start Delay가 의미가 없기 떄문이다.

- Start LifeTime : 파티클의 생존시간이다. ▼ 버튼을 통해 정적인 시간이외에 다양한 방식으로 지정할 수 있다.

    - curve : Duration 동안 LifeTime이 점점 증가하거나 감소시킬 수 있다.
    - random between : 시간의 범위 동안 혹은 두 커브 범위 안에서 LifeTime을 조절할 수 있다.

- Start Speed : 파티클의 속도

- Start Size : 파티클의 크기이다. 우측의 ▼ 버튼을 통해 파티클의 크기를 랜덤적으로 다양하게 바꿀 수 있다.

    - curve : Duration 동안 Size가 점점 증가하거나 감소시킬 수 있다.
    - random between : 생성 될 때 마다 두 수 a와 b중 랜덤으로 크기가 정해진다.

- Start Rotation : 파티클의 각도이다. 우측의 ▼ 버튼을 통해 파티클의 각도를 랜덤적으로 다양하게 바꿀 수 있다.

    - curve : Duration 동안 각도가 점점 증가하거나 감소시킬 수 있다.
    - random between : 생성 될 때 마다 두 수 a와 b중 랜덤으로 각도가 정해진다.

- Filp Rotation : 파티클의 각도를 반대로 지정하는 것으로 0 ~ 1 사이의 값을 가진다. 
  
    > 근데 잘 안써요 그냥 각도를 -(부호) 붙이는게 좋은거 같다.

- Start Color : 파티클의 색상이다. 우측의 ▼ 버튼을 통해 파티클의 색상를 랜덤적으로 다양하게 바꿀 수 있다.

    - Color : 단색
    - Gradient : 색이 점점 연해진다.
    - Random Between Two Color : 두 색상중 랜덤으로 나온다.
    - Random Between Two Gradients : 두 색상 중 랜덤으로 나오며 점점 연해진다.
    - Random Color : 단색에서 다양한 밝기로 나온다.

- Gravity Modifier : 파티클에 중력을 적용하는 것으로 중력의 방향은 양수 기준이다. 무엇인가 폭발할 때 중력의 영향을 받지 않고 일직선으로 나가지 않듯이 자연스러운 효과를 적용할 수 있다. 

- Simulation Space : Particle System 오브젝트의 좌표계 기준을 설정하는 것으로 Local은 자신을 기준으로 World는 Global로 좌표계가 항상 고정되있다. Local은 위치가 이동하면 파티클도 다같이 이동하는 반면 World는 위치가 이동해도 각자 독립적으로 날아가므로 발자국 같은 효과를 적용할 때 많이 사용된다.

- Simulation Speed : 파티클의 절대적인 시간 배수로 1은 정상적인 프레임을 나타내고 점차 낮아질수록 슬로우 모션 같은 효과가 나온다.

- Delta Time : 게임을 일시정지할 때 Scaled라면 게임의 상태와 동일하게 정지하지만 UnScaled라면 게임의 상태와 무관하게 게속 파티클이 생성된다.

- Scaling Mode : 스케일을 어떻게 관리할 것인지 나타낸다. Particle System의 Transform 컴포넌트를 통해서도 스케일을 관리할 수 있지만, Scaling Mode를 통해 더 자세하게 관리할 수 있다. 

    - Hiarachy : 부모에 있는 Particle System이 부모의 스케일링에 따라 변한다.

    - Local : 부모의 스케일이 변해도 Particle System의 스케일은 변하지 않는다.

    - Shape : Particle System의 스케일이 커져도 파티클의 크기가 커지지 않는다.

    > 만약 Particle을 만들어서 다양한 형태로 사용하고 싶다면 Local로 적용할 시 다양한 스케일로 사용할 수 없기 때문에 Local로 해야된다는 점을 유의하자!!

- Play On Awake : 시작시 파티클이 생성된다.

- Emitter Velocity : 건들지 않아도 되요~

- Max Particles : 최대 생성 갯수를 정한다. 

    > 최적화와 관련된 부분으로 중요하다.

- Auto Random Seed : True면 파티클이 재 생성될 때 마다 생성되는 위치가 다르다. 항상 동일하게 파티클이 생성되야 된다면 False로 한다.

- Stop Action : 파티클 종료시 취할 액션을 뜻한다.

- Culling Mode : 카메라가 파티클을 비추지 않고 있을 때 어떻게 할지 정한다. 

    > 최적화와 관련된 부분으로 중요하다.
  
# Emission

## 속성

- Rate over Time : 초당 생성되는 파티클의 수이다.

- Rate over Distance : Particle System이 움직일 때만 파티클이 생성된다. 이때는 Simulation Space를 World로 해야된다.

- Bursts : 익스플로젼 !! 한 순간에 생성할 파티클을 지정할 수 있다. 여러 시간대에 다양한 수 의 파티클을 지정할 수 있다.

# Shape

## 속성

- Shape : 파티클이 방출되는 도형의 형태를 정할 수 있다.

    - Mesh : Mesh의 모양에 맞춰서 생성된다.

    - Mesh Renderer : 캐릭터로 만들어논 프리팹에 있는 Renderer에 파티클을 생성하는 것으로 불을 붙일 때 사용할 수 있다.

- Texture : 텍스쳐(예. 지형, 땅, 벽)의 색상에 랜덤으로 파티클이 생성된다.

- Randomize Position : 처음 생성될 포지션을 랜덤하게 지정할 수 있다. 

    > 이 것을 통해 게임에서 보이는 미세 먼지나 입자들 효과를 줄 수 있다.

# Inherit Velocity

## 속성

- Mode : Initial

- Multiplier : 1로 설정하고 파티클의 속도를 0으로 지정하면 Particle System이 움직일 때 움직인 방향으로 파티클이 퍼져나간다. 

    > 자동차가 드리프트 할 때 먼지 효과를 줄 수 있어요!!

# Force Over LifeTime

각 파티클이 점점 빠르게 움직인다.

## 속성

# Color Over LifeTime

각 파티클의 색상이 변해간다. 

> 불이 연소할 때 점점 색상이 검어지는 효과를 사용할 수 있어요!!


