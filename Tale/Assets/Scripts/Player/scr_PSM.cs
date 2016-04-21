using UnityEngine;
using System.Collections;

public class scr_PSM : MonoBehaviour {
    private bool playerStateModified, equipStateModified, ropeStateModified, playerposeModified;

    void Awake()
    {
        playerStateModified = equipStateModified = ropeStateModified = playerposeModified = false;
    }
    public enum Playerstate
    {
        state_airborne,
        state_grounded,
    }
    public enum RopeState
    {
        ropestate_none,
        ropestate_hanging,
        ropestate_climbing,
        ropestate_skimming,
        ropestate_swinging,
        ropestate_pulling,
    }
    public enum Equipstate
    {
        equip_none,
        equip_rope,
        equip_bow,
        equip_other,
    }
    public enum PlayerPose 
    {
        pose_standing,
        pose_crouching,
        pose_running,
        pose_jogging,
        pose_climbing,
        pose_idle,
    }

    Playerstate m_playerState;
    Equipstate m_equipState;
    RopeState m_ropeState;
    PlayerPose m_playerPose;

    Playerstate m_prevPlayerState;
    Equipstate m_prevEquipState;
    RopeState m_prevRopeState;
    PlayerPose m_prevPlayerPose;

    void LateUpdate()
    {
        playerStateModified = equipStateModified = ropeStateModified = playerposeModified = false;
    }
    public Playerstate GetPlayerState(bool wantCurrent)
    {
        if (wantCurrent)
        {
            return m_playerState;
        }
        else
        {
            return m_prevPlayerState;

        }
    }
    public Equipstate GetEquipState(bool wantCurrent)
    {
        if (wantCurrent)
        {
            return m_equipState;
        }
        else
        {
            return m_prevEquipState;
        }
    }
    public RopeState GetRopeState(bool wantCurrent)
    {
        if (wantCurrent)
        {
            return m_ropeState;
        }
        else
        {
            return m_prevRopeState;
        }
    }
    public PlayerPose GetPlayerPose(bool wantCurrent)
    {
        if (wantCurrent)
        {
            return m_playerPose;
        }
        else
        {
            return m_prevPlayerPose;
        }
    }
    public bool IsPlayerStateChanged()
    {
        return playerStateModified;
    }
    public bool IsEquipStateChanged()
    {
        return equipStateModified;
    }
    public bool IsRopestateChanged()
    {
        return ropeStateModified;
    }
    public bool IsPlayerPoseChanged()
    {
        return playerposeModified;
    }

    public void SetPlayerState(Playerstate p_state){
        m_prevPlayerState = m_playerState;
        m_playerState = p_state;
        playerStateModified = true;
    }
    public void SetEquipState(Equipstate p_state){
        m_prevEquipState = m_equipState;
        m_equipState = p_state;
        equipStateModified = true;
    }
    public void SetRopeState(RopeState p_state){
        m_prevRopeState = m_ropeState;
        m_ropeState = p_state;
        ropeStateModified = true;
    }
    public void SetPlayerPose(PlayerPose p_pose){
      m_prevPlayerPose = m_playerPose;
        m_playerPose = p_pose;
        playerposeModified = true;
    }
}
