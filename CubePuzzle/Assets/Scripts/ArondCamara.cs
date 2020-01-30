using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーザーの入力に応じてカメラの回転移動を行います。
/// </summary>
public class ArondCamara : MonoBehaviour
{
    // 原点を注視点とする（今回はオブジェクトの位置は移動しないため
    Vector3 gazingPoint = Vector3.zero;
    public float rotationalSpeed = 180;

    // Update is called once per frame
    void Update()
    {
        // 左右回転
        // Dのみ
        if (Input.GetKey(KeyCode.D) &&
            !Input.GetKey(KeyCode.A))
            transform.RotateAround(gazingPoint, Vector3.up, rotationalSpeed * Time.deltaTime);
        // Aのみ
        else if (!Input.GetKey(KeyCode.D) &&
            Input.GetKey(KeyCode.A))
            transform.RotateAround(gazingPoint, Vector3.up, -rotationalSpeed * Time.deltaTime);
        // y軸回転の度数を取得、幸い0~360でキレイに返ってきた
        var rot = transform.rotation.eulerAngles.y;
        var rad = rot * Mathf.Deg2Rad;
        // 高さ移動のAxisを計算する
        Vector3 axis = Vector3.zero;
        /// 上キーで上側に回り込むように
        ///   0度のとき( 1.0f, 0,  0.0f)
        ///  90度のとき( 0.0f, 0, -1.0f)
        /// 180度のとき(-1.0f, 0,  0.0f)
        /// 270度のとき( 0.0f, 0,  1.0f)
        /// 360度のとき( 1.0f, 0,  0.0f)
        /// つまり三角関数を用いてこういうこと
        axis.x = +Mathf.Cos(rad);
        axis.z = -Mathf.Sin(rad);

        // Wのみ
        if (Input.GetKey(KeyCode.W) &&
            !Input.GetKey(KeyCode.S))
            transform.RotateAround(gazingPoint, axis, rotationalSpeed * Time.deltaTime);
        // Sのみ
        else if (!Input.GetKey(KeyCode.W) &&
            Input.GetKey(KeyCode.S))
            transform.RotateAround(gazingPoint, axis, -rotationalSpeed * Time.deltaTime);
    }
}
