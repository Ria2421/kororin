using UnityEngine;

public class HedgehogParticle : MonoBehaviour
{
    public enum Particle_Type
    {
        Papelitos,
        No,
        Yes,
        Love
    }

    [SerializeField]
    GameObject papelitosPs;
    [SerializeField]
    GameObject noPs;
    [SerializeField]
    GameObject yesPs;
    [SerializeField]
    GameObject lovePs;

    /// <summary>
    /// パーティクル生成処理
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject SpawnParticle(Particle_Type type)
    {
        GameObject prefab = type switch
        {
            Particle_Type.Papelitos => papelitosPs,
            Particle_Type.No => noPs,
            Particle_Type.Yes => yesPs,
            Particle_Type.Love => lovePs,
            _ => null,
        };

        if(!prefab) return null;
        var particle = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        particle.SetActive(true);
        return particle;
    }
}
