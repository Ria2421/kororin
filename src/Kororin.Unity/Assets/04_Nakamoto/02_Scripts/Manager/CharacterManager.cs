//---------------------------------------------------
// �L�����N�^�[�Ǘ� [ CharacterManager.cs ]
//  Author:Kenta Nakamoto
//---------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    //-----------------------
    // �t�B�[���h

    /// <summary>
    /// �Q���҂̃v���C���[�I�u�W�F���X�g
    /// </summary>
    private Dictionary<Guid, GameObject> playerObjs = new Dictionary<Guid, GameObject>();

    public Dictionary<Guid, GameObject> PlayerObjs { get { return playerObjs; } }

    [SerializeField] private GameObject playerPrefab;   // ��������v���C���[�I�u�W�F

    [SerializeField] private GameObject playerObjSelf;  // ���[�J���p�ɑ����t�^

    //------------
    // �萔

    private const float UPDATE_SEC = 0.1f;  // �ʐM�p�x

    //-----------------------
    // �ʐM�֘A

    /// <summary>
    /// �����҂̃v���C���[�I�u�W�F�̐���
    /// </summary>
    /// <param name="connectionID"></param>
    public void GenerateCharacters(Guid connectionID)
    {
        // �J�n�ʒu�̐ݒ�
        //var point = startPoints[0];
        //startPoints.RemoveAt(0);

        var playerObj = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
        playerObjs.Add(connectionID, playerObj);
    }

    /// <summary>
    /// �Q�����Ă��郆�[�U�[�������ɁA�S���̃v���C���[�𐶐�����
    /// </summary>
    private void GenerateAllCharacters()
    {
        foreach (var joinduser in RoomModel.Instance.joinedUserList)
        {
            // �J�n�ʒu�̐ݒ�
            //var point = startPoints[0];
            //startPoints.RemoveAt(0);

            var playerObj = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
            playerObjs.Add(joinduser.Key, playerObj);

            // ���g�̃v���C���[�𐶐������ꍇ
            if (joinduser.Key == RoomModel.Instance.ConnectionId)
            {
                playerObjSelf = playerObj;

                // �J�����̒Ǐ]�ݒ�
                //if (cinemachineTargetGroup)
                //{
                //    var newTarget = new CinemachineTargetGroup.Target
                //    {
                //        Object = playerObjSelf.transform,
                //        Radius = 1f,
                //        Weight = 1f
                //    };
                //    cinemachineTargetGroup.Targets.Add(newTarget);
                //}
                //else
                //{
                //    var target = new CameraTarget();
                //    target.TrackingTarget = playerObjSelf.transform;
                //    target.LookAtTarget = playerObjSelf.transform;
                //    camera.GetComponent<CinemachineCamera>().Target.TrackingTarget = playerObjSelf.transform;
                //}
            }
        }
    }

    /// <summary>
    /// �L�����N�^�[�̏��X�V�Ăяo���p�R���[�`��
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (TestTopManager.Instance.IsConnected)
            {
                UpdateCharacterDataRequest();
            }
            yield return new WaitForSeconds(UPDATE_SEC);
        }
    }

    /// <summary>
    /// �v���C���[�̏��X�V
    /// </summary>
    async void UpdateCharacterDataRequest()
    {
        var playerData = GetCharacterData();

        // �v���C���[���X�V���N�G�X�g
        await RoomModel.Instance.UpdateCharacterAsync(playerData);
    }

    /// <summary>
    /// �v���C���[���擾
    /// </summary>
    /// <returns></returns>
    CharacterData GetCharacterData()
    {
        if (!playerObjs.ContainsKey(RoomModel.Instance.ConnectionId)) return null;
        var player = playerObjs[RoomModel.Instance.ConnectionId].GetComponent<Player>();
        return new CharacterData()
        {
            Position = player.transform.position,
            Scale = player.transform.localScale,
            Rotation = player.transform.rotation,
            AnimationId = player.GetAnimId(),

            // �ȉ��͐�p�ϐ�
            ConnectionID = RoomModel.Instance.ConnectionId,
        };
    }
}
