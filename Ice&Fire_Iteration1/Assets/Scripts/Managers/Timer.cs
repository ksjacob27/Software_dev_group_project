using UnityEngine;
using System.Collections;



/*
 * <summary>
 * </summary>
 * <param name=""></param>
 * <returns>void</returns>
 */
public class Timer : MonoBehaviour {
    private float        m_StartTime;
    private float        m_EndTime;
    private GameObject[] m_PlayerIds;


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    void Start() {
        m_StartTime = Time.time;
    }


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    void Update() {
        m_PlayerIds = GameObject.FindGameObjectsWithTag("Player");
        // object[] players = FindGameObjectWithTag('player');
    }


    /*
     * <summary>
     * </summary>
     * <param name=""></param>
     * <returns>void</returns>
     */
    void Stop() {
        m_EndTime = Time.time;
    }
}
