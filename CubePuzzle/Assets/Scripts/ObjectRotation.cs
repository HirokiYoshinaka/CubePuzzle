using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクト全体の回転を行います
/// </summary>
public class ObjectRotation : MonoBehaviour
{
    // 回転の速度を指定します。
    public float rotationalSpeed = 180;

    /// <summary>
    /// 使用キー定義
    /// </summary>
    readonly KeyCode rightKey = KeyCode.D;
    readonly KeyCode leftKey = KeyCode.A;
    readonly KeyCode upKey = KeyCode.W;
    readonly KeyCode downKey = KeyCode.S;
    readonly KeyCode resetKey = KeyCode.Z;
    /// <summary>
    /// オブジェクトの回転を初期位置に戻します。
    /// </summary>
    public void InitRotation()
    {
        // 回転をリセット
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(new Vector3(0, 1, 0), -45, Space.World);
        transform.Rotate(new Vector3(1, 0, 0), -30, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        // クリック中は操作不可とする（回転選択が面倒になるので
        if (Input.GetMouseButton(0))
            return;
        // 左右回転
        // Rightのみ入力
        if (Input.GetKey(rightKey) &&
            !Input.GetKey(leftKey))
            transform.Rotate(Vector3.up, -rotationalSpeed * Time.deltaTime, Space.World);
        // Leftのみ入力
        else if (!Input.GetKey(rightKey) &&
            Input.GetKey(leftKey))
            transform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime, Space.World);

        // Upのみ入力
        if (Input.GetKey(upKey) &&
            !Input.GetKey(downKey))
            transform.Rotate(new Vector3(1, 0, 0), rotationalSpeed * Time.deltaTime, Space.World);
        // Downのみ入力
        else if (!Input.GetKey(upKey) &&
            Input.GetKey(downKey))
            transform.Rotate(new Vector3(1, 0, 0), -rotationalSpeed * Time.deltaTime, Space.World);

        // 回転をリセット
        if (Input.GetKeyDown(resetKey))
            InitRotation();
    }
}
