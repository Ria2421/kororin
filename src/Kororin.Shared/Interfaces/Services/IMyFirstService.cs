////////////////////////////////////////////////////////////////
///
/// �C���^�[�t�F�[�X�Ǘ��e�X�g�p�X�N���v�g
/// 
/// Aughter:���{�S��
///
////////////////////////////////////////////////////////////////

using MagicOnion;
using UnityEngine;

namespace Kororin.Shared.Interfaces.Services
{
    /// <summary>
    /// �C���^�[�t�F�[�X��ݒ�
    /// </summary>
    public interface IMyFirstService : IService<IMyFirstService>
    {
        //�����Z
        UnaryResult<int> SumAsync(int x, int y);
    }
}