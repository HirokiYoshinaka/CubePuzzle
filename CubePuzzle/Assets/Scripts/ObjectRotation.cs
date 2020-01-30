using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクト全体の回転を行います
/// </summary>
public class ObjectRotation : MonoBehaviour
{
    public float rotationalSpeed = 180;

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
        // 左右回転
        // Dのみ入力
        if (Input.GetKey(KeyCode.D) &&
            !Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime, Space.World);
        // Aのみ入力
        else if (!Input.GetKey(KeyCode.D) &&
            Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -rotationalSpeed * Time.deltaTime, Space.World);

        // Wのみ入力
        if (Input.GetKey(KeyCode.W) &&
            !Input.GetKey(KeyCode.S))
            transform.Rotate(new Vector3(1, 0, 0), rotationalSpeed * Time.deltaTime,Space.World);
        // Sのみ入力
        else if (!Input.GetKey(KeyCode.W) &&
            Input.GetKey(KeyCode.S))
            transform.Rotate(new Vector3(1, 0, 0), -rotationalSpeed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.Z))
            InitRotation();
    }
}
