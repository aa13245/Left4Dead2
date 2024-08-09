using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTable_JSW;

 public class ItemTable_JSW : MonoBehaviour
{
    public static ItemTable_JSW instance;

    // 카테고리별 구조체
    public struct MainWeapon
    {
        public int magazineCapacity;
        public int maxAmmoCapacity;

        public float baseDmg;
        public float[] headDmg;
        public float[] bodyDmg;
        public float[] stomachDmg;
        public float[] limbDmg;

        public float maxRange;
        public float fireRate;
        public float reloadSpeed;

        public bool isSniper;
        public bool isShotgun;
        public int gauge;

        public float minRecoil;
        public float maxRecoil;
        public float recoil;
        #region 생성자
        public MainWeapon(int magazineCapacity, int maxAmmoCapacity, float baseDmg, float[] headDmg, float[] bodyDmg,
                      float[] stomachDmg, float[] limbDmg, float maxRange, float fireRate, float reloadSpeed,
                      bool isSniper, bool isShotgun, int gauge, float minRecoil, float maxRecoil, float recoil)
        {
            this.magazineCapacity = magazineCapacity;
            this.maxAmmoCapacity = maxAmmoCapacity;
            this.baseDmg = baseDmg;
            this.headDmg = headDmg;
            this.bodyDmg = bodyDmg;
            this.stomachDmg = stomachDmg;
            this.limbDmg = limbDmg;
            this.maxRange = maxRange;
            this.fireRate = fireRate;
            this.reloadSpeed = reloadSpeed;
            this.isSniper = isSniper;
            this.isShotgun = isShotgun;
            this.gauge = gauge;
            this.minRecoil = minRecoil;
            this.maxRecoil = maxRecoil;
            this.recoil = recoil;
        }
        #endregion
    }
    public struct SubWeapon
    {
        public int magazineCapacity;

        public float baseDmg;
        public float[] headDmg;
        public float[] bodyDmg;
        public float[] stomachDmg;
        public float[] limbDmg;

        public float maxRange;
        public float fireRate;
        public float reloadSpeed;

        public float minRecoil;
        public float maxRecoil;
        public float recoil;
        #region 생성자
        public SubWeapon(int magazineCapacity, float baseDmg, float[] headDmg, float[] bodyDmg,
                      float[] stomachDmg, float[] limbDmg, float maxRange, float fireRate, float reloadSpeed, float minRecoil, float maxRecoil, float recoil)
        {
            this.magazineCapacity = magazineCapacity;
            this.baseDmg = baseDmg;
            this.headDmg = headDmg;
            this.bodyDmg = bodyDmg;
            this.stomachDmg = stomachDmg;
            this.limbDmg = limbDmg;
            this.maxRange = maxRange;
            this.fireRate = fireRate;
            this.reloadSpeed = reloadSpeed;
            this.minRecoil = minRecoil;
            this.maxRecoil = maxRecoil;
            this.recoil = recoil;
        }
        #endregion
    }
    public struct MeleeWeapon
    {
        public float speed;
        public float horizontalRange;
        public float teamKillDmg;
        public bool isBlade;
        #region 생성자
        public MeleeWeapon(float speed, float horizontalRange, float teamKillDmg, bool isBlade)
        {
            this.speed = speed;
            this.horizontalRange = horizontalRange;
            this.teamKillDmg = teamKillDmg;
            this.isBlade = isBlade;
        }
        #endregion
    }
    public struct Projectile
    {
        public float dmg;
        public float range;
        public float time;
        public Projectile(float dmg, float range, float time)
        {
            this.dmg = dmg;
            this.range = range;
            this.time = time;
        }
    }
    public struct Recovery
    {
        public float value;
        public float time;
        public Recovery(float value, float time)
        {
            this.value = value;
            this.time = time;
        }
    }

    // 아이템 enum
    public enum Items
    {
        none,
        ak47,
        pistol,
        electricGuitar,
        crowbar,
        pipeBomb,
        molotov,
        medikit
    }
    // 아이템 테이블
    public Dictionary<Items, object> itemTable = new Dictionary<Items, object> { };
    public MainWeapon ak47;
    public SubWeapon pistol;
    public MeleeWeapon electricGuitar, crowbar;
    public Projectile pipeBomb, molotov;
    public Recovery medikit;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        // 아이템 정보
        ak47 = new MainWeapon(
            magazineCapacity: 40,
            maxAmmoCapacity: 360,
            baseDmg: 58,
            headDmg: new float[] { 211.74f, 232 },
            bodyDmg: new float[] { 52.94f, 58 },
            stomachDmg: new float[] { 66.17f, 72.5f },
            limbDmg: new float[] { 39.7f, 43.5f },
            maxRange: 57.14f,
            fireRate: 0.13f,
            reloadSpeed: 2.3833f,
            isSniper: false,
            isShotgun: false,
            gauge: 0,
            minRecoil: 2,
            maxRecoil: 5,
            recoil: 3
        );
        itemTable[Items.ak47] = ak47;
        pistol = new SubWeapon(
            magazineCapacity: 15,
            baseDmg: 36,
            headDmg: new float[] { 34.17f, 144 },
            bodyDmg: new float[] { 8.54f, 36 },
            stomachDmg: new float[] { 10.68f, 45 },
            limbDmg: new float[] { 6.41f, 27 },
            maxRange: 57.14f,
            fireRate: 0.2f,
            reloadSpeed: 1.6667f,
            minRecoil: 2,
            maxRecoil: 5,
            recoil: 2
        );
        itemTable[Items.pistol] = pistol;
        electricGuitar = new MeleeWeapon(
            speed: 1,
            horizontalRange: 16,
            teamKillDmg: 7,
            isBlade: false
        );
        itemTable[Items.electricGuitar] = electricGuitar;
        crowbar = new MeleeWeapon(
            speed: 0.8f,
            horizontalRange: 10,
            teamKillDmg: 5,
            isBlade: true
        );
        itemTable[Items.crowbar] = crowbar;
        pipeBomb = new Projectile(
            dmg: 750,
            range: 14.29f,
            time: 6
        );
        itemTable[Items.pipeBomb] = pipeBomb;
        molotov = new Projectile(
            dmg: 10,
            range: 15,
            time: 20
        );
        itemTable[Items.molotov] = molotov;
        medikit = new Recovery(
            value: 80,
            time: 8
        );
        itemTable[Items.medikit] = medikit;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*  접근방법
        
        if (itemTable[Items.crowbar] is MeleeWeapon item)
        {
            print(item.isBlade);
        }

        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
