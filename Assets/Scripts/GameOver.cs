using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartEndOfTheWorld();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] GameObject Explosion;
    [SerializeField] GameObject Explosions;

    [ContextMenu("generate explose")]
    private void StartEndOfTheWorld()
    {
        StartCoroutine(TheEndOfTheWorld());
    }

    private IEnumerator TheEndOfTheWorld()
    {
        int nbExplosion = 100;
        Vector3 min = new Vector3(-8, -4, 0);
        Vector3 max = new Vector3(8, 4, 0);
        for(var i = 0; i < nbExplosion; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f,1f)*0.1f);
            SummonExplosion(RandomVector3(min, max), Explosion.transform);
        }
    }

    private Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        Vector3 rand = Vector3.zero;
        rand.x = UnityEngine.Random.Range(min.x, max.x);
        rand.y = UnityEngine.Random.Range(min.y, max.y);
        rand.z = UnityEngine.Random.Range(min.z, max.z);

        return rand;
    }

    void SummonExplosion(Vector3 pos, Transform explosionTrans)
    {
        GameObject explosion = Instantiate(Explosion, pos, Explosion.transform.rotation, Explosions.transform);
        Destroy(explosion,0.583f*5);
    }
    
}
