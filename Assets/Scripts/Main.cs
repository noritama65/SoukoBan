using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField, Header("手数を設定")] private int _count = default;

    //移動
    private float _moveX = default;
    private float _moveY = default;

    //入力を一回にする
    private bool _isMove = default;

    /// <summary>
    /// 残り手数（移動した分だけ減る）
    /// </summary>
    private int _residueCount = default;

    //０は無、１は壁、２はプレイヤー、３は箱、４はゴール、６はゴールの上にプレイヤー、７はゴールの上に箱
    private const int FLOOR = 0;
    private const int WALL = 1;
    private const int PLAYER = 2;
    private const int BOX = 3;
    private const int GOAL = 4;
    private const int GOAL_ON_PLAYER = 5;
    private const int GOAL_ON_BOX = 6;

    //マップ
    private int[,] _stage = {
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 },
    {0,0,0,0,0,0,0,0,0,0 } };

    [Header("生成又は移動させるオブジェクトのプレハブを設定")]
    [SerializeField] private GameObject _player = default;
    [SerializeField] private GameObject _box = default;
    [SerializeField] private GameObject _goal = default;
    [SerializeField] private GameObject _wall = default;
    [SerializeField] private GameObject _floor = default;

    [Header("Canvasの子オブジェクトを設定")]
    //ゲーム終了時のUI
    [SerializeField] private GameObject _clearMenu = default;
    [SerializeField] private GameObject _gameOverMenu = default;

    //手数のUI
    [SerializeField] private Text _countText = default;
    //残り手数のUI
    [SerializeField] private Text _residueText = default;

    //マップ上のすべての箱
    private GameObject[] _allBox = default;

    [Header("オブジェクトのマップ上の座標を設定")]
    [SerializeField] private Vector2Int[] _clearPoint = default;
    [SerializeField] private Vector2Int[] _boxPoint = default;
    [SerializeField] private Vector2Int[] _wallPoint = default;
    [SerializeField, Header("プレイヤーの初期位置")] private Vector3Int _playerPoint = default;

    //ゴールの数を入れる
    private int _clearSearch = default;
    private void Start()
    {
        //ゴールの配列の長さをゴールの数とする
        _clearSearch = _clearPoint.Length;

        //残り手数を初期化
        _residueCount = _count;

        //テキストに手数を反映
        _countText.text = "手数:" + _count.ToString();

        //マップの配列に、設定した座標にオブジェクトの数値を入れる
        //壁
        foreach (Vector2Int wall in _wallPoint)
        {
            _stage[wall.x, wall.y] = WALL;
        }

        //ゴール
        foreach (Vector2Int goal in _clearPoint)
        {
            _stage[goal.x, goal.y] = GOAL;
        }

        //マップ生成
        //行
        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            //列
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                //マップの外周を壁にする
                if (i == 0 && j == 0)
                {
                    _stage[i, j] = 1;
                }
                if (i == 0 && j != 0)
                {
                    _stage[i, j] = 1;
                }
                if (i != 0 && j == 0)
                {
                    _stage[i, j] = 1;
                }
                if (i == 9 && j != 0)
                {
                    _stage[i, j] = 1;
                }
                if (i != 0 && j == 9)
                {
                    _stage[i, j] = 1;
                }

                switch (_stage[i, j])
                {
                    //床を生成
                    case 0:
                        Instantiate(_floor, new Vector2(i, j), _floor.transform.rotation);
                        break;
                    //壁を生成
                    case 1:
                        Instantiate(_wall, new Vector2(i, j), _wall.transform.rotation);
                        break;
                    //ゴールを生成
                    case 4:
                        Instantiate(_goal, new Vector2(i, j), _goal.transform.rotation);
                        break;
                }
            }
        }

        //箱生成
        foreach (Vector2Int box in _boxPoint)
        {
            Instantiate(_box, new Vector2(box.x, box.y), _box.transform.rotation);
            _stage[box.x, box.y] = BOX;
        }

        //箱を移動させる時に使う配列
        _allBox = GameObject.FindGameObjectsWithTag("Box");

        //プレイヤーの位置を初期化
        _player.transform.position = new Vector2(_playerPoint.x, _playerPoint.y);
        _stage[_playerPoint.x, _playerPoint.y] = PLAYER;
    }
    private void Update()
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");

        //リセットボタン
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //エスケープキーでゲームを終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //箱が乗っていないゴールの数と、残り手数が残ってたら
        if (_clearSearch > 0 && _residueCount > 0)
        {
            Move();
        }
        //クリア
        else if (_clearSearch <= 0)
        {
            _clearMenu.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Title");
            }
        }
        //ゲームオーバー
        else if (_residueCount <= 0)
        {
            _gameOverMenu.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
    /// <summary>
    /// マップの状態を検査
    /// 動くオブジェクトの位置更新とクリア判定
    /// </summary>
    private void MapStateSearch()
    {
        //箱の配列の指標
        int boxIndex = 0;

        //ゴールの数を初期化
        _clearSearch = _clearPoint.Length;

        //行
        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            //列
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                //クリアが確定した時点で処理を終わらす
                if (_clearSearch <= 0)
                {
                    return;
                }
                //プレイヤーの位置を更新
                if (_stage[i, j] == PLAYER || _stage[i, j] == GOAL_ON_PLAYER)
                {
                    _playerPoint = new Vector3Int(i, j, 0);
                    _player.transform.position = new Vector2(i, j);
                }
                //箱の位置を更新
                if (_stage[i, j] == BOX || _stage[i, j] == GOAL_ON_BOX)
                {
                    _allBox[boxIndex].transform.position = new Vector2(i, j);
                    boxIndex += 1;
                }
                //ゴールの上に乗っている箱の数を数える
                if (_stage[i, j] == GOAL_ON_BOX)
                {
                    _clearSearch -= 1;
                }
            }
        }
    }
    /// <summary>
    /// プレイヤーの移動と箱の移動
    /// マップの数値を直接書き換えてMapStateSearch()で位置を更新してる
    /// </summary>
    private void Move()
    {
        //確認する数
        int twoBeside = 2;

        //入力されなかったら
        if (_moveX == 0 && _moveY == 0)
        {
            //連続で動かないようにする
            _isMove = false;
            return;
        }

        //移動
        if (_isMove == false)
        {
            _isMove = true;

            BesideStateConfirmation(new Vector2Int(_playerPoint.x, _playerPoint.y), twoBeside,PLAYER,BOX);

            //移動したときだけマップの更新
            MapStateSearch();

            //残り手数をテキストに更新
            _residueText.text = "残り手数:" + _residueCount.ToString();
        }
    }
    /// <summary>
    /// プレイヤーの位置から上下左右に、searchCount分何があるか確認する
    /// 壁だったら処理しない
    /// </summary>
    /// <param name="search">状態を確認する座標</param>
    /// <param name="searchCount">確認する数</param>
    /// <param name="playerState">プレイヤーの状態</param>
    /// <param name="boxState">箱の状態</param>
    private void BesideStateConfirmation(Vector2Int search, int searchCount,int playerState,int boxState)
    {
        switch (_stage[search.x, search.y])
        {
            //床
            case 0:
                //箱が移動できる状態だったら
                if (0 >= searchCount)
                {
                    _stage[search.x, search.y] = BOX;
                }

                //箱があった時、無かった時
                //プレイヤーが床の上にいたら
                if (playerState == PLAYER&&boxState==BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }
                //プレイヤーがゴールの上にいたら
                else if(playerState == GOAL_ON_PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }

                //箱がゴールの上にあった時
                //プレイヤーが床の上にいたら
                else if (playerState == PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //プレイヤーがゴールの上にいたら
                else if (playerState == GOAL_ON_PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //残り手数を減らす
                _residueCount--;
                break;

            //プレイヤー
            case 2:
                //進行方向の状態を確認
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y],boxState);
                break;

            //箱
            case 3:
                //進行方向に箱が二つ並んでたら処理を終了させる
                if (0 >= searchCount)
                {
                    break;
                }

                //進行方向の状態を確認
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y], _stage[search.x, search.y]);
                break;

            //ゴール
            case 4:
                //箱が移動できる状態だったら
                if (0 >= searchCount)
                {
                    _stage[search.x, search.y] = GOAL_ON_BOX;
                }

                //箱がなかった時
                //プレイヤーが床の上にいたら
                if (playerState == PLAYER&&searchCount==1)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //プレイヤーがゴールの上にいたら
                else if(playerState == GOAL_ON_PLAYER && searchCount==1)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //箱があった時
                //プレイヤーが床の上にいたら
                else if (playerState == PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }
                //プレイヤーがゴールの上にいたら
                else if (playerState == GOAL_ON_PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }

                //箱がゴールの上にあった時
                //プレイヤーが床の上にいたら
                else if (playerState == PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //プレイヤーがゴールの上にいたら
                else if(playerState == GOAL_ON_PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //残り手数を減らす
                _residueCount--;
                break;

            //ゴールの上にプレイヤー
            case 5:
                //進行方向の状態を確認
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y],boxState);
                break;

            //ゴールの上に箱
            case 6:
                //進行方向に箱が二つ並んでたら処理を終了させる
                if (0 >= searchCount)
                {
                    break;
                }

                //進行方向の状態を確認
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y], _stage[search.x, search.y]);
                break;
        }
    }
}
