﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 
 * オブジェクトの描画と各種コンポーネントのアタッチのみ行う
 * 台形
 * 
 * 
 */

public class TrapezoidPole : MonoBehaviour {

    private int poleNum = -1;
    private int poleSum;
    private float radiusOut;
    private float radiusIn;
    private float poleHeight;

    private createTrapezoidPole createSorce;
    private centralSystem systemScript;

    //頂点座標
    Vector3[] vertex = new Vector3[24];

    //面情報
    int[] face = new int[] { 1, 3, 0,
                             3, 2, 0,

                             5, 4, 2+8,
                             3+8, 5, 2+8,

                             7, 6, 4+8,
                             5+8, 7, 4+8,

                             1+8, 0+8, 6+8,
                             7+8, 1+8, 6+8,

                             2+16, 4+16, 0+16,
                             4+16, 6+16, 0+16,

                             7+16, 3+16, 1+16,
                             5+16, 3+16, 7+16,
    };

    //マテリアル
    public Material _material;

    void Start() {
        createSorce = GameObject.Find("central").GetComponent<createTrapezoidPole>();
        systemScript = GameObject.Find("central").GetComponent<centralSystem>();
    }

    void Update() {

        //初期値を与えられたら処理する
        if (poleNum != -1) {

            //頂点計算
            CalcVertices();

            //メッシュ作成
            Mesh mesh = new Mesh();
            //メッシュリセット
            mesh.Clear();
            //メッシュへの頂点情報の追加
            mesh.vertices = vertex;
            //メッシュへの面情報の追加
            mesh.triangles = face;

            //メッシュフィルター追加
            MeshFilter mesh_filter = new MeshFilter();
            this.gameObject.AddComponent<MeshFilter>();
            mesh_filter = GetComponent<MeshFilter>();
            //メッシュアタッチ
            mesh_filter.mesh = mesh;
            //レンダラー追加 + マテリアルアタッチ
            this.gameObject.AddComponent<MeshRenderer>();
            this.gameObject.GetComponent<MeshRenderer>().material = _material;
            //コライダーアタッチ
            this.gameObject.AddComponent<MeshCollider>();
            this.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

            //NormalMapの再計算
            mesh_filter.mesh.RecalculateNormals();

            //暫定当たり判定用Event Trigger
            //イベントトリガーのアタッチと初期化
            EventTrigger currentTrigger = this.gameObject.AddComponent<EventTrigger>();
            currentTrigger.triggers = new List<EventTrigger.Entry>();
            //イベントトリガーのトリガーイベント作成
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((x) => OnTouchPointer());  //ラムダ式の右側は追加するメソッド
            //トリガーイベントのアタッチ
            currentTrigger.triggers.Add(entry);

            poleNum = -1;
        }
    }

    //何個のオブジェクト中の何番目のオブジェクトか
    public void SetPoleNums(int num, int sum, float radOut, float radIn, float height) {
        poleNum = num;
        poleSum = sum;
        radiusOut = radOut;
        radiusIn = radIn;
        poleHeight = height;
    }

    private void CalcVertices() {
        //台形の外側の頂点座標その1
        Vector3 vertex1 = new Vector3(radiusOut * Mathf.Sin(( poleNum + 0 ) / (float)poleSum * Mathf.PI * 2),
                                      radiusOut * Mathf.Cos(( poleNum + 0 ) / (float)poleSum * Mathf.PI * 2),
                                      0);
        //台形の外側の頂点座標その2
        Vector3 vertex2 = new Vector3(radiusOut * Mathf.Sin(( poleNum + 1 ) / (float)poleSum * Mathf.PI * 2),
                                      radiusOut * Mathf.Cos(( poleNum + 1 ) / (float)poleSum * Mathf.PI * 2),
                                      0);
        //台形の内側の頂点座標その1
        Vector3 vertex3 = new Vector3(radiusIn * Mathf.Sin(( poleNum + 0 ) / (float)poleSum * Mathf.PI * 2),
                                      radiusIn * Mathf.Cos(( poleNum + 0 ) / (float)poleSum * Mathf.PI * 2),
                                      0);
        //台形の内側の頂点座標その2
        Vector3 vertex4 = new Vector3(radiusIn * Mathf.Sin(( poleNum + 1 ) / (float)poleSum * Mathf.PI * 2),
                                      radiusIn * Mathf.Cos(( poleNum + 1 ) / (float)poleSum * Mathf.PI * 2),
                                      0);
        //全頂点数6にそれぞれ座標が3つずつある
        for (int i = 0; i < 8 * 3; i++) {
            if (i % 8 == 0) {
                vertex[i] = vertex3;
            } else if (i % 8 == 1) {
                vertex[i] = new Vector3(vertex3.x, vertex3.y, poleHeight);
            } else if (i % 8 == 2) {
                vertex[i] = vertex1;
            } else if (i % 8 == 3) {
                vertex[i] = new Vector3(vertex1.x, vertex1.y, poleHeight);
            } else if (i % 8 == 4) {
                vertex[i] = vertex2;
            } else if (i % 8 == 5) {
                vertex[i] = new Vector3(vertex2.x, vertex2.y, poleHeight);
            } else if (i % 8 == 6) {
                vertex[i] = vertex4;
            } else if (i % 8 == 7) {
                vertex[i] = new Vector3(vertex4.x, vertex4.y, poleHeight);
            } else {
                Debug.LogWarning("calcration error");
            }

        }
        createSorce.callBackVertex(new Vector3[4] { vertex[0], vertex[6], vertex[1],vertex[7]}, int.Parse(gameObject.name));
    }

    private void OnTouchPointer() {
        Debug.Log(int.Parse(gameObject.name));
        systemScript.UpdateChuringNum(int.Parse(gameObject.name));
    }
}
