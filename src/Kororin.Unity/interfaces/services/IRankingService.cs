//------------------------------------------------------------------------
// �����L���O�C���^�[�t�F�[�X [ IRankingService.cs ]
// Author�FKenta Nakamoto
//------------------------------------------------------------------------
using MagicOnion;
using Kororin.Shared.Interfaces.Model.Entity;

namespace Kororin.Shared.Interfaces.Services
{
    /// <summary>
    /// ���[�U�[�̃C���^�[�t�F�[�X�̒ǉ�(Shared)
    /// </summary>
    public interface IRankingService : IService<IRankingService>
    {
        // �����L���O�̓o�^
        UnaryResult<int> RegistRankingsAsync();

        // �����L���O�̑S�擾
        UnaryResult<User[]> GetAllRankingsAsync();

        // �X�e�[�WID�w��擾
        UnaryResult<User> GetRankingAsync(int stageId);
    }
}