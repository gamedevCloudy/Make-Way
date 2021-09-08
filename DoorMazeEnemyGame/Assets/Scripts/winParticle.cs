using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] cannon;
    [SerializeField] private GameObject winUI;
    

    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < cannon.Length; i++)
        {
            cannon[i].Play();
        }
        Invoke("WinGame", 2f);
       
    }

    private void WinGame()
    {
        Debug.Log("Win");
        winUI.SetActive(true);
    }
}
