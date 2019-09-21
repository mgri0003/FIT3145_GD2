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
    private List<Effect> m_projectileEffects = new List<Effect>();

    //--Methods--

    public void Init(float newDamage, float newSpeed, Vector3 newDirection)
    {
        m_damage = newDamage;
        m_speed = newSpeed;
        m_direction = newDirection;
        m_hasInit = true;
    }

    public void SetLifeTime(float newLifeTime) { m_currentLifeTime = newLifeTime; }

    public void AddProjectileEffect(Effect projEffect)
    {
        m_projectileEffects.Add(projEffect);
    }
    public void AddProjectileEffects(List<Effect> projEffects)
    {
        m_projectileEffects.AddRange(projEffects);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_hasInit, "Projectile Not Initialised!!!");
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

            CheckCollisions();
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

    private void CheckCollisions()
    {
        List<GameObject> gameObjectsHit = m_hitbox.GetAllGameObjectsCollided();
        if (gameObjectsHit.Count > 0)
        {
            bool validCollision = false;

            foreach (GameObject go in gameObjectsHit)
            {
                if(go)
                {
                    if (go.tag == "Character")
                    {
                        go.GetComponent<Character_Core>().ReceiveHit(m_damage, m_projectileEffects);
                        validCollision = true;
                        break;
                    }
                }
            }

            foreach (GameObject go in gameObjectsHit)
            {
                if (go)
                {
                    if (go.transform.root.tag == "World")
                    {
                        validCollision = true;
                    }
                }
            }

            if(validCollision)
            {
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }



}
