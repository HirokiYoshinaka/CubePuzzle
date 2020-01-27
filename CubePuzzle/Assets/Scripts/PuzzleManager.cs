using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パズルのデータ、ロジックの管理クラス
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    static readonly int cubeLength = 3;
    // 6面
    enum SixSides
    {
        Top = 0, Under, Right, Left, Front, Back, MAX
    }
    // 6色
    enum SixColors
    {
        White, Orange, Red, Green, Yellow, Blue, MAX
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
