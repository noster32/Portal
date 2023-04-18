using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
◆ partial

- partial 클래스는 C# 2.0? 3.0? 도입된 기능
ㄴ 클래스를 여러 파일에 정의할 수 있다

- 클래스가 대상이 되기도 하지만 기본저긍로 구조체, 인터페이스에도 사용이 가능

- 논리적으로 하나다 (분리 이후 실행될 때 합쳐진다)


*/

//! 전역 상수
public static partial class KDefine
{
    #region 해상도 처리

    public static readonly int SCREEN_WIDTH = 1280;
    public static readonly int SCREEN_HEIGHT = 720;

    public static readonly float UNIT_SCALE = 0.01F;

    #endregion
}