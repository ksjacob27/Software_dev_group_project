using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Object = System.Object;



// [RequireComponent(typeof(NavMeshAgent))]
public class Player : NetworkBehaviour {

    // Server Variables \\
    [SyncVar] public string _playerName = "....";
    [SyncVar] public string _BuildFile;

    // Header Variables \\
    [Header("Character")] private Entity        p_Entity;
    [Header("Character")] public  Builder       _PersistenceBuild;
    [Header("Character")] public  List<SpellScript>   _Spells;
    [Header("Character")] public  List<Ability> _Abilities;

    [Header("UI")] public GameObject _PlayerHUD;
    [Header("UI")] public HealthBar  _HealthHUD;

    // Variables \\
    static  Timer              _PlayerTimer_;
    private RoomManager p_Scene;

    // Path Variables \\
    public static readonly string _ConditionLibrary_ = "Effects/";

    // Event Functions \\
    public RoomManager Scene {
        get {
            if (p_Scene != null) {
                return p_Scene;
            }
            return p_Scene = NetworkManager.singleton as RoomManager;
        }
        set { p_Scene = value; }
    }

    
    
    // ---------------------------------------------------- Starters ---------------------------------------------------- \\
    /*[Client] */
    public override void OnStartClient() {
        foreach (Player player in Scene.Players) {
            player
                .gameObject
                .SetActive(false);
        }
        (Scene)?.Players.Add(this);
    }

    /*[Server] */
    public override void OnStartServer() {
        _PersistenceBuild = Builder.LoadDataFromString(_BuildFile);
        Transform  spawnPoint = NetworkManager.singleton.GetStartPosition();
        GameObject eObject    = Instantiate(_PersistenceBuild.GetCharacterFromBuild().EntityPrefab, spawnPoint.position, spawnPoint.rotation);

        eObject.GetComponent<Entity>()._EntityNetId = netId;
        NetworkServer.Spawn(eObject, connectionToClient);
    }
    
    public override void OnStartAuthority() {
        _PersistenceBuild = Builder.LoadDataFromString(_BuildFile);
        _Abilities = _PersistenceBuild.GetAbilitiesFromBuild();
        _Spells = _PersistenceBuild.GetSpellsFromBuild();
        Dictionary<Ability, int> abilityDict = _Abilities.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        Dictionary<SpellScript, int>   spellDict   = _Spells.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());


        foreach (KeyValuePair<Ability, int> dPair in abilityDict) {
            RegisterPrefabrication(dPair.Key._AbilityPrefab, dPair.Value);
        }
        _BuildFile = _PersistenceBuild.ToString();


        Command_PlayerInstantiate();
    }


    // ---------------------------------------------------- Commands ---------------------------------------------------- \\
    public void GetFromPool(string aNAme, Vector3 position, Quaternion rotation) {
        Command_GetFromPool(aNAme, position, rotation);
    }


    public void RegisterPrefabrication(GameObject prefab, int count) {
        Command_RegisterPrefab(prefab.name, count);
    }


    // Damage registry initiation
    public void DealDamage(Health other, float amount) {
        if (hasAuthority)
            Command_DealDamage(other.GetComponent<NetworkIdentity>().netId, amount);
    }


    // Spell cast initiation
    public void Cast(string spellName) {
        Command_Cast(spellName);
    }


    // Ability initiation
    public void Action(string actionName) {
        Command_Action(actionName);
    }


    // Post Condition Effect (Debuff)
    public void PostDebuff(Health health, ConditionInventory inventory) {
        if (hasAuthority) { Command_PostDebuff(health, inventory.name); }
    }


    // Post Condition Effect (Buff)
    public void PostBuff(Health health, ConditionInventory inventory) {
        if (hasAuthority) { Command_PostBuff(health, inventory.name); }
    }


    // ---------------------------------------------------- Remote Command Calls ---------------------------------------------------- \\
    [Command] private void Command_GetFromPool(string aName, Vector3 position, Quaternion rotation) {
        GameObject newSpawn = ObjectPool._Singleton.GetFromPool(aName, position, rotation);
        newSpawn.GetComponent<NetworkIdentity>()
                .AssignClientAuthority(connectionToClient);
    }


    [Command] private void Command_PlayerInstantiate() {
        Client_PlayerInstantiate();
    }


    [Command] private void Command_RegisterPrefab(string aName, int count) {
        ObjectPool
            .RegisterPrefab(aName, count);
    }


    [Command] private void Command_DealDamage(uint id, float amount) {
        ObjectPool._Singleton._SpawnedObj[id]
                  .GetComponent<Health>()
                  .OnTakeDamage(amount, p_Entity);
    }


    [Command] private void Command_PlayerDeath() {
        Debug.Log($"Player: {p_Entity._EntityNetId} -> dying on server {connectionToClient}");
        StartCoroutine(WaitThenRespawn(5f));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellName"></param>
    [Command] private void Command_Cast(string spellName) {
        GameObject      spellObj = ObjectPool._Singleton.GetFromPool(spellName, p_Entity._SToolBox._AbilityPostRender.position, p_Entity._SToolBox._AbilityPostRender.rotation);
        NetworkIdentity id       = spellObj.GetComponent<NetworkIdentity>();

        id.AssignClientAuthority(p_Entity._Player.connectionToClient);
        Client_Action(id.netId);
    }


    [Command] private void Command_Action(string actName) {
        GameObject      actionObj = ObjectPool._Singleton.GetFromPool(actName, p_Entity._AToolBox._AbilityPostRender.position, p_Entity._AToolBox._AbilityPostRender.rotation);
        NetworkIdentity id        = actionObj.GetComponent<NetworkIdentity>();

        id.AssignClientAuthority(p_Entity._Player.connectionToClient);
        Client_Cast(id.netId);
    }


    /// <summary>
    /// Called by PostDebuff()
    /// </summary>
    /// <param name="healthToID"></param>
    /// <param name="debuffName"></param>
    [Command] public void Command_PostDebuff(NetworkBehaviour healthToID, string debuffName) {
        ConditionInventory effData = Resources.Load<ConditionInventory>($"{_ConditionLibrary_}{debuffName}");
        ObjectPool._Singleton._SpawnedObj[healthToID.netId]
                  .GetComponent<Health>().AddBuff(effData, p_Entity);
    }


    /// <summary>
    /// Called by PostBuff()
    /// </summary>
    /// <param name="healthToID"></param>
    /// <param name="buffName"></param>
    [Command] private void Command_PostBuff(NetworkBehaviour healthToID, string buffName) {
        ConditionInventory effData = Resources.Load<ConditionInventory>($"{_ConditionLibrary_}{buffName}");
        ObjectPool._Singleton._SpawnedObj[healthToID.netId]
                  .GetComponent<Health>().AddBuff(effData, p_Entity);
    }



    // ---------------------------------------------------- Client Callbacks ---------------------------------------------------- \\

    [Client] public override void OnStopClient() {
        Scene.Players.Remove(this);
    }


    [ClientRpc] private void Client_PlayerInstantiate() {
        Entity[]                 allEntities      = FindObjectsOfType<Entity>();
        List<Player>             players          = new List<Player>(FindObjectsOfType<Player>());
        Dictionary<uint, Player> playerDictionary = players.ToDictionary(x => x.netId, x => x);
        foreach (Entity ent in allEntities) {
            if (playerDictionary.ContainsKey(ent.netId) && !ent.Initiated) {
                ent.Init(playerDictionary[ent._EntityNetId]);
                playerDictionary[ent._EntityNetId].p_Entity = ent;

                ent._SToolBox._SpellList = Builder.LoadDataFromString(playerDictionary[ent._EntityNetId]._BuildFile).GetSpellsFromBuild();
                ent._AToolBox._ActiveAbilities = Builder.LoadDataFromString(playerDictionary[ent._EntityNetId]._BuildFile).GetAbilitiesFromBuild();

            }

            // TODO: TEST/CHECK to make sure a player is attached to an identity
        }
    }


    [Client] public void Client_SetPlayerAlias(string setName) {
        _playerName = setName;
    }


    [ClientRpc] private void Client_Cast(uint id) {
        Spell spellcast = ObjectPool._Singleton._SpawnedObj[id].GetComponent<Spell>();
        p_Entity._SToolBox.PostCast(spellcast);
        Debug.Log($"Player: {p_Entity._EntityNetId} -> Invokes Spell: {spellcast.name} on {connectionToClient}");

    }


    [ClientRpc] private void Client_Action(uint id) {
        Spell abilityAction = ObjectPool._Singleton._SpawnedObj[id].GetComponent<Spell>();
        p_Entity._SToolBox.PostCast(abilityAction);
        Debug.Log($"Player: {p_Entity._EntityNetId} -> Invokes Ability: {abilityAction.name} on {connectionToClient}");
    }


    [ClientRpc] private void Client_PlayerRespawn() {
        Debug.Log($"Player: {p_Entity._EntityNetId} -> respawning on {connectionToClient}");
        ((PlayerHealth)p_Entity._Health)?.Respawn();
    }
    
    
    



    // ---------------------------------------------------- Server Callbacks ---------------------------------------------------- \\
    [Server] public void Server_SetPlayerAlias(string aName) { _playerName = aName; }
    



    // ---------------------------------------------------- Events ---------------------------------------------------- \\
    public void AlterCurrentHealthStatus(float value) { p_Entity._Health._CurrentHealth += value; }
    public void SetMaxHealthStatus(float       value) { p_Entity._Health._MaxHealth = value; }
    public void SetDeathStatus(bool            value) { p_Entity._Health._IsDead = value; }
    public void Client_PlayerDead() {
        if (hasAuthority) { Command_PlayerDeath(); }
    }


    private IEnumerator WaitThenRespawn(float waitTime) {
        yield return new WaitForSeconds(waitTime);

        p_Entity._Health._CurrentHealth = p_Entity._Health._MaxHealth;
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();

        Transform ptransform = p_Entity.transform;
        ptransform.position = spawnPoint.position;
        ptransform.rotation = spawnPoint.rotation;
        Client_PlayerRespawn();
    }



}
