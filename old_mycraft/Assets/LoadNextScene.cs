using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class LoadNextScene : MonoBehaviour
    {
        public float load_delay;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadNextScene1());
        }

        IEnumerator LoadNextScene1()
        {
            yield return new WaitForSeconds(load_delay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }
}