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
    // 色定義
    readonly Color[] colorPattern = new Color[6]
    {
        //        R     G     B
        new Color(1.0f, 1.0f, 1.0f),   // White
        new Color(1.0f, 0.5f, 0.1f),   // Orange
        new Color(1.0f, 0.0f, 0.0f),   // Red
        new Color(0.0f, 1.0f, 0.0f),   // Green
        new Color(1.0f, 1.0f, 0.0f),   // Yellow
        new Color(0.0f, 0.0f, 1.0f),   // Blue
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
