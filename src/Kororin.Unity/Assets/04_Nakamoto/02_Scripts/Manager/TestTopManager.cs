//--------------------------------------------------
// �g�b�v��ʒʐM�e�X�g�p [ TestTopManager.cs ]
// Author:Kenta Nakamoto
//--------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTopManager : MonoBehaviour
{
    [SerializeField] int testId = 0;
    [SerializeField] string testName = "";

    public static TestTopManager Instance { get; private set; }

    // �N������
    private async void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
        if (Instance == null) Instance = this;

        await RoomModel.Instance.ConnectAsync();
    }

    // ��������
    void Start()
    {
        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
    }

    // �V���O�����[�h�J��
    public void TransitionSinglScene()
    {
        LoadingManager.Instance.StartSceneLoad("01_Stage");
    }

    // �}���`���[�h�J��
    public async void TransitionMultiScene()
    {
        await RoomModel.Instance.JoinedAsync("Kororin",testId,testName);
    }

    #region �ʒm����

    // ���[���쐬�ʒm
    public void OnCreatedRoom()
    {
        Debug.Log("����");
    }

    // ���������ʒm
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserName + "���������܂����B");
    }

    // �ގ��ʒm
    public void OnLeavedUser(JoinedUser leavedUser)
    {
        Debug.Log(leavedUser.UserName + "���ގ����܂����B");
    }

    #endregion

}
