using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Projectile : MonoBehaviour
{
    //--Variables--
    private bool m_hasInit = false;
    private float m_damage = 0;
    private float m_speed = 0;
    private Vector3 m_direction = Vector3.zero;
    private const float PROJECTILE_LIFETIME = 5;
    private float m_currentLifeTime = PROJECTILE_LIFETIME;
    [SerializeField] private Hitbox m_hitbox = null;

    //--Methods--

    public void Init(float newDamage, float newSpeed, Vector3 newDirection)
    {
        m_damage = newDamage;
        m_speed = newSpeed;
        m_direction = newDirection;
        m_hasInit = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_hasInit, "Projectile Not Initialsed!!!");
        Debug.Assert(m_hitbox, "Projectile Hitbox unassigned!?!?/");
    }

    // Update is called once per frame
    void Update()
    {
        if(m_hasInit)
        {
            //constantly move
            transform.Translate(m_direction * m_speed * Time.deltaTime, Space.World);

            UpdateLifetime();

            CheckCollissions();
        }
    }

    private void UpdateLifetime()
    {
        if (m_currentLifeTime > 0)
        {
            m_currentLifeTime -= Time.deltaTime;
        }
        else
        {
            DestroySelf();
        }
    }

    private void CheckCollissions()
    {
        List<GameObject> gameObjectsHit = m_hitbox.GetAllGameObjectsCollided();
        if (gameObjectsHit.Count > 0)
        {
            foreach (GameObject go in gameObjectsHit)
            {
                if (go.tag == "Character")
                {
                    //boop em forward
                    go.GetComponent<Rigidbody>().AddForce(transform.forward * 100.0f);

                    DestroySelf();
                    break;
                }
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }


}
