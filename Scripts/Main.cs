using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField, Header("�萔��ݒ�")] private int _count = default;

    //�ړ�
    private float _moveX = default;
    private float _moveY = default;

    //���͂����ɂ���
    private bool _isMove = default;

    /// <summary>
    /// �c��萔�i�ړ���������������j
    /// </summary>
    private int _residueCount = default;

    //�O�͖��A�P�͕ǁA�Q�̓v���C���[�A�R�͔��A�S�̓S�[���A�U�̓S�[���̏�Ƀv���C���[�A�V�̓S�[���̏�ɔ�
    private const int FLOOR = 0;
    private const int WALL = 1;
    private const int PLAYER = 2;
    private const int BOX = 3;
    private const int GOAL = 4;
    private const int GOAL_ON_PLAYER = 5;
    private const int GOAL_ON_BOX = 6;

    //�}�b�v
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

    [Header("�������͈ړ�������I�u�W�F�N�g�̃v���n�u��ݒ�")]
    [SerializeField] private GameObject _player = default;
    [SerializeField] private GameObject _box = default;
    [SerializeField] private GameObject _goal = default;
    [SerializeField] private GameObject _wall = default;
    [SerializeField] private GameObject _floor = default;

    [Header("Canvas�̎q�I�u�W�F�N�g��ݒ�")]
    //�Q�[���I������UI
    [SerializeField] private GameObject _clearMenu = default;
    [SerializeField] private GameObject _gameOverMenu = default;

    //�萔��UI
    [SerializeField] private Text _countText = default;
    //�c��萔��UI
    [SerializeField] private Text _residueText = default;

    //�}�b�v��̂��ׂĂ̔�
    private GameObject[] _allBox = default;

    [Header("�I�u�W�F�N�g�̃}�b�v��̍��W��ݒ�")]
    [SerializeField] private Vector2Int[] _clearPoint = default;
    [SerializeField] private Vector2Int[] _boxPoint = default;
    [SerializeField] private Vector2Int[] _wallPoint = default;
    [SerializeField, Header("�v���C���[�̏����ʒu")] private Vector3Int _playerPoint = default;

    //�S�[���̐�������
    private int _clearSearch = default;
    private void Start()
    {
        //�S�[���̔z��̒������S�[���̐��Ƃ���
        _clearSearch = _clearPoint.Length;

        //�c��萔��������
        _residueCount = _count;

        //�e�L�X�g�Ɏ萔�𔽉f
        _countText.text = "�萔:" + _count.ToString();

        //�}�b�v�̔z��ɁA�ݒ肵�����W�ɃI�u�W�F�N�g�̐��l������
        //��
        foreach (Vector2Int wall in _wallPoint)
        {
            _stage[wall.x, wall.y] = WALL;
        }

        //�S�[��
        foreach (Vector2Int goal in _clearPoint)
        {
            _stage[goal.x, goal.y] = GOAL;
        }

        //�}�b�v����
        //�s
        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            //��
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                //�}�b�v�̊O����ǂɂ���
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
                    //���𐶐�
                    case 0:
                        Instantiate(_floor, new Vector2(i, j), _floor.transform.rotation);
                        break;
                    //�ǂ𐶐�
                    case 1:
                        Instantiate(_wall, new Vector2(i, j), _wall.transform.rotation);
                        break;
                    //�S�[���𐶐�
                    case 4:
                        Instantiate(_goal, new Vector2(i, j), _goal.transform.rotation);
                        break;
                }
            }
        }

        //������
        foreach (Vector2Int box in _boxPoint)
        {
            Instantiate(_box, new Vector2(box.x, box.y), _box.transform.rotation);
            _stage[box.x, box.y] = BOX;
        }

        //�����ړ������鎞�Ɏg���z��
        _allBox = GameObject.FindGameObjectsWithTag("Box");

        //�v���C���[�̈ʒu��������
        _player.transform.position = new Vector2(_playerPoint.x, _playerPoint.y);
        _stage[_playerPoint.x, _playerPoint.y] = PLAYER;
    }
    private void Update()
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");

        //���Z�b�g�{�^��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //�G�X�P�[�v�L�[�ŃQ�[�����I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //��������Ă��Ȃ��S�[���̐��ƁA�c��萔���c���Ă���
        if (_clearSearch > 0 && _residueCount > 0)
        {
            Move();
        }
        //�N���A
        else if (_clearSearch <= 0)
        {
            _clearMenu.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Title");
            }
        }
        //�Q�[���I�[�o�[
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
    /// �}�b�v�̏�Ԃ�����
    /// �����I�u�W�F�N�g�̈ʒu�X�V�ƃN���A����
    /// </summary>
    private void MapStateSearch()
    {
        //���̔z��̎w�W
        int boxIndex = 0;

        //�S�[���̐���������
        _clearSearch = _clearPoint.Length;

        //�s
        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            //��
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                //�N���A���m�肵�����_�ŏ������I��炷
                if (_clearSearch <= 0)
                {
                    return;
                }
                //�v���C���[�̈ʒu���X�V
                if (_stage[i, j] == PLAYER || _stage[i, j] == GOAL_ON_PLAYER)
                {
                    _playerPoint = new Vector3Int(i, j, 0);
                    _player.transform.position = new Vector2(i, j);
                }
                //���̈ʒu���X�V
                if (_stage[i, j] == BOX || _stage[i, j] == GOAL_ON_BOX)
                {
                    _allBox[boxIndex].transform.position = new Vector2(i, j);
                    boxIndex += 1;
                }
                //�S�[���̏�ɏ���Ă��锠�̐��𐔂���
                if (_stage[i, j] == GOAL_ON_BOX)
                {
                    _clearSearch -= 1;
                }
            }
        }
    }
    /// <summary>
    /// �v���C���[�̈ړ��Ɣ��̈ړ�
    /// �}�b�v�̐��l�𒼐ڏ���������MapStateSearch()�ňʒu���X�V���Ă�
    /// </summary>
    private void Move()
    {
        //�m�F���鐔
        int twoBeside = 2;

        //���͂���Ȃ�������
        if (_moveX == 0 && _moveY == 0)
        {
            //�A���œ����Ȃ��悤�ɂ���
            _isMove = false;
            return;
        }

        //�ړ�
        if (_isMove == false)
        {
            _isMove = true;

            BesideStateConfirmation(new Vector2Int(_playerPoint.x, _playerPoint.y), twoBeside,PLAYER,BOX);

            //�ړ������Ƃ������}�b�v�̍X�V
            MapStateSearch();

            //�c��萔���e�L�X�g�ɍX�V
            _residueText.text = "�c��萔:" + _residueCount.ToString();
        }
    }
    /// <summary>
    /// �v���C���[�̈ʒu����㉺���E�ɁAsearchCount���������邩�m�F����
    /// �ǂ������珈�����Ȃ�
    /// </summary>
    /// <param name="search">��Ԃ��m�F������W</param>
    /// <param name="searchCount">�m�F���鐔</param>
    /// <param name="playerState">�v���C���[�̏��</param>
    /// <param name="boxState">���̏��</param>
    private void BesideStateConfirmation(Vector2Int search, int searchCount,int playerState,int boxState)
    {
        switch (_stage[search.x, search.y])
        {
            //��
            case 0:
                //�����ړ��ł����Ԃ�������
                if (0 >= searchCount)
                {
                    _stage[search.x, search.y] = BOX;
                }

                //�������������A����������
                //�v���C���[�����̏�ɂ�����
                if (playerState == PLAYER&&boxState==BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }
                //�v���C���[���S�[���̏�ɂ�����
                else if(playerState == GOAL_ON_PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }

                //�����S�[���̏�ɂ�������
                //�v���C���[�����̏�ɂ�����
                else if (playerState == PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //�v���C���[���S�[���̏�ɂ�����
                else if (playerState == GOAL_ON_PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //�c��萔�����炷
                _residueCount--;
                break;

            //�v���C���[
            case 2:
                //�i�s�����̏�Ԃ��m�F
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y],boxState);
                break;

            //��
            case 3:
                //�i�s�����ɔ��������ł��珈�����I��������
                if (0 >= searchCount)
                {
                    break;
                }

                //�i�s�����̏�Ԃ��m�F
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y], _stage[search.x, search.y]);
                break;

            //�S�[��
            case 4:
                //�����ړ��ł����Ԃ�������
                if (0 >= searchCount)
                {
                    _stage[search.x, search.y] = GOAL_ON_BOX;
                }

                //�����Ȃ�������
                //�v���C���[�����̏�ɂ�����
                if (playerState == PLAYER&&searchCount==1)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //�v���C���[���S�[���̏�ɂ�����
                else if(playerState == GOAL_ON_PLAYER && searchCount==1)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //������������
                //�v���C���[�����̏�ɂ�����
                else if (playerState == PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }
                //�v���C���[���S�[���̏�ɂ�����
                else if (playerState == GOAL_ON_PLAYER && boxState == BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = PLAYER;
                }

                //�����S�[���̏�ɂ�������
                //�v���C���[�����̏�ɂ�����
                else if (playerState == PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = FLOOR;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }
                //�v���C���[���S�[���̏�ɂ�����
                else if(playerState == GOAL_ON_PLAYER && boxState == GOAL_ON_BOX)
                {
                    _stage[_playerPoint.x, _playerPoint.y] = GOAL;
                    _stage[_playerPoint.x + (int)_moveX, _playerPoint.y + (int)_moveY] = GOAL_ON_PLAYER;
                }

                //�c��萔�����炷
                _residueCount--;
                break;

            //�S�[���̏�Ƀv���C���[
            case 5:
                //�i�s�����̏�Ԃ��m�F
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y],boxState);
                break;

            //�S�[���̏�ɔ�
            case 6:
                //�i�s�����ɔ��������ł��珈�����I��������
                if (0 >= searchCount)
                {
                    break;
                }

                //�i�s�����̏�Ԃ��m�F
                BesideStateConfirmation(new Vector2Int(search.x + (int)_moveX, search.y + (int)_moveY),
                    --searchCount, _stage[_playerPoint.x, _playerPoint.y], _stage[search.x, search.y]);
                break;
        }
    }
}
