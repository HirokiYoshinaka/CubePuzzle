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
        // Dのみ入力
        if (Input.GetKey(KeyCode.D) &&
            !Input.GetKey(KeyCode.A))
            transform.RotateAround(gazingPoint, Vector3.up, rotationalSpeed * Time.deltaTime);
        // Aのみ入力
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
        // 同じキーで同じ方向に回し続けられるようにする
        var sign = (0 <= rot && rot < 180) ? 1 : -1;
        // Wのみ入力
        if (Input.GetKey(KeyCode.W) &&
            !Input.GetKey(KeyCode.S))
            transform.RotateAround(gazingPoint, axis, sign * rotationalSpeed * Time.deltaTime);
        // Sのみ入力
        else if (!Input.GetKey(KeyCode.W) &&
            Input.GetKey(KeyCode.S))
            transform.RotateAround(gazingPoint, axis, sign * -rotationalSpeed * Time.deltaTime);
    }
}
