using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damage = other.GetComponent<IDamagable>();
        if (damage != null)
            damage.OnDamage();

        //Class Batu batu = other.gameobject.getcomponent<Batu>();
        // if(Batu) Batu.BatuHancur(); 

        //Class KotakAmunisi kotak = other.gameobject.getcomponent<KotakAmunisi>();
        // if(kotak) kotak.kotakHancur();

        //Class BarrelExplosion barrel = other.gameobject.getcomponent<BarrelExplosion>();
        // if(barrel) barrel.BarrelHancur();

        //Class Batu batu = other.gameobject.getcomponent<Batu>();
        // if(Batu) Batu.BatuHancur(); 

        //Class KotakAmunisi kotak = other.gameobject.getcomponent<KotakAmunisi>();
        // if(kotak) kotak.kotakHancur();

        //Class BarrelExplosion barrel = other.gameobject.getcomponent<BarrelExplosion>();
        // if(barrel) barrel.BarrelHancur();

        //Class Batu batu = other.gameobject.getcomponent<Batu>();
        // if(Batu) Batu.BatuHancur(); 

        //Class KotakAmunisi kotak = other.gameobject.getcomponent<KotakAmunisi>();
        // if(kotak) kotak.kotakHancur();

        //Class BarrelExplosion barrel = other.gameobject.getcomponent<BarrelExplosion>();
        // if(barrel) barrel.BarrelHancur();

        //Class Batu batu = other.gameobject.getcomponent<Batu>();
        // if(Batu) Batu.BatuHancur(); 

        //Class KotakAmunisi kotak = other.gameobject.getcomponent<KotakAmunisi>();
        // if(kotak) kotak.kotakHancur();

        //Class BarrelExplosion barrel = other.gameobject.getcomponent<BarrelExplosion>();
        // if(barrel) barrel.BarrelHancur();
    }
}

public interface IDamagable
{
    void OnDamage();
}
public class KotakAmunisi : MonoBehaviour, IDamagable
{
    public void OnDamage()
    {
        KotakHancur();
    }

    public void KotakHancur()
    {

    }
}

public class BaseEnemy : MonoBehaviour
{
    public float health;
    [SerializeField] GameObject prefabBullet;
    public virtual void Attack()
    {
        InstantiteBullet();
    }
    void InstantiteBullet()
    {

    }
}
public class EnemyManusia : BaseEnemy
{
    public override void Attack()
    {
        TrejectorLurus();
        base.Attack();
    }
    void TrejectorLurus()
    {

    }
}
public class Drone : BaseEnemy
{
    public override void Attack()
    {
        TrejectorBawah();
        base.Attack();
    }
    void TrejectorBawah()
    {

    }
}
public class DroneListrik : BaseEnemy
{
    float newHealth;
    private void Awake()
    {
        health = newHealth;
    }
    void MendekatiPlayer()
    {

    }
    void Kamikaze()
    {

    }
    public override void Attack()
    {
        MendekatiPlayer();
        Kamikaze();
        base.Attack();
    }
}
public class EnemyManusiaRacun : EnemyManusia
{
    void MencariTempatLindung()
    {
    }
    public override void Attack()
    {
        MencariTempatLindung();
        base.Attack();
    }
}
public class OldBaseEnemy : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] GameObject prefabBullet;

    public enum TypeBaseEnemy { Manusia, Turret, Tank, Drone, ManusiaRacun, TankListrik, DroneListrik, TurretShield }
    [SerializeField] TypeBaseEnemy type;
    private void Attack1()
    {
        InstantiateBullet(prefabBullet);
    }
    private void Attack2()
    {
        switch (type)
        {
            case TypeBaseEnemy.Manusia:
            case TypeBaseEnemy.Turret:
                TrejectoryLurus();
                InstantiateBullet(prefabBullet);
                break;
            case TypeBaseEnemy.Tank:
                TrejectoryParabolic();
                InstantiateBullet(prefabBullet);
                break;
            case TypeBaseEnemy.Drone:
                TrejectoryBawah();
                InstantiateBullet(prefabBullet);
                break;
        }
    }
    private void Attack3()
    {
        switch (type)
        {
            case TypeBaseEnemy.Manusia:
            case TypeBaseEnemy.Turret:
                TrejectoryLurus();
                InstantiateBullet(prefabBullet);
                break;
            case TypeBaseEnemy.Tank:
                TrejectoryParabolic();
                InstantiateBullet(prefabBullet);
                break;
            case TypeBaseEnemy.Drone:
                TrejectoryBawah();
                InstantiateBullet(prefabBullet);
                break;
        }
    }
    private void InstantiateBullet(GameObject prefabBullet)
    {

    }

    void TrejectoryLurus()
    {

    }
    void TrejectoryBawah()
    {

    }
    void TrejectoryParabolic()
    {

    }
}