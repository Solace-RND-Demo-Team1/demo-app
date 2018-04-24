using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    public class KeyboardGamePad : MonoBehaviour
    {

        private CustomCarController m_Car;
        private int counter;
        private int bumpCounter;
        private int bumpBackoff;

        public void Awake()
        {
            m_Car = GetComponent<CustomCarController>();
            counter = 0;
            bumpCounter = 0;
            bumpBackoff = 0;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (bumpBackoff == 0 && Input.GetKeyDown("space"))
            {
                bumpCounter = 10;
                bumpBackoff = 60;
            }
            if (Input.GetKeyDown("f"))
            {
                Debug.Log("f pressed");
                counter = 120;
           
            }
            if (counter > 0)
            {
                counter -= counter > 0 ? 1 : 0;
                m_Car.Move(0f, 100f, 100f, 0f);
            } else if (bumpCounter > 0 || bumpBackoff > 0) {

                bumpCounter -= bumpCounter > 0 ? 1 : 0;
                bumpBackoff -= bumpBackoff > 0 ? 1 : 0;
                if (bumpCounter > 0)
                    m_Car.Bump();
            } else {
                // pass the input to the car!
                
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                
                m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            }

        }
    }
}
