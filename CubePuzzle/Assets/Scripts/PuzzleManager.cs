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
        Top = 0, Bottom, Right, Left, Front, Back, MAX
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
    SixColors[,,] dataTable = new SixColors[(int)SixSides.MAX, cubeLength, cubeLength];
    // データ描画面のみを取り出しておく
    Renderer[,,] renderTargets = new Renderer[(int)SixSides.MAX, cubeLength, cubeLength];

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
            for (int y = 0; y < cubeLength; y++)
                for (int z = 0; z < cubeLength; z++)
                {
                    // Cubeを生成して
                    var cube = Instantiate(cubePrefab);
                    // 位置決め、中心(x,y,z)=(1,1,1)がnew Vector3(0,0,0)になる
                    cube.transform.position = new Vector3(x - 1, y - 1, z - 1);
                    // 配列に追加
                    cubes[x, y, z] = cube;
                    // puzzleの子とする
                    cube.transform.parent = puzzle.transform;
                    // 名前はデバッグが楽になりそうというだけの理由
                    cube.name = x.ToString() + "," + y.ToString() + "," + z.ToString();
                }
    }

    /// <summary>
    /// ルービックキューブデータの初期化
    /// </summary>
    void InitCubeData()
    {
        for (int side = 0; side < (int)SixSides.MAX; side++)
            for (int i = 0; i < cubeLength; i++)
                for (int j = 0; j < cubeLength; j++)
                    dataTable[side, i, j] = (SixColors)side;
    }

    /// <summary>
    /// dataTableを描画に反映します
    /// </summary>
    void InitRenderer()
    {
        for (int side = 0; side < (int)SixSides.MAX; side++)
            for (int i = 0; i < cubeLength; i++)
                for (int j = 0; j < cubeLength; j++)
                {
                    // dataTableからカラーパターンに変換
                    var color = colorPattern[(int)dataTable[side, i, j]];
                    // どの面に描画するか
                    GameObject renderTarget = null;
                    /// このように平面に展開したとき
                    ///        Top
                    /// Left | Front | Right | Back
                    ///        Bottom
                    /// 全ての面について 
                    ///   j
                    /// i＼0 1 2 
                    ///  0
                    ///  1
                    ///  2
                    /// となるように
                    switch ((SixSides)side)
                    {
                        case SixSides.Top:
                            renderTarget = cubes[j, 2, 2 - i].transform.Find("Top").gameObject;
                            break;
                        case SixSides.Bottom:
                            renderTarget = cubes[j, 0, i].transform.Find("Bottom").gameObject;
                            break;
                        case SixSides.Right:
                            renderTarget = cubes[2, 2 - i, j].transform.Find("Right").gameObject;
                            break;
                        case SixSides.Left:
                            renderTarget = cubes[0, 2 - i, 2 - j].transform.Find("Left").gameObject;
                            break;
                        case SixSides.Front:
                            renderTarget = cubes[j, 2 - i, 0].transform.Find("Front").gameObject;
                            break;
                        case SixSides.Back:
                            renderTarget = cubes[2 - j, 2 - i, 2].transform.Find("Back").gameObject;
                            break;
                    }
                    renderTargets[side, i, j] = renderTarget.GetComponent<Renderer>();
                    renderTargets[side, i, j].material.color = color;
                }
    }

    // dataTableに応じて再描画
    void DrawData()
    {
        for (int side = 0; side < (int)SixSides.MAX; side++)
            for (int i = 0; i < cubeLength; i++)
                for (int j = 0; j < cubeLength; j++)
                    renderTargets[side, i, j].material.color
                        = colorPattern[(int)dataTable[side, i, j]];
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateCube();
        InitCubeData();
        InitRenderer();
        // オブジェクト回転初期化の呼び出し
        GetComponent<ObjectRotation>().InitRotation();
        DrawData();
    }

    /// <summary>
    /// 回転の中心軸と方向を指定します
    /// </summary>
    public enum RotationType
    {
        // SideDirectionで命名
        TopRight,           // 上面右回転
        TopLeft,            // 上面左回転
        BottomRight,        // 底面右回転
        BottomLeft,         // 底面左回転
        RightRight,         // 右面右回転
        RightLeft,          // 右面左回転
        LeftRight,          // 左面右回転
        LeftLeft,           // 左面左回転
        FrontRight,         // 前面右回転
        FrontLeft,          // 前面左回転
        BackRight,          // 背面右回転
        BackLeft,           // 背面左回転

        /// <summary>
        /// 中心を軸としたものは６通り
        ///  ／T＼
        /// |＼ ／|
        /// |L | F|
        ///  ＼|／
        /// と置いて見たときに
        /// </summary>
        CenterRightForward, // ↘   手前に右回転
        CenterLeftBack,     // ↖   奥側に左回転（RFの逆
        CenterLeftForward,  // ↙   手前に左回転
        CenterRightBack,    // ↗   奥側に右回転（LFの逆
        CenterRightSlice,   // →  平行に右回転
        CenterLeftSlice,    // ←  平行に左回転（RSの逆
    }

    /// <summary>
    /// Cubeの回転を行います
    /// </summary>
    /// <param name="rotationType">回転軸と方向の指定</param>
    void Rotation(RotationType rotationType)
    {
        // テーブルをディープコピー（と言っても一度intにキャストしてenumにキャストし直すだけでOK
        // ２回キャストでの値コピーはうっかり冗長キャストとして修正する人がいそうで少し怖い、他の方法探したほうが良いかも？
        SixColors[,,] workTable = new SixColors[(int)SixSides.MAX, cubeLength, cubeLength];
        for (int side = 0; side < (int)SixSides.MAX; side++)
            for (int i = 0; i < cubeLength; i++)
                for (int j = 0; j < cubeLength; j++)
                    workTable[side, i, j] = (SixColors)(int)dataTable[side, i, j];
        switch (rotationType)
        {
            // {.... break;}の形に統一したのは単にアウトラインの折りたたみを使いたいだけです
            case RotationType.TopRight:
                {
                    // Top面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Top, i, j] = dataTable[(int)SixSides.Top, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Left, 0, i] = dataTable[(int)SixSides.Front, 0, i];
                        workTable[(int)SixSides.Front, 0, i] = dataTable[(int)SixSides.Right, 0, i];
                        workTable[(int)SixSides.Right, 0, i] = dataTable[(int)SixSides.Back, 0, i];
                        workTable[(int)SixSides.Back, 0, i] = dataTable[(int)SixSides.Left, 0, i];
                    }
                    break;
                }
            case RotationType.TopLeft:
                {
                    // Top面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Top, i, j] = dataTable[(int)SixSides.Top, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Front, 0, i] = dataTable[(int)SixSides.Left, 0, i];
                        workTable[(int)SixSides.Right, 0, i] = dataTable[(int)SixSides.Front, 0, i];
                        workTable[(int)SixSides.Back, 0, i] = dataTable[(int)SixSides.Right, 0, i];
                        workTable[(int)SixSides.Left, 0, i] = dataTable[(int)SixSides.Back, 0, i];
                    }
                    break;
                }
            case RotationType.BottomRight:
                {
                    // Bottom面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Bottom, i, j] = dataTable[(int)SixSides.Bottom, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Left, 2, i] = dataTable[(int)SixSides.Front, 2, i];
                        workTable[(int)SixSides.Front, 2, i] = dataTable[(int)SixSides.Right, 2, i];
                        workTable[(int)SixSides.Right, 2, i] = dataTable[(int)SixSides.Back, 2, i];
                        workTable[(int)SixSides.Back, 2, i] = dataTable[(int)SixSides.Left, 2, i];
                    }
                    break;
                }
            case RotationType.BottomLeft:
                {
                    // Top面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Bottom, i, j] = dataTable[(int)SixSides.Bottom, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Front, 2, i] = dataTable[(int)SixSides.Left, 2, i];
                        workTable[(int)SixSides.Right, 2, i] = dataTable[(int)SixSides.Front, 2, i];
                        workTable[(int)SixSides.Back, 2, i] = dataTable[(int)SixSides.Right, 2, i];
                        workTable[(int)SixSides.Left, 2, i] = dataTable[(int)SixSides.Back, 2, i];
                    }
                    break;
                }
            case RotationType.RightRight:
                {
                    // Right面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Right, i, j] = dataTable[(int)SixSides.Right, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Top, i, 2] = dataTable[(int)SixSides.Front, i, 2];
                        workTable[(int)SixSides.Front, i, 2] = dataTable[(int)SixSides.Bottom, i, 2];
                        workTable[(int)SixSides.Bottom, i, 2] = dataTable[(int)SixSides.Back, i, 0];
                        workTable[(int)SixSides.Back, i, 0] = dataTable[(int)SixSides.Top, i, 2];
                    }
                    break;
                }
            case RotationType.RightLeft:
                {
                    // Right面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Right, i, j] = dataTable[(int)SixSides.Right, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Front, i, 2] = dataTable[(int)SixSides.Top, i, 2];
                        workTable[(int)SixSides.Bottom, i, 2] = dataTable[(int)SixSides.Front, i, 2];
                        workTable[(int)SixSides.Back, i, 0] = dataTable[(int)SixSides.Bottom, i, 2];
                        workTable[(int)SixSides.Top, i, 2] = dataTable[(int)SixSides.Back, i, 0];
                    }
                    break;
                }
            case RotationType.LeftRight:
                {
                    // Left面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Left, i, j] = dataTable[(int)SixSides.Left, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Front, i, 0] = dataTable[(int)SixSides.Top, i, 0];
                        workTable[(int)SixSides.Bottom, i, 0] = dataTable[(int)SixSides.Front, i, 0];
                        workTable[(int)SixSides.Back, i, 2] = dataTable[(int)SixSides.Bottom, i, 0];
                        workTable[(int)SixSides.Top, i, 0] = dataTable[(int)SixSides.Back, i, 2];
                    }
                    break;
                }
            case RotationType.LeftLeft:
                {
                    // Left面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Left, i, j] = dataTable[(int)SixSides.Left, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Top, i, 0] = dataTable[(int)SixSides.Front, i, 0];
                        workTable[(int)SixSides.Front, i, 0] = dataTable[(int)SixSides.Bottom, i, 0];
                        workTable[(int)SixSides.Bottom, i, 0] = dataTable[(int)SixSides.Back, i, 2];
                        workTable[(int)SixSides.Back, i, 2] = dataTable[(int)SixSides.Top, i, 0];
                    }
                    break;
                }
            case RotationType.FrontRight:
                {
                    // Front面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Front, i, j] = dataTable[(int)SixSides.Front, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Top, 2, i] = dataTable[(int)SixSides.Left, 2 - i, 2];
                        workTable[(int)SixSides.Left, 2 - i, 2] = dataTable[(int)SixSides.Bottom, 0, i];
                        workTable[(int)SixSides.Bottom, 0, i] = dataTable[(int)SixSides.Right, 2 - i, 0];
                        workTable[(int)SixSides.Right, 2 - i, 0] = dataTable[(int)SixSides.Top, 2, i];
                    }
                    break;
                }
            case RotationType.FrontLeft:
                {
                    // Front面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Front, i, j] = dataTable[(int)SixSides.Front, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Left, 2 - i, 2] = dataTable[(int)SixSides.Top, 2, i];
                        workTable[(int)SixSides.Bottom, 0, i] = dataTable[(int)SixSides.Left, 2 - i, 2];
                        workTable[(int)SixSides.Right, 2 - i, 0] = dataTable[(int)SixSides.Bottom, 0, i];
                        workTable[(int)SixSides.Top, 2, i] = dataTable[(int)SixSides.Right, 2 - i, 0];
                    }
                    break;
                }
            case RotationType.BackRight:
                {
                    // Back面を右回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Back, i, j] = dataTable[(int)SixSides.Back, 2 - j, i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Right, i, 2] = dataTable[(int)SixSides.Bottom, 2, 2 - i];
                        workTable[(int)SixSides.Bottom, 2, 2 - i] = dataTable[(int)SixSides.Left, 2 - i, 0];
                        workTable[(int)SixSides.Left, 2 - i, 0] = dataTable[(int)SixSides.Top, 0, i];
                        workTable[(int)SixSides.Top, 0, i] = dataTable[(int)SixSides.Right, i, 2];
                    }
                    break;
                }
            case RotationType.BackLeft:
                {
                    // Back面を左回転
                    for (int i = 0; i < cubeLength; i++)
                        for (int j = 0; j < cubeLength; j++)
                            workTable[(int)SixSides.Back, i, j] = dataTable[(int)SixSides.Back, j, 2 - i];
                    // 側面の移動
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Bottom, 2, 2 - i] = dataTable[(int)SixSides.Right, i, 2];
                        workTable[(int)SixSides.Left, 2 - i, 0] = dataTable[(int)SixSides.Bottom, 2, 2 - i];
                        workTable[(int)SixSides.Top, 0, i] = dataTable[(int)SixSides.Left, 2 - i, 0];
                        workTable[(int)SixSides.Right, i, 2] = dataTable[(int)SixSides.Top, 0, i];
                    }
                    break;
                }
            case RotationType.CenterRightForward:
                {
                    // ↘   手前に右回転
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Top, i, 1] = dataTable[(int)SixSides.Back, 2 - i, 1];
                        workTable[(int)SixSides.Back, 2 - i, 1] = dataTable[(int)SixSides.Bottom, i, 1];
                        workTable[(int)SixSides.Bottom, i, 1] = dataTable[(int)SixSides.Front, i, 1];
                        workTable[(int)SixSides.Front, i, 1] = dataTable[(int)SixSides.Top, i, 1];
                    }
                    break;
                }
            case RotationType.CenterLeftBack:
                {
                    // ↖   奥側に左回転（RFの逆
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Back, 2 - i, 1] = dataTable[(int)SixSides.Top, i, 1];
                        workTable[(int)SixSides.Bottom, i, 1] = dataTable[(int)SixSides.Back, 2 - i, 1];
                        workTable[(int)SixSides.Front, i, 1] = dataTable[(int)SixSides.Bottom, i, 1];
                        workTable[(int)SixSides.Top, i, 1] = dataTable[(int)SixSides.Front, i, 1];
                    }
                    break;
                }
            case RotationType.CenterLeftForward:
                {
                    // ↙   手前に左回転
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Left, 2 - i, 1] = dataTable[(int)SixSides.Top, 1, i];
                        workTable[(int)SixSides.Top, 1, i] = dataTable[(int)SixSides.Right, i, 1];
                        workTable[(int)SixSides.Right, i, 1] = dataTable[(int)SixSides.Bottom, 1, 2 - i];
                        workTable[(int)SixSides.Bottom, 1, 2 - i] = dataTable[(int)SixSides.Left, 2 - i, 1];
                    }
                    break;
                }
            case RotationType.CenterRightBack:
                {
                    // ↗   奥側に右回転（LFの逆
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Top, 1, i] = dataTable[(int)SixSides.Left, 2 - i, 1];
                        workTable[(int)SixSides.Right, i, 1] = dataTable[(int)SixSides.Top, 1, i];
                        workTable[(int)SixSides.Bottom, 1, 2 - i] = dataTable[(int)SixSides.Right, i, 1];
                        workTable[(int)SixSides.Left, 2 - i, 1] = dataTable[(int)SixSides.Bottom, 1, 2 - i];
                    }
                    break;
                }
            case RotationType.CenterRightSlice:
                {
                    // →  平行に右回転
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Front, 1, i] = dataTable[(int)SixSides.Left, 1, i];
                        workTable[(int)SixSides.Left, 1, i] = dataTable[(int)SixSides.Back, 1, i];
                        workTable[(int)SixSides.Back, 1, i] = dataTable[(int)SixSides.Right, 1, i];
                        workTable[(int)SixSides.Right, 1, i] = dataTable[(int)SixSides.Front, 1, i];
                    }
                    break;
                }
            case RotationType.CenterLeftSlice:
                {
                    // ←  平行に左回転（RSの逆
                    // 側面の移動のみ行う
                    for (int i = 0; i < cubeLength; i++)
                    {
                        workTable[(int)SixSides.Left, 1, i] = dataTable[(int)SixSides.Front, 1, i];
                        workTable[(int)SixSides.Back, 1, i] = dataTable[(int)SixSides.Left, 1, i];
                        workTable[(int)SixSides.Right, 1, i] = dataTable[(int)SixSides.Back, 1, i];
                        workTable[(int)SixSides.Front, 1, i] = dataTable[(int)SixSides.Right, 1, i];
                    }
                    break;
                }
        }
        // 作業の終了したデータをコピー（もういじらないので参照コピーでOK
        // 元のdataTableはGC行き？少し無駄が多いような気はする
        dataTable = workTable;
        // データを反映
        DrawData();
    }

    // キャッシュ用
    GameObject clickStart = null;

    // Update is called once per frame
    void Update()
    {
        // クリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            // rayで当たり判定
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                clickStart = hit.collider.gameObject;
        }
        // クリック中（パフォーマンス的に不安な書き方ではあるが、今回はこれくらいしか重い処理はないのでヨシ
        else if (Input.GetMouseButton(0))
        {
            // 選択がない場合return
            if (clickStart == null)
                return;
            // rayで当たり判定
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                // クリック終了時に当たっていたオブジェクトを取得して回転判定に送る
                InputRotation(hit.collider.gameObject);
            // 空中で離したときも回転させたいが今はとりあえず置いとく
        }
    }

    void InputRotation(GameObject clickEnd)
    {
        // 同じところで離していたら回転させない
        if (clickStart == clickEnd)
            return;
        // 親GameObjectを取得
        var start = clickStart.transform.parent.gameObject;
        var end = clickEnd.transform.parent.gameObject;
        // Indexをリスト照合して取得
        var startIndex = new Vector3Int(-1, -1, -1);
        var endIndex = new Vector3Int(-1, -1, -1);
        for (int x = 0; x < cubeLength; x++)
            for (int y = 0; y < cubeLength; y++)
                for (int z = 0; z < cubeLength; z++)
                {
                    if (start == cubes[x, y, z])
                        startIndex = new Vector3Int(x, y, z);
                    if (end == cubes[x, y, z])
                        endIndex = new Vector3Int(x, y, z);
                }
        // 差分を取得
        var difIndex = endIndex - startIndex;
        Debug.Log(difIndex);
        // それぞれの面の名前を取得
        var startSideName = clickStart.name;
        var endSideName = clickEnd.name;
        //Debug.Log(startSideName);
        //Debug.Log(endSideName);

        int rotationType = -1;
        // start面とend面それぞれについて分岐する（しか思いつきませんでした
        switch (startSideName)
        {
            case "Top":
                {
                    switch (endSideName)
                    {
                        case "Top":
                            // 方向を判定したら軸を判定する
                            if (difIndex == new Vector3Int(1, 0, 0))
                                rotationType =
                                    (startIndex.z == 0) ? (int)RotationType.FrontRight :
                                    (startIndex.z == 1) ? (int)RotationType.CenterRightBack :
                                    (int)RotationType.BackLeft;
                            else if (difIndex == new Vector3Int(-1, 0, 0))
                                rotationType =
                                    (startIndex.z == 0) ? (int)RotationType.FrontLeft :
                                    (startIndex.z == 1) ? (int)RotationType.CenterLeftForward :
                                    (int)RotationType.BackRight;
                            else if (difIndex == new Vector3Int(0, 0, 1))
                                rotationType =
                                    (startIndex.x == 0) ? (int)RotationType.LeftLeft :
                                    (startIndex.x == 1) ? (int)RotationType.CenterLeftBack :
                                    (int)RotationType.RightRight;
                            else if (difIndex == new Vector3Int(0, 0, -1))
                                rotationType =
                                    (startIndex.x == 0) ? (int)RotationType.LeftRight :
                                    (startIndex.x == 1) ? (int)RotationType.CenterRightForward :
                                    (int)RotationType.RightLeft;
                            break;
                        case "Right":
                            break;
                        case "Left":
                            break;
                        case "Front":
                            break;
                        case "Back":
                            break;
                    }
                    break;
                }
            case "Bottom":
                break;
            case "Right":
                break;
            case "Left":
                break;
            case "Front":
                break;
            case "Back":
                break;
        }
        if (rotationType != -1)
            Rotation((RotationType)rotationType);

        // 角の場合名前で判断する
        if (difIndex == new Vector3Int(0, 0, 0)) { }
        else if (difIndex == new Vector3Int(1, 0, 0)) { }
        else if (difIndex == new Vector3Int(-1, 0, 0)) { }
        else if (difIndex == new Vector3Int(0, 1, 0)) { }
        else if (difIndex == new Vector3Int(0, -1, 0)) { }
        else if (difIndex == new Vector3Int(0, 0, 1)) { }
        else if (difIndex == new Vector3Int(0, 0, -1)) { }

        // 回転させたらクリック開始キャッシュをクリア
        clickStart = null;
    }
}
