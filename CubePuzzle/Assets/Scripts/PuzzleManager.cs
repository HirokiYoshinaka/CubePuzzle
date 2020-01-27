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

    // puzzle全体の親オブジェクト
    GameObject puzzle = null;
    // 描画に使用するCubeオブジェクトの配列(3*3*3)
    GameObject[,,] cubes = new GameObject[cubeLength, cubeLength, cubeLength];
    // 実際のデータ保持配列(6面*3*3)
    SixColors[,,] dataTable = new SixColors[(int)SixColors.MAX, cubeLength, cubeLength];

    /// <summary>
    /// データ表示に使用するオブジェクトの生成
    /// </summary>
    void CreateCube()
    {
        // パズル全体の親となる空オブジェクトを生成
        puzzle = new GameObject("Puzzle");
        // 自身の子とする
        puzzle.transform.parent = transform;
        GameObject cubePrefab = Resources.Load("Prefabs/Cube") as GameObject;

        for (int x = 0; x < cubeLength; x++)
        {
            for (int y = 0; y < cubeLength; y++)
            {
                for (int z = 0; z < cubeLength; z++)
                {
                    // Cubeを生成して
                    var cube = Instantiate(cubePrefab);
                    // 位置決め
                    cube.transform.position = new Vector3(x, y, z);
                    // 配列に追加
                    cubes[x, y, z] = cube;
                    // puzzleの子とする
                    cube.transform.parent = puzzle.transform;
                    // 名前はデバッグが楽になりそうというだけの理由
                    cube.name = x.ToString() + "," + y.ToString() + "," + z.ToString();
                }
            }
        }
    }

    /// <summary>
    /// ルービックキューブデータの初期化
    /// </summary>
    void InitCubeData()
    {
        for (int side = 0; side < (int)SixSides.MAX; side++)
        {
            for (int i = 0; i < cubeLength; i++)
            {
                for (int j = 0; j < cubeLength; j++)
                {
                    dataTable[side, i, j] = (SixColors)side;
                }
            }
        }
    }

    /// <summary>
    /// dataTableを描画に反映します
    /// </summary>
    void DrawCube()
    {
        for (int side = 0; side < (int)SixSides.MAX; side++)
        {
            for (int i = 0; i < cubeLength; i++)
            {
                for (int j = 0; j < cubeLength; j++)
                {
                    // dataTableからカラーパターンに変換
                    var color = colorPattern[(int)dataTable[side, i, j]];
                    // どの面に描画するか
                    GameObject renderTarget = null;
                    switch ((SixSides)side)
                    {
                        case SixSides.Top:
                            renderTarget = cubes[i, 2, j].transform.Find("Top").gameObject;
                            break;
                        case SixSides.Under:
                            renderTarget = cubes[i, 0, j].transform.Find("Bottom").gameObject;
                            break;
                        case SixSides.Right:
                            renderTarget = cubes[2, i, j].transform.Find("Right").gameObject;
                            break;
                        case SixSides.Left:
                            renderTarget = cubes[0, i, j].transform.Find("Left").gameObject;
                            break;
                        case SixSides.Front:
                            renderTarget = cubes[i, j, 0].transform.Find("Front").gameObject;
                            break;
                        case SixSides.Back:
                            renderTarget = cubes[i, j, 2].transform.Find("Back").gameObject;
                            break;
                    }
                    renderTarget.GetComponent<Renderer>().material.color = color;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateCube();
        InitCubeData();
        DrawCube();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
