////////////////////////////////////////////////////////////////
///
/// Unity�ƃT�[�o�[�̐ڑ����Ǘ�����X�N���v�g
/// 
/// Aughter:�ؓc�W��
///
////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : MonoBehaviour
{
#if DEBUG
    //localhost��Azure��URL������
   public const string ServerURL = "http://localhost:5254";
    //public const string ServerURL = "http://car-boom-crash.japaneast.cloudapp.azure.com:5254";
#else
    public const string ServerURL = "http://car-boom-crash.japaneast.cloudapp.azure.com:5254";
#endif
}
